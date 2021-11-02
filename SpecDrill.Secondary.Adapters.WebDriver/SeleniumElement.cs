using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using SpecDrill.Infrastructure;
using SpecDrill.Secondary.Adapters.WebDriver.Extensions;
using SpecDrill.Secondary.Ports.AutomationFramework;
using SpecDrill.Secondary.Ports.AutomationFramework.Core;
using SpecDrill.Secondary.Ports.AutomationFramework.Exceptions;
using SpecDrill.Secondary.Ports.AutomationFramework.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SpecDrill.Secondary.Adapters.WebDriver
{
    internal enum ClickType
    {
        Single,
        Double,
        Js
    }

    [Flags]
    internal enum ElementStateFlags
    {
        None = 0,
        Displayed = 1,
        Enabled = 2
    }
    internal class SeleniumElement : IElement
    {
        protected static readonly ILogger Logger = DI.GetLogger<SeleniumElement>();

        protected IBrowser? browser;
        protected IElementLocator locator;
        protected IElement? parent;

        public SeleniumElement(IBrowser? browser, IElement? parent, IElementLocator locator)
        {
            this.browser = browser;
            this.locator = locator;
            this.parent = parent;
        }

        public bool IsReadOnly
        {
            get { return this.Element.GetAttribute("readonly") != null; }
        }

        public bool IsAvailable => AvailabilityTest(this.NativeElementSearchResult());

        private bool AvailabilityTest(SearchResult searchResult)
        => Test(searchResult, "AvailabilityTest", (state) => state.HasFlag(ElementStateFlags.Displayed) && state.HasFlag(ElementStateFlags.Enabled)).Evaluate();

        public bool IsDisplayed => Test(this.NativeElementSearchResult(), "VisibilityTest", (state) => state.HasFlag(ElementStateFlags.Displayed)).Evaluate(throwException: true);
        public bool IsEnabled => Test(this.NativeElementSearchResult(), "IsEnabledTest", (state) => state.HasFlag(ElementStateFlags.Enabled)).Evaluate(throwException: true);

        //private (ElementStateFlags stateFlags, Exception? exception) InternalElementState
        //{

        //    // (ESF.None, exception) -> inconclusive - present but invalid object type ??? or not present
        //    // (ESF.None, null) -> Item state is according to ESF flags : is not shown and not enabled
        //    // (ESF.*, null) -> Item state is according to ESF flags
        //    get
        //    {
        //        Exception? exception = null;
        //        SearchResult? searchResult = null;
        //        string locatorString = "(null)";
        //        try 
        //        { 
        //            searchResult = Browser.PeekElement(this);
        //            locatorString = searchResult?.Locator?.ToString() ?? "(null)";
        //        }
        //        catch (NotFoundException nfe)
        //        {
        //            exception = nfe;
        //            Log.Error(exception, $"SpecDrill: Availability Test failed for {locatorString}");
        //            return (ElementStateFlags.None, exception);
        //        }

        //        if (searchResult == null || !searchResult.HasResult)
        //        {
        //            var info = $"Element ({locatorString}) not found!";
        //            Log.Info(info);
        //            exception = new NotFoundException(info);
        //            return (ElementStateFlags.None, exception);
        //        }

        //        if (searchResult.NativeElement == null)
        //        {
        //            var info = $"Element ({ locatorString }) is not an IWebElement!";
        //            Log.Info(info);
        //            return (ElementStateFlags.None, new Exception(info, exception));
        //        }

        //        return InternalStateTest(searchResult);
        //    }
        //}

        private (bool result, Exception? exception) Test(SearchResult searchResult, string testName, Func<ElementStateFlags, bool> test)
        {
            var resultUnderTest = searchResult;
            var (state, exception) = InternalStateTest(resultUnderTest);
            var locator = resultUnderTest.Locator;
            if (exception != null)
            {
                Logger.LogInformation(string.Format($"[TEST] {testName} test result for {locator}: false ; Reason {(exception?.ToString() ?? "(null)")}"));
            }

            var testResult = test(state);

            if (!testResult)
            {
                Logger.LogInformation(string.Format($"[TEST] {testName} result for {locator}: false > displayed:{state.HasFlag(ElementStateFlags.Displayed)}, enabled:{state.HasFlag(ElementStateFlags.Enabled)}"));
            }

            return (testResult, exception);
        }
        U ResilientAccess<U>(ref SearchResult searchResult, Func<IWebElement?, U> op)
            where U : struct
        {
            IWebElement? ToIWebElement(SearchResult searchResult)
                => (searchResult.Elements.FirstOrDefault() is IWebElement iwe) ?
                        iwe :
                        throw new ElementNotFoundException($"Element {locator} was not found!");

            U result = default;
            try
            {
                result = op(ToIWebElement(searchResult));
            }
            catch (ElementNotFoundException) { throw;  }
            catch
            {
                searchResult = Browser.Find(searchResult.Locator, searchResult.Container);
                result = op(ToIWebElement(searchResult));
            }
            return result;
        }
        private (ElementStateFlags state, Exception? exception) InternalStateTest(SearchResult resultUnderTest)
        {
            ElementStateFlags state = ElementStateFlags.None;
            Exception? exception = null;
            try
            {
                Logger.LogInformation($"Testing State for {resultUnderTest.Locator}");

                if (ResilientAccess(ref resultUnderTest, iwe => (iwe?.Displayed ?? false))) state |= ElementStateFlags.Displayed;
                if (ResilientAccess(ref resultUnderTest, iwe => (iwe?.Enabled ?? false))) state |= ElementStateFlags.Enabled;

                Logger.LogInformation($"state = {state}");
                return (state, exception);
            }
            catch (StaleElementReferenceException sere)
            {
                exception = sere;
                Logger.LogError(exception, $"SpecDrill: State Test for {locator}");
            }
            catch (NotFoundException nfe)
            {
                exception = nfe;
                Logger.LogError(exception, $"SpecDrill: State Test for {locator}");
            }
            catch (ElementNotFoundException enfe)
            {
                exception = enfe;
                Logger.LogError(exception, $"SpecDrill: State Test for {locator}");
            }
            catch (Exception e)
            {
                exception = e;
                Logger.LogError(exception, $"SpecDrill: State Test for {locator}");
            }

            return (state, exception);
        }

        public IBrowser Browser => this.browser ?? throw new Exception("Browser could not be instantiated!");

        private void Click(ClickType clickType, bool waitForSilence = false)
        {
            if (waitForSilence) { this.ContainingPage?.WaitForSilence(); }

            Logger.LogInformation("Clicking {0}", this.locator);
            try
            {
                if (clickType == ClickType.Single)
                {
                    this.Element.Click();
                }
                else
                {
                    this.browser?.DoubleClick(this);
                }
            }
            catch (StaleElementReferenceException sere)
            {
                Logger.LogError(sere, $"Click: Element {this.locator} is stale!");
                throw;
            }
            catch (ElementNotVisibleException enve)
            {
                Logger.LogError(enve, $"Element {this.locator} is not visible!");
                throw;
            }
            catch (InvalidOperationException ioe)
            {
                Logger.LogError(ioe, $"Clicking Element {this.locator} caused an InvalidOperationException!");
            }
        }

        public void DoubleClick(bool waitForSilence = false) => this.Click(ClickType.Double, waitForSilence);

        public void Click(bool waitForSilence = false) => this.Click(ClickType.Single, waitForSilence);

        public IElement SendKeys(string keys, bool waitForSilence = false)
        {
            if (waitForSilence) { this.ContainingPage?.WaitForSilence(); }
            this.Element.SendKeys(keys);
            return this;
        }

        public void Blur(bool waitForSilence = false)
        {
            if (waitForSilence) { this.ContainingPage?.WaitForSilence(); }
            this.Element.SendKeys("\t");
        }

        public string Text
        {
            get { return this.Element.Text; }
        }

        public string GetCssValue(string cssValueName)
        {
            try
            {
                return this.Element.GetCssValue(cssValueName) ?? string.Empty;
            }
            catch (StaleElementReferenceException sere)
            {
                Logger.LogError(sere, $"GetCssValue: Element {this.locator} is stale!");
            }

            return "";
        }

        public string GetAttribute(string attributeName)
        {
            try
            {
                return this.Element.GetAttribute(attributeName);
            }
            catch (StaleElementReferenceException sere)
            {
                Logger.LogError(sere, $"GetAttribute: Element {this.locator} is stale!");
            }
            catch (Exception e)
            {
                string z = e.Message;
            }

            return "";
        }
        public bool SetAttribute(string attributeName, string attributeValue)
        {
            try
            {
                Browser.ExecuteJavascript(@$"arguments[0].style.outline='{attributeValue}';", this.Element);
                return true;

            }
            catch (StaleElementReferenceException sere)
            {
                Logger.LogError(sere, $"GetAttribute: Element {this.locator} is stale!");
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"GetAttribute: Element {this.locator}.");
            }
            return false;
        }
        public void Hover(bool waitForSilence = false)
        {
            if (waitForSilence) { this.ContainingPage?.WaitForSilence(); }
            this.browser?.Hover(this);
        }

        public IElement Clear(bool waitForSilence = false)
        {
            if (waitForSilence) { this.ContainingPage?.WaitForSilence(); }
            this.Element.Clear();
            return this;
        }

        public IElement? SearchContainingPage(IElement element)
        {
            if (element is IPage)
                return element;

            return (element.Parent != null) ?
                        SearchContainingPage(element.Parent) :
                        null;
        }

        public IPage? ContainingPage => SearchContainingPage(this) as IPage;

        public SearchResult NativeElementSearchResult()
        {
            var elementContainers = DiscoverElementContainers();
            IElementLocator lastContainerLocator;
            if (elementContainers.Any())
            {
                elementContainers.Reverse();
                lastContainerLocator = elementContainers.First().Locator;
            }
            else
            {
                lastContainerLocator = SeleniumElementLocator.Create(Ports.AutomationFramework.By.TagName, "html");
            }

            var previousContainer = Browser.Find(lastContainerLocator);

            if (!previousContainer.HasResult)
                return SearchResult.Empty;

            bool isParentAvailable = AvailabilityTest(previousContainer);

            Logger.LogInformation($"Finding element {locator} which is nested {elementContainers.Count} level(s) deep. Its parent is{(isParentAvailable ? string.Empty : " not")} available.");
            Logger.LogInformation($"L00>{lastContainerLocator}");

            if (elementContainers.Count > 1)
            {
                for (int i = 1; i < elementContainers.Count; i++)
                {
                    var containerToSearch = elementContainers[i];
                    previousContainer = SearchElementInPreviousContainer(containerToSearch, previousContainer, i);

                    if (!previousContainer.HasResult)
                        return SearchResult.Empty;
                    //// HERE -> wat to do with container tosearch vs previous container . why last container locator is not used and 
                    //// why it was introduced? 
                    //lastContainerLocator = containerToSearch.Locator;
                }
            }

            Logger.LogInformation($"LOC>{locator}");

            return SearchElementInPreviousContainer(
                elementToSearch: this,
                previousContainer: previousContainer, elementContainers.Count);
        }

        private object[] ToNullFreeArray(IWebElement? element)
            => element == null ? new object[0] : new object[] { element };

        private IElementLocator CreateLocator(Ports.AutomationFramework.By locatorType, string selector, int? index = null, bool isShadowRoot = false)
            => new ElementLocatorFactory().Create(locatorType, locatorValue: selector, index, isShadowRoot);


        private List<IElement> DiscoverElementContainers()
        {
            List<IElement> elementContainers = new();

            IElement? current = this.Parent;

            if (current != null)
            {
                do
                {
                    if (current is IControl || current is IPage)
                    {
                        elementContainers.Add(current);
                    }
                    current = current.Parent;
                } while (current != null);
            }

            return elementContainers;
        }

        private const string CSS_SHADOW_DOM_TOKEN = ">>>";
        private const string XPATH_SHADOW_DOM_TOKEN = "!";
        private SearchResult SearchElementInPreviousContainer(IElement elementToSearch, SearchResult previousContainer, int depth)
        {
            if (previousContainer == null || !previousContainer.HasResult)
                return SearchResult.Empty;

            var previousContainerNativeElement = previousContainer;

            if (!AvailabilityTest(previousContainer))
            {
                Logger.LogError($"AvailabilityTest({previousContainer.Locator}) = False!");
                return SearchResult.Empty;
            }


            try
            {
                ////////////////////////////////////
                SearchResult? shadowRoot = null;

                var selector = elementToSearch.Locator.LocatorValue;
                var shadowDomParts = new string[0];
                if (selector.Contains(CSS_SHADOW_DOM_TOKEN))
                {
                    switch (elementToSearch.Locator.LocatorType)
                    {
                        case Ports.AutomationFramework.By.Id:
                        case Ports.AutomationFramework.By.Name:
                        case Ports.AutomationFramework.By.LinkText:
                        case Ports.AutomationFramework.By.PartialLinkText:
                        case Ports.AutomationFramework.By.ClassName:
                        case Ports.AutomationFramework.By.TagName:
                            elementToSearch = new SeleniumElement(elementToSearch.Browser,
                                elementToSearch.Parent,
                                CreateLocator(Locator.LocatorType, selector.Replace(CSS_SHADOW_DOM_TOKEN, string.Empty), Locator.Index));
                            break;
                        case Ports.AutomationFramework.By.CssSelector:

                            var cssSelector = elementToSearch.Locator.LocatorValue;
                            shadowDomParts = cssSelector.Split(CSS_SHADOW_DOM_TOKEN);

                            for (int pi = 0; pi < shadowDomParts.Length - 1; pi++)
                            {
                                var part = pi == 0 ?
                                    shadowDomParts[pi] :
                                    shadowDomParts[pi].TrimStart();

                                var locator = CreateLocator(Ports.AutomationFramework.By.CssSelector, part, isShadowRoot: true);

                                shadowRoot = FindElementsInContainer(locator, depth++,
                                    shadowRoot ?? previousContainerNativeElement);

                                Logger.LogInformation($"Reached {locator} #shadowRoot");
                            }

                            break;
                        case Ports.AutomationFramework.By.XPath:
                            var xPathSelector = elementToSearch.Locator.LocatorValue;
                            shadowDomParts = xPathSelector.Split('!', StringSplitOptions.RemoveEmptyEntries);
                            break;
                    }
                }
                if (shadowRoot != null)
                {
                    previousContainerNativeElement = shadowRoot;
                    var finalSelector = shadowDomParts.LastOrDefault();
                    
                    if (!string.IsNullOrWhiteSpace(finalSelector))
                    {
                        elementToSearch = new SeleniumElement(elementToSearch.Browser,
                                elementToSearch.Parent,
                                CreateLocator(elementToSearch.Locator.LocatorType, finalSelector /*shadowDomParts.Last()*/, elementToSearch.Locator.Index));
                    }
                    else
                    {
                        return shadowRoot;
                    }
                }
                ////////////////////////////////////
                var elements = FindElementsInContainer(elementToSearch.Locator, depth++, previousContainerNativeElement);

                if (elements.Count == 0)
                {
                    return elements;
                }
                previousContainer = elements;
            }
            catch (StaleElementReferenceException sere)
            {
                Logger.LogError(sere, $"L{depth:00}>{elementToSearch.Locator} - Is Stale !");
                return SearchResult.Empty;
            }
            Logger.LogInformation($"L{depth:00}>{elementToSearch.Locator}");


            return previousContainer;
        }

        private SearchResult FindElementsInContainer(IElementLocator locator, int depth, SearchResult previousContainerNativeElement)
        {
            if (previousContainerNativeElement == null)
            {   //TODO: Consider adding browser.FindNativeElements and using it here to have parity with IWebElement.FindElements()
                throw new Exception($"previousContainerNativeElement is null. {depth:00}{locator} cannot be searched");
            }
            
            var elements = Browser.Find(locator, previousContainerNativeElement);
            return elements;
        }

        public void DragAndDropTo(IElement target)
        {
            Browser.DragAndDrop(this, target);
        }

        public void DragAndDropAt(int offsetX, int offsetY)
        {
            Browser.DragAndDrop(this, offsetX, offsetY);
        }

        public (int, int) GetCoordinates()
            => (this.Element.Location.X, this.Element.Location.Y);

        public (int, int, int, int) GetRectangle()
            => (this.Element.Location.X, this.Element.Location.Y, this.Element.Size.Width, this.Element.Size.Height);

        internal IWebElement Element
        {
            get
            {
                Wait.Until(() => this.IsAvailable);
                var nativeElement = this.NativeElementSearchResult().Elements.FirstOrDefault() as IWebElement;
                if (nativeElement == null)
                {
                    throw new Exception("SpecDrill: Element Not Found!");
                }

                Browser.ExecuteJavascript(@"arguments[0].style.outline='1px solid red';", nativeElement);

                return nativeElement;
            }
        }

        public IElement? Parent
        {
            get
            {
                return this.parent;
            }
        }

        public IElementLocator Locator
        {
            get
            {
                return this.locator;
            }
        }

        public int Count
        {
            get
            {
                Wait.NoMoreThan(TimeSpan.FromSeconds(5)).Until(() => this.IsAvailable);
                var result = this.NativeElementSearchResult();

                if (result.Elements.Any())
                   Browser.ExecuteJavascript(@"arguments[0].style.outline='1px solid green';", result.Elements.First());

                return result.Count;
            }
        }
    }
}
