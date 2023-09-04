using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V114.Memory;
using OpenQA.Selenium.Interactions;
using SpecDrill.Configuration;
using SpecDrill.Infrastructure;
using SpecDrill.Infrastructure.Enums;
using SpecDrill.Secondary.Adapters.WebDriver.Extensions;
using SpecDrill.Secondary.Ports.AutomationFramework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace SpecDrill.Secondary.Adapters.WebDriver
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
        private readonly IWebDriver seleniumDriver;

        private readonly ILogger Logger = DI.GetLogger<SeleniumBrowserDriver>();

        private readonly Settings? configuration;

        public SeleniumBrowserDriver(IWebDriver seleniumDriver, Settings? configuration)
        {
            this.seleniumDriver = seleniumDriver;
            this.configuration = configuration;
        }

        public static IBrowserDriver Create(IWebDriver seleniumDriver, Settings? configuration)
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

        private Func<IAlert?> WdAlert => () =>
        {
            try
            {
                return this.seleniumDriver.SwitchTo().Alert();
            }
            catch (NoAlertPresentException)
            { }

            return null;
        };

        public IBrowserAlert? Alert
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
            var timeouts = this.seleniumDriver.Manage().Timeouts();
            timeouts.ImplicitWait = timeout;
            timeouts.AsynchronousJavaScript = timeout;
            timeouts.PageLoad = timeout;
        }
        
        public void JsLog(string logEntry)
            => (seleniumDriver as IJavaScriptExecutor)!.ExecuteScript($"console.log('{logEntry.Replace("'", "\"").Replace("\r", "").Replace("\n", "")}')");
        public object? ExecuteJavaScript(string js, params object[] arguments)
        {
            var javaScriptExecutor = (seleniumDriver as IJavaScriptExecutor);

            if (javaScriptExecutor == null)
            {
                Logger.LogError($" {nameof(seleniumDriver)} is not of type {nameof(IJavaScriptExecutor)}");
                return null;
            }

            try
            {
                return javaScriptExecutor.ExecuteScript(js, arguments);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error when executing JavaScript");
                JsLog(e.Message);
                return null;
            }
        }

        public void MoveToElement(IElement element)
        {
            var actions = new Actions(this.seleniumDriver);
            actions.MoveToElement(element.NativeElementSearchResult().Elements.FirstOrDefault()?.NativeElement as IWebElement);
            actions.Build().Perform();
        }

        public void Click(IElement element)
        {
            element.Click();
        }

        public void DoubleClick(IElement element)
        {
            var mode = configuration?.WebDriver?.Mode.ToEnum<Modes>();
            var browserName = configuration?.WebDriver?.Browser?.BrowserName.ToEnum<BrowserNames>();

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

        public void DoubleClickJs(IElement element)
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
        public void ClickJs(IElement element)
        {
            var jsClick =
                @"
                 if (element.getAttribute('style') == null) {
                 var sa = document.createAttribute('style');
                 sa.value = 'pointer-events: auto;'; 
                element.setAtributeNode(sa);
console.log('added style attribute with pointer events');
                } else {
                 var sv = element.getAttribute('style');
                 element.setAttribute('style', sv + ';' + 'pointer-events: auto;');
                console.log('added pointer events style to style attribute');
                }
console.log('mouse click!');
                 var event = new MouseEvent('click', {
                            'view': window,
                            'bubbles': true,
                            'cancelable': false,
                            'button': 0,
                            'buttons': 0
                          });
          
                  arguments[0].dispatchEvent(event);
                  setTimeout(() => { 
                        var enterDown = new KeyboardEvent('keydown', {
                                'cancelable': false,
                                'key': 'Enter'
                              });
                      arguments[0].dispatchEvent(enterDown);
                      var enterUp = new KeyboardEvent('keyup', {
                            'cancelable': false,
                            'key': 'Enter'
                              });
                      arguments[0].dispatchEvent(enterUp);  
                  }, 2000);
                  
                ";
            ////var jsClick = @"arguments[0].click()";
            //if (element.GetCssValue("pointer-events").Contains("none"))
            //{
            //    var style = element.GetAttribute("style");
            //    element.SetAttribute("style", $"{style};pointer-events: auto;");
            //}
            //element.Click();
            JsLog($"clicking button {element.Locator}");
            this.ExecuteJavaScript(jsClick, element.ToWebElement());
            //this.ExecuteJavaScript("document.getElementById('SignIn').click();");
            //Actions action = new Actions(seleniumDriver);
            //// action.MoveToElement(element.ToWebElement()).Click().Perform();
            //try { action.SendKeys(element.ToWebElement(), Keys.Enter).Perform(); } catch { }
            // action.SendKeys(Keys.Tab).Perform();
            // action.SendKeys(Keys.Enter).Perform();
            
        }

        public void ScrollIntoView(IElement element)
        {
            this.ExecuteJavaScript($"arguments[0].scrollIntoViewIfNeeded();", element.ToWebElement());
        }

        public double? ScrollDivVertically(IElement divElement, int deltaPixels)
        {
            var result = this.ExecuteJavaScript($"arguments[0].scrollTop += {deltaPixels}; return arguments[0].scrollTop;", divElement.ToWebElement());
            if (result == null)
                return null;
            try
            {
                return (double)result;
            } 
            catch
            {
                return (long)result;
            }
        }

        public double? ScrollDivHorizontally(IElement divElement, int deltaPixels)
        {
            var result = this.ExecuteJavaScript($"arguments[0].scrollLeft += {deltaPixels}; return arguments[0].scrollLeft;", divElement.ToWebElement());
            if (result == null)
                return null;

            try
            {
                return (double)result;
            }
            catch
            {
                return (long)result;
            }
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
            var sfe = (seleniumFrameElement as SeleniumElement);
            if (sfe == null) return;
            seleniumDriver.SwitchTo().Frame(sfe.Element);
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
            if (mostRecentWindow != default)
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
            StringBuilder pdfText = new();
            string userAgent = (ExecuteJavaScript("return navigator.userAgent") as string) ?? string.Empty;

            using (var webClient = new WebClient())
            {
                var formattedCookiesString = GetFormattedCookiesString();

                Uri uri = new Uri(seleniumDriver.Url);
                webClient.Headers.Add(HttpRequestHeader.Host, uri.Host);
                webClient.Headers.Add(HttpRequestHeader.UserAgent, userAgent);
                webClient.Headers.Add(HttpRequestHeader.Cookie, formattedCookiesString);
                webClient.Headers.Add(HttpRequestHeader.Accept, "application/pdf");

                using var pdfStream = new MemoryStream(webClient.DownloadData(uri.OriginalString));
                using var pdfDoc = new PdfDocument(new PdfReader(pdfStream));

                StringBuilder allPdfText = new();
                foreach (var page in pdfDoc.Pages())
                {
                    allPdfText.AppendLine(page.GetText());
                }

                return allPdfText.ToString();
            }
        }

        private string GetFormattedCookiesString()
        {
            StringBuilder strCookies = new();
            foreach (var cookie in (seleniumDriver.Manage().Cookies.AllCookies ?? new ReadOnlyCollection<OpenQA.Selenium.Cookie>(new List<OpenQA.Selenium.Cookie>())))
            {
                strCookies.Append($"{cookie.Name}={cookie.Value}; ");
            }
            if (strCookies.Length > 1) { strCookies.Remove(strCookies.Length - 2, 2); }

            return strCookies.ToString();
        }

        //TODO: return bool so test can assert!
        public void SaveScreenshot(string fileName)
        {
            var attemptNo = 0;
            bool succeeded = false;
            do
            {
                succeeded = this.SaveScreeshotInternal(fileName);
                attemptNo++;
                if (succeeded)
                {
                    Logger.LogInformation($"Saved Screenshot `{fileName}`");
                }
                else
                {
                    Logger.LogError($"Saving Screenshot `{fileName}`. Attempt #{attemptNo} failed!");
                }
            } while (!succeeded && attemptNo < 3);
            if (!succeeded)
            {
                Logger.LogError($"Make sure the configured folder specified in `webdriver.screenshotsPath` exists!");
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
                Logger.LogError(string.Format("Error when saving screenshot `{0}`", fileName), e);
            }
            return false;
        }

        public Dictionary<string, object> GetCapabilities()
        {
            void TryCopyCapability(ICapabilities? from, IDictionary<string, object> to, string key)
            {
                if (from?.HasCapability(key) is object)
                {
                    to[key] = from.GetCapability(key);
                }
            }
            var capabilities = new Dictionary<string, object>();
            var configCapabilities = configuration?.WebDriver?.Browser?.Capabilities ?? new Dictionary<string, object>();

            foreach (var kvp in configCapabilities)
                capabilities[kvp.Key] = kvp.Value.ToString()??"";

            var remoteDriver = (this.seleniumDriver as OpenQA.Selenium.Remote.RemoteWebDriver);
            if (remoteDriver != null)
            {
                TryCopyCapability(remoteDriver?.Capabilities, to: capabilities, "browserName");
                TryCopyCapability(remoteDriver?.Capabilities, to: capabilities, "platformName");
                TryCopyCapability(remoteDriver?.Capabilities, to: capabilities, "browserVersion");
            }

            return capabilities;
        }

        public void ClickAndDrag(IElement from, IElement to, TimeSpan? duration = null)
        {
            if (duration is null)
            {
                duration = TimeSpan.FromMilliseconds(50);
            }
            Wait.Until(() => from.IsAvailable && from.IsVisible);
            Wait.Until(() => to.IsAvailable && to.IsVisible);
            var fromCoords = from.GetRectangle();
            var (fromX, fromY) = (fromCoords.Item1 + fromCoords.Item3 / 2, fromCoords.Item2 + fromCoords.Item4 / 2);
            var toCoords = to.GetRectangle();
            var (toX, toY) = (toCoords.Item1 + toCoords.Item3 / 2, toCoords.Item2 + toCoords.Item4 / 2);
            var builder = new Actions(this.seleniumDriver);
            builder.MoveToElement(from.ToWebElement());
            builder.ClickAndHold();

            for (int i = 0; i< 12; i++) // emulate mouse movement :)
            {
                
                builder.MoveByOffset(1,1);
            }


            builder.MoveToElement(to.ToWebElement());
            builder.Release(to.ToWebElement());
            builder.Perform();
        }

        /// <summary>
        /// Finds elements for given locator. If searchRoot is provided, search is performed 
        /// only on the DOM subtree that has it as root.
        /// </summary>
        /// <param name="locator"></param>
        /// <param name="searchRoot">it's basically a IWebElement</param>
        /// <returns></returns>
        public ReadOnlyCollection<ISearchable> FindElements(IElementLocator locator, ISearchable? searchRoot = null)
        {
            var result = new List<ISearchable>();
            var searchRootElement = searchRoot as ISearchContext;
            var seleniumLocator = locator.ToSelenium();

            ISearchable ToSearchable(IWebElement webElement) => SeleniumSearchable.Create(this, webElement);
            var elements = searchRootElement == null ?
                seleniumDriver.FindElements(seleniumLocator).Select(ToSearchable) :
                searchRootElement.FindElements(seleniumLocator).Select(ToSearchable);

            result.AddRange(elements);
            return new ReadOnlyCollection<ISearchable>(result);
        }
        #region ISearchable
        public ReadOnlyCollection<ISearchable> FindElements(IElementLocator locator)
            => FindElements(locator, null);

        public bool IsShadowRoot()
            => false;

        public ISearchable GetShadowRoot()
            => throw new Exception("Element `<html/>` is not a ShadowRoot!");


        public object NativeElement => FindElements(SeleniumElementLocator.Create(Ports.AutomationFramework.By.TagName, "html")).First();
        #endregion
    }
}
