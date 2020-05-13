using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using SpecDrill.Adapters.WebDriver.Extensions;
using SpecDrill.Infrastructure.Logging;
using SpecDrill.SecondaryPorts.AutomationFramework;
using SpecDrill.Infrastructure.Logging.Interfaces;
using System.Net;
using System.Text;
using System.Net.Http;
using System.IO;
using System.Security.Cryptography;
using SpecDrill.Configuration;
using SpecDrill.Infrastructure.Enums;
using SpecDrill.Infrastructure;

namespace SpecDrill.Adapters.WebDriver
{
    internal class SeleniumBrowserDriver : IBrowserDriver
    {
        #region Scripts
        private static readonly string simulateHtml5DragAndDropScript = @"
        (function(source, target){
            var dnd = { simulateEvent: function(elem, options) {
                                /*Simulating drag start*/
                                var type = 'dragstart';
                                var event = this.createEvent(type);
                                this.dispatchEvent(elem, type, event);

                                /*Simulating drop*/
                                type = 'drop';
                                var dropEvent = this.createEvent(type, {});
                                dropEvent.dataTransfer = event.dataTransfer;
                                this.dispatchEvent(options.dropTarget, type, dropEvent);

                                /*Simulating drag end*/
                                type = 'dragend';
                                var dragEndEvent = this.createEvent(type, {});
                                dragEndEvent.dataTransfer = event.dataTransfer;
                                this.dispatchEvent(elem, type, dragEndEvent);
                        },
                        createEvent: function(type) {
                                var event = document.createEvent('CustomEvent');
                                event.initCustomEvent(type, true, true, null);
                                event.dataTransfer = {
                                                        data: { },
                                                        setData: function(type, val){ this.data[type] = val; },
                                                        getData: function(type){ return this.data[type]; }
                                };
                                return event;
                        },
                        dispatchEvent: function(elem, type, event) {
                                if (elem.dispatchEvent) { elem.dispatchEvent(event); }
                                else if(elem.fireEvent ) { elem.fireEvent('on'+type, event); }
                        }
            };
            dnd.simulateEvent(source, { dropTarget : target });
        })(arguments[0], arguments[1]);";

        #endregion
        private IWebDriver seleniumDriver = null;

        private readonly ILogger Log = Infrastructure.Logging.Log.Get<SeleniumBrowserDriver>();

        private readonly Settings configuration;

        public SeleniumBrowserDriver(IWebDriver seleniumDriver, Settings configuration)
        {
            this.seleniumDriver = seleniumDriver;
            this.configuration = configuration;
        }

        public static IBrowserDriver Create(IWebDriver seleniumDriver, Settings configuration)
        {
            return new SeleniumBrowserDriver(seleniumDriver, configuration);
        }

        public void GoToUrl(string url)
        {
            seleniumDriver.Navigate().GoToUrl(url);
        }

        public void Exit()
        {
            seleniumDriver.Quit();
        }

        public string Title
        {
            get { return seleniumDriver.Title; }
        }

        private Func<IAlert> WdAlert => () =>
        {
            try
            {
                return this.seleniumDriver.SwitchTo().Alert();
            }
            catch (NoAlertPresentException)
            { }

            return null;
        };

        public IBrowserAlert Alert
        {
            get
            {
                if (this.WdAlert() == null)
                    return null;

                return new SeleniumAlert(WdAlert);
            }
        }

        public bool IsAlertPresent => this.WdAlert != null;

        public Uri Url => new Uri(this.seleniumDriver.Url);

        public void ChangeBrowserDriverTimeout(TimeSpan timeout)
        {
            this.seleniumDriver.Manage().Timeouts().ImplicitWait = timeout;
        }

        public ReadOnlyCollection<object> FindElements(IElementLocator locator)
        {
            var result = new List<object>();
            var elements = seleniumDriver.FindElements(locator.ToSelenium());
            result.AddRange(elements);
            return new ReadOnlyCollection<object>(result);
        }

        //public object FindElement(IElementLocator locator)
        //{
        //    var elements = seleniumDriver.FindElements(locator.ToSelenium());
        //    if (elements == null || elements.Count == 0)
        //        return null;
        //    return elements[0];
        //}

        public object ExecuteJavaScript(string js, params object[] arguments)
        {
            var javaScriptExecutor = (seleniumDriver as IJavaScriptExecutor);

            if (javaScriptExecutor == null)
            {
                Log.Error($" {nameof(seleniumDriver)} is not of type {nameof(IJavaScriptExecutor)}");
                return false;
            }

            try
            {
                return javaScriptExecutor.ExecuteScript(js, arguments);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error when executing JavaScript");
                return false;
            }
        }

        public void MoveToElement(IElement element)
        {
            var actions = new Actions(this.seleniumDriver);
            actions.MoveToElement((element as SeleniumElement).Element);
            actions.Build().Perform();
        }

        public void Click(IElement element)
        {
            (element as SeleniumElement).Element.Click();
        }

        public void DoubleClick(IElement element)
        {
            var mode = configuration.WebDriver.Mode.ToEnum<Modes>();
            var browserName = configuration.WebDriver.Browser.BrowserName.ToEnum<BrowserNames>();

            if (mode == Modes.browser && browserName == BrowserNames.firefox)
            {
                this.DoubleClickJs(element);
            }
            else
            {
                var actions = new Actions(this.seleniumDriver);
                actions.DoubleClick(element.ToWebElement());
                actions.Build().Perform();
            }
        }

        private void DoubleClickJs(IElement element)
        {
            var jsDoubleClick = 
                @"var event = new MouseEvent('dblclick', {
                            'view': window,
                            'bubbles': true,
                            'cancelable': true
                          });
                  arguments[0].dispatchEvent(event);";
            this.ExecuteJavaScript(jsDoubleClick, element.ToWebElement());
        }

        public void DragAndDrop(IElement draggable, int offsetX, int offsetY)
        {
            var fromElement = draggable.ToWebElement();

            var builder = new Actions(this.seleniumDriver);
            var size = fromElement.Size;
            
            builder.MoveToElement(fromElement);
            builder.ClickAndHold(fromElement);
            builder.MoveByOffset(offsetX, offsetY);
            builder.Release().Perform();
        }

        public void DragAndDrop(IElement draggable, IElement dropTarget)
        {
            var fromElement = draggable.ToWebElement();
            var toElement = dropTarget.ToWebElement();

            if (string.Compare(fromElement.GetAttribute("draggable") ?? string.Empty, "true", true) == 0)
            {
                ExecuteJavaScript(simulateHtml5DragAndDropScript, fromElement, toElement);
            }
            else
            {
                var builder = new Actions(this.seleniumDriver);
                builder.MoveToElement(fromElement);
                builder.ClickAndHold(fromElement);
                builder.MoveToElement(toElement);
                builder.Release().Perform();
            }
        }

        public void RefreshPage()
        {
            seleniumDriver.Navigate().Refresh();
        }

        public void MaximizePage()
        {
            seleniumDriver.Manage().Window.Maximize();
        }

        public void SwitchToDocument()
        {
            seleniumDriver.SwitchTo().DefaultContent();
        }

        public void SwitchToFrame(IElement seleniumFrameElement)
        {
            seleniumDriver.SwitchTo().Frame((seleniumFrameElement as SeleniumElement).Element);
        }

        public void SetWindowSize(int initialWidth, int initialHeight)
        {
            seleniumDriver.Manage().Window.Size = new System.Drawing.Size(initialWidth, initialHeight);
        }

        void IBrowserDriver.SwitchToWindow<T>(IWindowElement<T> seleniumWindowElement)
        {
            var windowCount = seleniumDriver.WindowHandles.Count();
            Wait.WithRetry().Doing(() => seleniumWindowElement.Click()).Until(() => seleniumDriver.WindowHandles.Count() > windowCount);
            
            var mostRecentWindow = seleniumDriver.WindowHandles.LastOrDefault();
            if (mostRecentWindow != default(string))
            {
                seleniumDriver.SwitchTo().Window(mostRecentWindow);
            }
        }

        public void CloseLastWindow()
        {
            seleniumDriver.Close();
        }
        
        public string GetPdfText()
        {
            //StringBuilder pdfText = new StringBuilder();
            //string userAgent = (string)ExecuteJavaScript("return navigator.userAgent") ?? string.Empty;


            //using (var webClient = new WebClient())
            //{
            //    var formattedCookiesString = GetFormattedCookiesString();

            //    Uri uri = new Uri(seleniumDriver.Url);
            //    webClient.Headers.Add(HttpRequestHeader.Host, uri.Host);
            //    webClient.Headers.Add(HttpRequestHeader.UserAgent, userAgent);
            //    webClient.Headers.Add(HttpRequestHeader.Cookie, formattedCookiesString);
            //    webClient.Headers.Add(HttpRequestHeader.Accept, "application/pdf");

            //    using (var pdfStream = new MemoryStream(webClient.DownloadData(uri.OriginalString)))
            //    using (var extractor = new PdfExtract.Extractor())
            //        return extractor.ExtractToString(pdfStream);
            //}
            return ""; //TODO: Find .netstandard 2.0 compatible pdf extractor
        }

        private string GetFormattedCookiesString()
        {
            StringBuilder strCookies = new StringBuilder();
            foreach (var cookie in (seleniumDriver.Manage().Cookies.AllCookies ?? new ReadOnlyCollection<OpenQA.Selenium.Cookie>(new List<OpenQA.Selenium.Cookie>())))
            {
                strCookies.Append($"{cookie.Name}={cookie.Value}; ");
            }
            if (strCookies.Length > 1) { strCookies.Remove(strCookies.Length - 2, 2); }

            return strCookies.ToString();
        }

        public void SaveScreenshot(string fileName)
        {
            var attemptNo = 0;
            bool succeeded = false;
            do
            {
                succeeded = this.SaveScreeshotInternal(fileName);
                attemptNo++;
                Log.Error($"Saving Screenshot `{fileName}`. Attempt #{attemptNo}");
            } while (!succeeded && attemptNo < 3);
            if (!succeeded)
            {
                Log.Error($"Make sure the configured folder specified in `webdriver.screenshotsPath` exists!");
            }
        }

        private bool SaveScreeshotInternal(string fileName)
        {
            try
            {
                Screenshot screenshot = ((ITakesScreenshot)seleniumDriver).GetScreenshot();
                screenshot.SaveAsFile(fileName);
                return true;
            }
            catch (Exception e)
            {
                Log.Error(string.Format("Error when saving screenshot `{0}`", fileName), e);
            }
            return false;
        }

        public Dictionary<string, object> GetCapabilities()
        {
            void TryCopyCapability(ICapabilities from, IDictionary<string, object> to, string key)
            {
                if (from?.HasCapability(key) is object)
                {
                    to[key] = from.GetCapability(key);
                }
            }
            var capabilities = new Dictionary<string, object>();

            foreach (var kvp in configuration.WebDriver.Browser.Capabilities)
                capabilities[kvp.Key] = kvp.Value;

            var remoteDriver = (this.seleniumDriver as OpenQA.Selenium.Remote.RemoteWebDriver);
            if (remoteDriver != null)
            {
                TryCopyCapability(remoteDriver?.Capabilities, to: capabilities, "browserName");
                TryCopyCapability(remoteDriver?.Capabilities, to: capabilities, "platformName");
                TryCopyCapability(remoteDriver?.Capabilities, to: capabilities, "browserVersion");
            }
            
            return capabilities;
        }
    }
}
