using Microsoft.Extensions.Logging;
using SpecDrill.Infrastructure;
using SpecDrill.Secondary.Ports.AutomationFramework;
using SpecDrill.Secondary.Ports.AutomationFramework.Model;
using System;
using System.Text.RegularExpressions;

namespace SpecDrill
{
    public class WebPage : Element, IPage
    {
        protected ILogger Logger = DI.GetLogger<WebPage>();
        private string titlePattern;

        public WebPage() : this(string.Empty) { }
        public WebPage(string titlePattern) : base(null, ElementLocatorFactory.Create(By.TagName, "html"))
        {
            this.titlePattern = titlePattern;
        }

        public string Title
        {
            get
            {
                string retrievedTitle = "";
                try
                {
                    using (Browser.ImplicitTimeout(TimeSpan.FromSeconds(3)))
                        retrievedTitle = (this.Browser.ExecuteJavascript("return document.title;") as string) ?? "";
                }
                catch (Exception e)
                {
                    Logger.LogError("Cannot read page Title! {0}", e);
                }
                return retrievedTitle;
            }
        }


        //public IElement Element
        //{
        //    get { return rootElement; }
        //}

        #region IPage
        public virtual bool IsLoaded
        {
            get
            {
                //    object result =
                //    this.Browser.ExecuteJavascript(@"
                //        if (document.readyState !== 'complete') {
                //            return false;

                //            if ((document.jQuery) && (document.jQuery.active || (document.jQuery.ajax && document.jQuery.ajax.active))) {
                //                return false;
                //            } 

                //if (document.angular) {
                //                if (!window.specDrill) {
                //                    window.specDrill = { silence : false };
                //                }
                //                var injector = window.angular.element('body').injector();
                //                var $rootScope = injector.get('$rootScope');
                //                var $http = injector.get('$http');
                //                var $timeout = injector.get('$timeout');

                //                if ($rootScope.$$phase === '$apply' || $rootScope.$$phase === '$digest' || $http.pendingRequests.length != 0) {
                //                    window.specDrill.silence = false;
                //                    return false;
                //                }

                //                if (!window.specDrill.silence) {
                //                    $timeout(function () { window.specDrill.silence = true; }, 0);
                //                    return false;
                //                }
                //            }
                //        }

                //        return true;
                //    ");

                if (titlePattern == null) return true;

                var title = this.Title;
                var isLoaded = title != null &&
                               Regex.IsMatch(title, this.titlePattern);

                Logger.LogInformation("LoadCompleted = {0}, retrievedTitle = {1}, patternToMatch = {2}", isLoaded, title ?? "(null)",
                    this.titlePattern);

                return isLoaded;
            }
        }

        public PageContextTypes ContextType { get; set; }

        public void RefreshPage()
        {
            Browser.RefreshPage();
            Wait.Until(() => this.IsLoaded);
            this.WaitForSilence();
        }

        public virtual void WaitForSilence()
        {
        }

        public void Dispose()
        {
            if (this.ContextType == PageContextTypes.Frame)
            {
                Browser.SwitchToDocument();
            }
            else if (this.ContextType == PageContextTypes.Window)
            {
                Browser.CloseLastWindow();
            }


        }

        // TODO: Investigate how virtual IsPageLoaded can be used to sum up all kinds of wait (static, jQuery, Angular1, Angular2, etc)
        // goal is to have an immediately returning test so we can wait on it
        // currently there is no IsPageLoaded method on IPage so we can use in lambda
        #endregion
    }
}
