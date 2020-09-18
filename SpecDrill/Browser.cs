using SpecDrill.AutomationScopes;
using SpecDrill.Configuration;
using SpecDrill.Exceptions;
using SpecDrill.Infrastructure;
using SpecDrill.Infrastructure.Enums;
using SpecDrill.Infrastructure.Logging;
using SpecDrill.Infrastructure.Logging.Interfaces;
using SpecDrill.SecondaryPorts.AutomationFramework;
using SpecDrill.SecondaryPorts.AutomationFramework.Core;
using SpecDrill.SecondaryPorts.AutomationFramework.Model;
using SpecDrill.WebControls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SpecDrill
{
    public sealed class Browser : IBrowser
    {
        private static IBrowser? browserInstance;

        private readonly Settings configuration;

        private readonly ILogger Log = Infrastructure.Logging.Log.Get<Browser>();

        private readonly IBrowserDriver browserDriver;

        private static readonly Stack<TimeSpan> timeoutHistory = new Stack<TimeSpan>();

        public Browser(IRuntimeServices runtimeServices, Settings configuration)
        {
            Trace.Write($"Configuration = {(configuration?.ToString() ?? "(null)")}");

            this.configuration = configuration ?? throw new MissingConfigurationException("Configuration is missing!");

            Log.Info("Initializing Driver...");
            var driverFactory = runtimeServices.GetBrowserFactoryBuilder(configuration);

            var browserName = this.configuration?.WebDriver?.Browser?.BrowserName.ToEnum<BrowserNames>();
            Log.Info($"WebDriver.BrowserDriver = {(browserName)}");
            browserDriver = driverFactory.Create(browserName ?? BrowserNames.chrome);

            if (configuration?.WebDriver?.Mode.ToEnum<Modes>() == Modes.browser)
            {
                var isMaximized = configuration?.WebDriver?.Browser?.Window?.IsMaximized ?? false;
                // configuring browser window
                Log.Info($"BrowserWindow.IsMaximized = {isMaximized}");

                if (isMaximized)
                {
                    MaximizePage();
                }
                else
                {
                    SetWindowSize(configuration?.WebDriver?.Browser?.Window?.InitialWidth ?? 800, configuration?.WebDriver?.Browser?.Window?.InitialHeight ?? 600);
                }
            }
            var maxWait = configuration?.WebDriver?.MaxWait;
            long waitMilliseconds = Math.Max(maxWait ?? 0, 60000L);
            Log.Info($"MaxWait = {waitMilliseconds}ms");
            var cfgMaxWait = TimeSpan.FromMilliseconds(waitMilliseconds);

            // set initial browser driver timeout to configuration or 1 minute if not defined
            lock (timeoutHistory)
            {
                timeoutHistory.Push(cfgMaxWait);
                browserDriver.ChangeBrowserDriverTimeout(cfgMaxWait);
            }

            FrameworkInit(runtimeServices);
        }

        private void FrameworkInit(IRuntimeServices runtimeServices)
        {
            browserInstance = this;
            WebElement.ElementFactory = runtimeServices.GetElementFactory(this);
            ElementLocator.ElementLocatorFactory = runtimeServices.ElementLocatorFactory;
        }

        public void SetWindowSize(int initialWidth, int initialHeight)
        {
            this.browserDriver.SetWindowSize(initialWidth, initialHeight);
        }

        public static IBrowser Instance => browserInstance ?? throw new Exception("Browser could not be instantiated!");

        public T Open<T>()
            where T : class, IPage
        {
            var homePage = configuration.Homepages.FirstOrDefault(homepage => homepage.PageObjectType == typeof(T).Name);
            if (homePage != null)
            {
                string url = string.Format("file:///{0}{1}",
                            Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location).Replace('\\', '/'),
                            homePage.Url);
                Log.Info($"Browser opening {url}");
                Action navigateToUrl = homePage.IsFileSystemPath ?
                    (Action)(() =>
                    this.GoToUrl(url)) : () => this.GoToUrl(homePage.Url ?? "");

                navigateToUrl();

                var targetPage = this.CreatePage<T>();

                Wait.WithRetry().Doing(navigateToUrl).Until(() => targetPage.IsLoaded);
                targetPage.WaitForSilence();
                return targetPage;
            }
            string errMsg = $"SpecDrill: Page({ typeof(T).Name }) cannot be found in Homepages section of settings file.";
            Log.Info(errMsg);
            throw new MissingHomepageConfigEntryException(errMsg);
        }

        public T CreatePage<T>() where T : class, IPage => CreateContainer<T>();
        public T CreateControl<T>(T? fromInstance) where T : class, IElement => CreateContainer<T>(fromInstance);

        private T CreateContainer<T>(T? containerInstance = default(T))
            where T : class, IElement
        {
            var container = EnsureContainerInstance<T>(containerInstance);

            Type containerType = typeof(T);

            containerType.GetMembers()
                .Where(member => member.MemberType == MemberTypes.Property || member.MemberType == MemberTypes.Field)
                .ToList()
                .ForEach(
                member =>
                {
                    var memberType = GetMemberType(member);
                    var memberAttributes = member.GetCustomAttributes<FindAttribute>(false)
                    .ToList();

                    if (memberAttributes.Any())
                    {
                        var memberValue = GetMemberValue(member, container);

                        if (memberValue != null)
                            return;

                        memberAttributes
                        .ForEach //TODO: Currently if many attributes apply, last one wins; Should throw exception !
                        (
                            findAttribute =>
                            {
                                object? element = InstantiateMember<T>(findAttribute, container, memberType);

                                SetValue(containerType, member, instance: container, value: element);
                            }
                        );
                    }
                });
            return (T)container;
        }

        private static object? InstantiateMember<T>(FindAttribute findAttribute, IElement container, Type memberType) where T : IElement
        {
            object? element = null;
            if (memberType == typeof(IElement))
            {
                element = WebElement.Create(findAttribute.Nested ? container : default(T),
                    ElementLocator.Create(findAttribute.SelectorType, findAttribute.SelectorValue));
            }
            else if (memberType == typeof(ISelectElement))
            {
                element = WebElement.CreateSelect(findAttribute.Nested ? container : default(T),
                    ElementLocator.Create(findAttribute.SelectorType, findAttribute.SelectorValue));
            }
            else if (typeof(INavigationElement<IPage>).IsAssignableFrom(memberType))
            {
                element = InvokeFactoryMethod("CreateNavigation", memberType.GenericTypeArguments, container, findAttribute);
            }
            else if (typeof(IFrameElement<IPage>).IsAssignableFrom(memberType))
            {
                element = InvokeFactoryMethod("CreateFrame", memberType.GenericTypeArguments, container, findAttribute);
            }
            else if (typeof(IWindowElement<IPage>).IsAssignableFrom(memberType))
            {
                element = InvokeFactoryMethod("CreateWindow", memberType.GenericTypeArguments, container, findAttribute);
            }
            else if (typeof(WebControl).IsAssignableFrom(memberType))
            {
                if (memberType.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IListElement<>)))
                {
                    element = InvokeFactoryMethod("CreateList", memberType.GenericTypeArguments, container, findAttribute);
                }
                else
                {
                    element = InvokeFactoryMethod("CreateControl", new Type[] { memberType }, container, findAttribute);
                }
            }

            return element;
        }

        private IElement EnsureContainerInstance<T>(T? containerInstance) where T : class, IElement
        {
            try
            {
                return (containerInstance as IElement) ?? (T)Activator.CreateInstance(typeof(T));
            }
            catch (MissingMethodException mme)
            {
                throw new MissingEmptyConstructorException($"SpecDrill: Page ({typeof(T).Name}) does not have a prameterless constructor. This error happens when you define at least one constructor with parameters. Possible Solution: Explicitly declare a parameterless constructor.", mme);
            }
        }

        private static object InvokeFactoryMethod<T>(string methodName, Type[] genericTypeArguments, T page, FindAttribute findAttribute) where T : IElement
        {
            object element;
            MethodInfo method = typeof(WebElement).GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);
            MethodInfo generic = method.MakeGenericMethod(genericTypeArguments);
            element = generic.Invoke(null, new object?[] {
                                    findAttribute.Nested ? page : default,
                                    ElementLocator.Create(findAttribute.SelectorType, findAttribute.SelectorValue) });
            return element;
        }

        private object? GetMemberValue(MemberInfo member, object instance)
        {
            PropertyInfo? property = member as PropertyInfo;
            if (property != null)
            {
                return property.GetValue(instance);
            }
            FieldInfo? field = member as FieldInfo;
            if (field != null)
            {
                return field.GetValue(instance);
            }
            return null;
        }

        private void SetValue(Type containerType, MemberInfo member, object instance, object? value)
        {
            PropertyInfo? property = member as PropertyInfo;
            if (property != null)
            {
                var propertyName = property.Name;

                SetPropertyValue(containerType, propertyName, instance, value);
            }
            FieldInfo? field = member as FieldInfo;
            if (field != null)
            {
                var fieldName = field.Name;

                SetFieldValue(containerType, fieldName, instance, value);
            }
        }

        private void SetPropertyValue(Type type, string propertyName, object instance, object? value)
        {
            if (type != null)
            {
                PropertyInfo pInfo = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var setter = pInfo?.GetSetMethod(true);
                if (setter != null)
                {
                    setter.Invoke(instance, new object?[] { value });
                    return;
                }

                SetPropertyValue(type.BaseType, propertyName, instance, value);
            }
            else
            {
                throw new DynamicMemberInitializationException($"SpecDrill: Could not set {propertyName}");
            }
        }

        private void SetFieldValue(Type type, string fieldName, object instance, object? value)
        {
            if (type != null)
            {
                FieldInfo pInfo = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                try
                {
                    pInfo.SetValue(instance, value);
                    return;
                }
                catch { }

                SetFieldValue(type.BaseType, fieldName, instance, value);
            }
            else
            {
                throw new DynamicMemberInitializationException($"SpecDrill: Could not set {fieldName}");
            }
        }
        private Type GetMemberType(MemberInfo member)
        {
            PropertyInfo? property = member as PropertyInfo;
            if (property != null) return property.PropertyType;
            FieldInfo? field = member as FieldInfo;
            if (field != null) return field.FieldType;

            throw new InvalidAttributeTargetException($"SpecDrill - Browser: Find attribute cannot be applied to members of type {member.GetType().FullName}");

        }

        public void GoToUrl(string url)
        {
            browserDriver.GoToUrl(url);
        }

        public string PageTitle
        {
            get { return browserDriver.Title; }
        }

        public bool IsAlertPresent => this.browserDriver.Alert != null;

        public IBrowserAlert Alert
        {
            get
            {
                var alert = this.browserDriver.Alert;
                if (alert == null)
                    throw new AlertNotFoundException("SpecDrill: No alert present!");
                return alert;
            }
        }

        public bool IsJQueryDefined => ((bool?)ExecuteJavascript("if (window.jQuery) return true else return false;")) ?? false;

        public Uri Url => browserDriver.Url;

        public IDisposable ImplicitTimeout(TimeSpan implicitTimeout, string? message = null)
        {
            return new ImplicitWaitScope(browserDriver, timeoutHistory, implicitTimeout, message);
        }

        //public IElement PeekElement(IElementLocator locator)
        //{
        //    using (ImplicitTimeout(TimeSpan.FromSeconds(1)))
        //    {
        //        var webElement = WebElement.Create(this, null, locator);
        //        var nativeElement = webElement.NativeElement;
        //        return nativeElement == null ? null : webElement;
        //    }
        //}

        public SearchResult PeekElement(IElement element)
        {
            var webElement = WebElement.Create(element.Parent, element.Locator);
            using (ImplicitTimeout(TimeSpan.FromSeconds(.5d))) // Wait max 500ms to conclude element is not present.
            {
                return webElement.NativeElementSearchResult;
            }
        }

        public void Exit()
        {
            browserDriver.Exit();
        }

        //public IElement FindElement(IElementLocator locator)
        //{
        //    return WebElement.Create(null, locator);
        //}

        //public IList<IElement> FindElements(IElementLocator locator)
        //{
        //    var elements = this.browserDriver.FindElements(locator);

        //    var elementCount = elements?.Count ?? 0;

        //    var result = new List<IElement>();
        //    if (elementCount > 0)
        //    {
        //        for (int i=0; i<elements.Count; i++)
        //        {
        //            result.Add(WebElement.Create(null, locator));
        //        }
        //    }

        //    return result;
        //}

        public SearchResult FindNativeElement(IElementLocator locator)
        {
            var elements = browserDriver.FindElements(locator);
            int index = 0;
            int count = 1;

            if (locator.Index.HasValue)
            {
                if (locator.Index > elements.Count)
                {
                    throw new IndexOutOfRangeException($"SpecDrill: Browser.FindNativeElement : Not enough elements. You want element number {locator.Index} but only {elements.Count} were found.");
                }
                index = locator.Index.Value;
                count = elements.Count;
            }

            //if (elements.Count == 0)
            //{
            //    throw new IndexOutOfRangeException($"SpecDrill: No elements found.");
            //}

            return SearchResult.Create(elements.Count > 0 ? elements[index] : null, elements.Count);
        }

        public object? ExecuteJavascript(string script, params object[] arguments)
        {
            return browserDriver.ExecuteJavaScript(script, arguments);
        }

        public void Hover(IElement element)
        {
            browserDriver.MoveToElement(element);
        }

        public void Click(IElement element)
        {
            browserDriver.Click(element);
        }
        public void DragAndDrop(IElement startFromElement, IElement stopToElement)
        {
            browserDriver.DragAndDrop(startFromElement, stopToElement);
        }

        public void RefreshPage()
        {
            browserDriver.RefreshPage();
        }

        public void MaximizePage()
        {
            browserDriver.MaximizePage();
        }

        public void SwitchToDocument()
        {
            browserDriver.SwitchToDocument();
        }

        void IBrowser.SwitchToFrame<T>(IFrameElement<T> seleniumFrameElement)
        {
            browserDriver.SwitchToFrame(seleniumFrameElement);
        }

        void IBrowser.SwitchToWindow<T>(IWindowElement<T> seleniumWindowElement)
        {
            browserDriver.SwitchToWindow(seleniumWindowElement);
        }

        public void CloseLastWindow()
        {
            browserDriver.CloseLastWindow();
        }

        public string GetPdfText()
        {
            return browserDriver.GetPdfText();
        }

        public void DoubleClick(IElement element) => this.browserDriver.DoubleClick(element);

        //public bool LoadJQuery()
        //{
        //    return (bool) this.ExecuteJavascript($"if (!window.jQuery) {{{jQueryScript} jQuery.noConflict(); return true;}} else {{return false;}}");
        //}

        public void DragAndDrop(IElement startFromElement, int offsetX, int offsetY)
        {
            this.browserDriver.DragAndDrop(startFromElement, offsetX, offsetY);
        }

        public void DragAndDropElement(IElement startFromElement, IElement stopToElement) => DragAndDrop(startFromElement, stopToElement);

        public void SaveScreenshot(string testClassName, string testMethodName)
        {
            string fileName = "";
            string screenshotsPath = "";
            try
            {
                screenshotsPath = this.configuration?.WebDriver?.Screenshots?.Path ?? System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                var now = DateTime.Now;
                fileName = Path.Combine(string.Format("{0}", screenshotsPath),
                                        string.Format("{0}_{1:00}_{2:00}_{3:0000}_{4:00}_{5:00}_{6:00}_{7:000}.png",
                                         string.Format($"{testClassName}_{testMethodName}"),
                                         now.Day, now.Month, now.Year, now.Hour, now.Minute, now.Second, now.Millisecond));
                this.browserDriver.SaveScreenshot(fileName);
            }
            catch (Exception e)
            {
                Log.Error(e, $"Could not save Screenshot `{fileName}`.");
            }
        }

        public Dictionary<string, object> GetCapabilities()
            => this.browserDriver.GetCapabilities();

    }
}
