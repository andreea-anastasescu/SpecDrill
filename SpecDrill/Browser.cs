using Microsoft.Extensions.Logging;
using SpecDrill.AutomationScopes;
using SpecDrill.Configuration;
using SpecDrill.Configuration.Homepages;
using SpecDrill.Exceptions;
using SpecDrill.Infrastructure;
using SpecDrill.Infrastructure.Enums;
using SpecDrill.Secondary.Ports.AutomationFramework;
using SpecDrill.Secondary.Ports.AutomationFramework.Core;
using SpecDrill.Secondary.Ports.AutomationFramework.Exceptions;
using SpecDrill.Secondary.Ports.AutomationFramework.Model;
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

        private readonly Settings configuration = new Settings();

        private readonly ILogger Logger = DI.GetLogger<Browser>();

        private readonly IBrowserDriver browserDriver;

        private static readonly Stack<TimeSpan> timeoutHistory = new Stack<TimeSpan>();

        public Browser(IRuntimeServices runtimeServices, Settings configuration)
        {
            Trace.Write($"Configuration = {(configuration?.ToString() ?? "(null)")}");

            this.configuration = configuration ?? throw new MissingConfigurationException("Configuration is missing!");

            Logger.LogInformation("Initializing Driver...");
            var driverFactory = runtimeServices.GetBrowserFactoryBuilder(configuration);

            var browserName = this.configuration?.WebDriver?.Browser?.BrowserName.ToEnum<BrowserNames>();
            Logger.LogInformation($"WebDriver.BrowserDriver = {(browserName)}");
            browserDriver = driverFactory.Create(browserName ?? BrowserNames.chrome);

            if (configuration?.WebDriver?.Mode.ToEnum<Modes>() == Modes.browser)
            {
                var isMaximized = configuration?.WebDriver?.Browser?.Window?.IsMaximized ?? false;
                // configuring browser window
                Logger.LogInformation($"BrowserWindow.IsMaximized = {isMaximized}");

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
            Logger.LogInformation($"MaxWait = {waitMilliseconds}ms");
            var cfgMaxWait = TimeSpan.FromMilliseconds(waitMilliseconds);

            // set initial browser driver timeout to configuration or 1 minute if not defined
            lock (timeoutHistory)
            {
                timeoutHistory.Push(cfgMaxWait);
                browserDriver.ChangeBrowserDriverTimeout(cfgMaxWait);
            }

            FrameworkInit(runtimeServices, this);
        }

        private static void FrameworkInit(IRuntimeServices runtimeServices, IBrowser @this)
        {
            browserInstance = @this;
            ElementFactory.Instance = runtimeServices.GetElementFactory(@this);
            ElementLocatorFactory.Instance = runtimeServices.ElementLocatorFactory;
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
            string BuildFileSystemPath(HomepageConfiguration homePage) => string.Format("file:///{0}{1}",
                            Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)??"".Replace('\\', '/'),
                            homePage.Url);
            
            if (homePage != null)
            {
                var url = homePage.IsFileSystemPath ? BuildFileSystemPath(homePage) : homePage.Url ?? "";

                Logger.LogInformation($"Browser opening {url}");
                
                Action navigateToUrl = () => this.GoToUrl(url);

                navigateToUrl();

                var targetPage = this.CreatePage<T>();

                Wait.WithRetry().Doing(navigateToUrl).Until(() => targetPage.IsLoaded);
                targetPage.WaitForSilence();
                return targetPage;
            }
            string errMsg = $"SpecDrill: Page({ typeof(T).Name }) cannot be found in Homepages section of settings file.";
            Logger.LogInformation(errMsg);
            throw new MissingHomepageConfigEntryException(errMsg);
        }

        public T CreatePage<T>() where T : class, INavigationTargetElement => CreateContainer<T>();
        public T CreateControl<T>(IElement? parent, IElementLocator elementLocator) where T : class, IElement
        {
            T? fromInstance;
            Type toInstantiate = typeof(T);
            
            if (toInstantiate.IsInterface)
            {
                if (!typeof(INavigationElement<>).MakeGenericType(toInstantiate.GenericTypeArguments).IsAssignableFrom(typeof(T)))
                {
                    throw new NotSupportedException("CreateControl currently can only promote to instance the following interfaces: INavigationElement<>,.");
                }
                toInstantiate = typeof(NavigationElement<>).MakeGenericType(toInstantiate.GenericTypeArguments);
                var targetLocator = GetTargetLocatorFromNavigationTarget(null, toInstantiate.GenericTypeArguments[0]);
                fromInstance = Activator.CreateInstance(toInstantiate, parent, elementLocator, targetLocator) as T;
            }
            else
            {
                fromInstance = Activator.CreateInstance(toInstantiate, parent, elementLocator) as T;
            }

            return (fromInstance != default && fromInstance.GetType() == fromInstance.Parent?.GetType()) ?
                  fromInstance :
                  CreateContainer<T>(fromInstance);
        }

        public T CreateTarget<T>(IElement? parent = null, IElementLocator? elementLocator = null) where T : class, INavigationTargetElement
        {
            if (typeof(IPage).IsAssignableFrom(typeof(T)))
            {
                return this.CreatePage<T>();
            }
            else
            {
                return elementLocator == null
                    ? throw new ArgumentNullException($@"Argument {nameof(elementLocator)} of {nameof(CreateTarget)} must not be null!")
                    : this.CreateControl<T>(parent, elementLocator);
            }
        }

        private T CreateContainer<T>(T? containerInstance = default(T))
            where T : class, IElement
        {
            var container = EnsureContainerInstance(containerInstance);

            Type containerType = typeof(T);

            var members = containerType.GetMembers()
                .Where(member =>
                 (member.MemberType == MemberTypes.Property || member.MemberType == MemberTypes.Field))
                .ToDictionary(mi => mi.Name, mi => mi);
                
              members.Values.ToList().ForEach(
              (Action<MemberInfo>)(member =>
                {
                    var memberType = GetMemberType(member);
                    var memberFindAttributes = member.GetCustomAttributes<FindAttribute>(false)
                    .ToList<FindAttribute>();

                    if (memberFindAttributes.Any<FindAttribute>())
                    {
                        var memberValue = GetMemberValue(member, container);

                        if (memberValue != null)
                            return;

                        memberFindAttributes.ForEach //TODO: Currently if many attributes apply, last one wins; Should throw exception !
                        (
                            (Action<FindAttribute>)(findAttribute =>
                            {
                                var navigationTargetLocator = FigureOutNavigationTargetLocator(member, containerType, members, memberType);
                                object? element = InstantiateMember<T>(findAttribute, container, memberType, navigationTargetLocator);

                                SetValue(containerType, member, instance: container, value: element);
                            })
                        );
                    }
                }));
            return (T)container;
        }

        private IElementLocator? FigureOutNavigationTargetLocator(MemberInfo member, Type containerType, Dictionary<string, MemberInfo> members, Type memberType)
        {
            
            Type? navigationTargetType;
            IElementLocator? navigationTargetLocator = null;

            if (typeof(INavigationElement<INavigationTargetElement>).IsAssignableFrom(memberType))
            {
                navigationTargetType = memberType.GenericTypeArguments[0];
                if (typeof(IControl).IsAssignableFrom(navigationTargetType))
                {
                    var memberFindTargetAttributes = member.GetCustomAttributes<FindTargetAttribute>(false).ToList();

                    //if no target attribute defined
                    if (!memberFindTargetAttributes.Any())
                    {  // must have Find at Control (WebControl derived) class level
                        navigationTargetLocator = GetTargetLocatorFromNavigationTarget(member, navigationTargetType);
                    }
                    else
                    {
                        // We have [FindTarget]
                        var findTargetAttribute = memberFindTargetAttributes.Last();
                        var (selectorType, selectorValue) = (findTargetAttribute.SelectorType, findTargetAttribute.SelectorValue);

                        if (findTargetAttribute.PropertyName != null) // TargetProperty case
                        {
                            if (!members.ContainsKey(findTargetAttribute.PropertyName)) // member not foud (typo or class refactoring)
                                throw new WebControlTargetPropertyNotFoundException($"No member named `{findTargetAttribute.PropertyName}` could be found in `{containerType.Name}` type");

                            var navigationTargetMember = members[findTargetAttribute.PropertyName];
                            if (!typeof(INavigationTargetElement).IsAssignableFrom(GetMemberType(navigationTargetMember))) // member does not have INavigationTargetElement type (meaning it is not a Page or a Control)
                            { // This should not be possible due to Generic type parameter compiler check !
                                throw new TargetPropertyIsNotWebControlException($"[FindTarget] applied to ({member.DeclaringType?.Name ?? "N/A"}.{member.Name}) must point to a member of type WebPage or WebControl [IPage, IControl or INavigationTarget]");
                            }

                            var navigationTargetFindAttributes = navigationTargetMember.GetCustomAttributes<FindAttribute>().ToList();

                            if (!navigationTargetFindAttributes.Any()) // member has no [Find] attribute which poses a problem since we cannot get a target css window/popup locator
                                throw new ArgumentException($"Member `{findTargetAttribute.PropertyName}` has no [Find] attribute applied!");

                            var navigationTargetFindAttribute = navigationTargetFindAttributes.Last();
                            (selectorType, selectorValue) = (navigationTargetFindAttribute.SelectorType, navigationTargetFindAttribute.SelectorValue);

                            //navigationTargetLocator = ElementLocatorFactory.Create(navigationTargetFindAttribute.SelectorType, navigationTargetFindAttribute.SelectorValue); //locator from referenced Member

                        }

                        navigationTargetLocator = (selectorType, selectorValue) switch
                        {
                            (By st, string sv) => ElementLocatorFactory.Create(st, sv), //locator from [FindTarget] attribute
                            (null, string _) => throw new ArgumentNullException($"[FindTarget]'s SelectorType must not be null!"),
                            (By _, null) => throw new ArgumentNullException($"[FindTarget]'s SelectValue must not be null!"),
                            (null, null) => throw new ArgumentNullException($"[FindTarget]'s SelectorType and SelectValue must not be null!")
                        };
                    }

                }
            }
            return navigationTargetLocator;    
        }

        private static IElementLocator GetTargetLocatorFromNavigationTarget(MemberInfo? member, Type navigationTargetType)
        {
            IElementLocator? navigationTargetLocator;
            var targetTypeFindAttributes = navigationTargetType.GetCustomAttributes<FindAttribute>();
            if (!targetTypeFindAttributes.Any())
            {
                throw new NoFindTargetAttributeOnNavigationElementMemberNorFindAttributeOnTargetWebControlException($"Either: Member INavitationElement<{navigationTargetType.Name}> ({member?.DeclaringType?.Name ?? "N/A"}.{member?.Name??"N/A"}) must have [FindTarget] attribute applied Or: Target Control class ({navigationTargetType.Name}) must have [Find] attribute applied");
            }
            var targetTypeFindAttribute = targetTypeFindAttributes.Last();
            navigationTargetLocator = ElementLocatorFactory.Create(targetTypeFindAttribute.SelectorType, targetTypeFindAttribute.SelectorValue); // locator from target Control (css window/popup) type
            return navigationTargetLocator;
        }

        private static readonly string CREATE_NAVIGATION = "CreateNavigation";
        private static object? InstantiateMember<T>(FindAttribute findAttribute, IElement container, Type memberType, IElementLocator? targetLocator = null) where T : IElement
        {
            object? element = null;
            if (memberType == typeof(IElement))
            {
                element = ElementFactory.Create(findAttribute.Nested ? container : default(T),
                    ElementLocatorFactory.Create(findAttribute.SelectorType, findAttribute.SelectorValue));
            }
            else if (memberType == typeof(ISelectElement))
            {
                element = ElementFactory.CreateSelect(findAttribute.Nested ? container : default(T),
                    ElementLocatorFactory.Create(findAttribute.SelectorType, findAttribute.SelectorValue));
            }
            else if (typeof(INavigationElement<INavigationTargetElement>).IsAssignableFrom(memberType))
            {
                element = InvokeFactoryMethod(CREATE_NAVIGATION, memberType.GenericTypeArguments, container, findAttribute, targetLocator);
            }
            else if (typeof(IFrameElement<IPage>).IsAssignableFrom(memberType))
            {
                element = InvokeFactoryMethod("CreateFrame", memberType.GenericTypeArguments, container, findAttribute, null);
            }
            else if (typeof(IWindowElement<IPage>).IsAssignableFrom(memberType))
            {
                element = InvokeFactoryMethod("CreateWindow", memberType.GenericTypeArguments, container, findAttribute, null);
            }
            else if (typeof(WebControl).IsAssignableFrom(memberType))
            {
                if (memberType.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IListElement<>)))
                {
                    element = InvokeFactoryMethod("CreateList", memberType.GenericTypeArguments, container, findAttribute, null);
                }
                else
                {
                    element = InvokeFactoryMethod("CreateControl", new Type[] { memberType }, container, findAttribute, null);
                }
            }

            return element;
        }

        private static IElement EnsureContainerInstance<T>(T? containerInstance) where T : class, IElement
        {
            try
            {
                return ((containerInstance as IElement) ?? (T?)Activator.CreateInstance(typeof(T))) ?? throw new Exception("Could not ensure container!");
            }
            catch (MissingMethodException mme)
            {
                throw new MissingEmptyConstructorException($"SpecDrill: Page ({typeof(T).Name}) does not have a prameterless constructor. This error happens when you define at least one constructor with parameters. Possible Solution: Explicitly declare a parameterless constructor.", mme);
            }
        }

        private static object? InvokeFactoryMethod<T>(string methodName, Type[] genericTypeArguments, T page, FindAttribute findAttribute, IElementLocator? targetLocator) where T : IElement
        {
            object? element;
            MethodInfo? method = typeof(ElementFactory).GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);
            MethodInfo? generic = method?.MakeGenericMethod(genericTypeArguments);

            element = (methodName == CREATE_NAVIGATION) ?
                                    generic?.Invoke(null, new object?[] {
                                    findAttribute.Nested ? page : default,
                                    ElementLocatorFactory.Create(findAttribute.SelectorType, findAttribute.SelectorValue),
                                    targetLocator}) :
                                    generic?.Invoke(null, new object?[] {
                                    findAttribute.Nested ? page : default,
                                    ElementLocatorFactory.Create(findAttribute.SelectorType, findAttribute.SelectorValue)});
            return element;
        }

        private static object? GetMemberValue(MemberInfo member, object instance)
        {
            PropertyInfo? property = member as PropertyInfo;
            if (property != null)
            {
                return property.GetValue(instance);
            }
            FieldInfo? field = member as FieldInfo;
            if (field != null)
            {
                return field?.GetValue(instance);
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
            if (type != null && type.BaseType != null)
            {
                PropertyInfo? pInfo = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
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
            if (type != null && type.BaseType != null)
            {
                FieldInfo? pInfo = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                try
                {
                    pInfo?.SetValue(instance, value);
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
            var webElement = ElementFactory.Create(element.Parent, element.Locator);
            using (ImplicitTimeout(TimeSpan.FromSeconds(.5d))) // Wait max 500ms to conclude element is not present.
            {
                return webElement.NativeElementSearchResult();
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

        public SearchResult Find(IElementLocator locator, SearchResult? searchRoot = null)
        {
            
            var searchContext = searchRoot?.Elements.FirstOrDefault();

            if (searchRoot?.Locator?.IsShadowRoot ?? false)
            {
                Logger.LogInformation($"DOM LOOKUP: { searchRoot.Locator } (shadowRoot)");
                //searchContext = ExecuteJavascript($"return arguments[0].shadowRoot;", searchRoot?.Elements.FirstOrDefault() ?? throw new ElementNotFoundException("searchRoot is null. shadowRoot cannot be accessed!"));
                searchContext = searchRoot?.Elements.FirstOrDefault()?.GetShadowRoot();
            }

            Logger.LogInformation($"DOM LOOKUP: {searchRoot?.Locator?.ToString() ?? "ROOT"} :: {locator}");
            var elements = browserDriver.FindElements(locator, searchContext);

            if (locator.Index.HasValue)
            {
                if (locator.Index > elements.Count)
                {
                    throw new IndexOutOfRangeException($"SpecDrill: Browser.FindNativeElement : Not enough elements. You want element number {locator.Index} but only {elements.Count} were found.");
                }
                return SearchResult.Create(locator, searchRoot, elements[locator.Index.Value-1]);
            }

            return SearchResult.Create(locator, searchRoot, elements.ToArray());
        }

        public void JsLog(string logEntry)
        {
            Logger.LogInformation($"Browser.JsLog(logEntry={logEntry}");
            browserDriver.JsLog(logEntry);
        }

        public object? ExecuteJavascript(string script, params object[] arguments)
        {
            return browserDriver.ExecuteJavaScript(script, arguments);
        }

        public void Hover(IElement element)
        {
            browserDriver.MoveToElement(element);
        }

        public void Click(IElement element) => browserDriver.Click(element); 
        public void ClickJs(IElement element) => browserDriver.ClickJs(element);

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
        public void DoubleClickJs(IElement element) => this.browserDriver.DoubleClickJs(element);

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
            string? screenshotsPath;
            try
            {
                screenshotsPath = this.configuration?.WebDriver?.Screenshots?.Path ?? System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                var now = DateTime.Now;
                fileName = Path.Combine(string.Format("{0}", screenshotsPath),
                                        string.Format("{0}_{1:00}_{2:00}_{3:0000}_{4:00}_{5:00}_{6:00}_{7:000}.png",
                                         string.Format($"{testClassName}_{testMethodName}"),
                                         now.Day, now.Month, now.Year, now.Hour, now.Minute, now.Second, now.Millisecond));
                Logger.LogInformation($"Saving screensot `{fileName}`");
                this.browserDriver.SaveScreenshot(fileName);
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Could not save Screenshot `{fileName}`.");
            }
        }

        public Dictionary<string, object> GetCapabilities()
            => this.browserDriver.GetCapabilities();

        public void ClickAndDrag((int x, int y) from, int offsetX, int offsetY)
        {
            this.browserDriver.ClickAndDrag(from, offsetX, offsetY);
        }
    }
}
