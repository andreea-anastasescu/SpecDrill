using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using SpecDrill.AutomationScopes;
using SpecDrill.Infrastructure.Logging;
using SpecDrill.Infrastructure.Logging.Interfaces;
using SpecDrill.SecondaryPorts.AutomationFramework;
using SpecDrill.SecondaryPorts.AutomationFramework.Core;
using SpecDrill.SecondaryPorts.AutomationFramework.Model;

namespace SpecDrill
{
    public class WebPage : ElementBase, IPage
    {
        protected ILogger Log = Infrastructure.Logging.Log.Get<WebPage>();
        private string titlePattern;

        public WebPage() : this(string.Empty) {  }
        public WebPage(string titlePattern) : base(null, ElementLocator.Create(By.TagName, "html"))
        {
            this.titlePattern = titlePattern;
        }

        public string Title
        {
            get
            {
                string retrievedTitle = null;
                try
                {
                    using (this.Browser.ImplicitTimeout(TimeSpan.FromSeconds(3)))
                        retrievedTitle = this.Browser.ExecuteJavascript("return document.title;") as string;
                }
                catch (Exception e)
                {
                    Log.Error("Cannot read page Title!", e);
                }
                return retrievedTitle;
                //return this.Browser.PageTitle;
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
                var title = this.Title;
                var isLoaded = title != null &&
                               Regex.IsMatch(this.Title, this.titlePattern);

                Log.Info("LoadCompleted = {0}, retrievedTitle = {1}, patternToMatch = {2}", isLoaded, title ?? "(null)",
                    this.titlePattern ?? "(null)");



                return isLoaded;

            }
        }

        public PageContextTypes ContextType { get; set; }

        public void RefreshPage()
        {
            browser.RefreshPage();
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
                browser.SwitchToDocument();
            }
            else if (this.ContextType == PageContextTypes.Window)
            {
                browser.CloseLastWindow();
            }


        }

        // TODO: Investigate how virtual IsPageLoaded can be used to sum up all kinds of wait (static, jQuery, Angular1, Angular2, etc)
        // goal is to have an immediately returning test so we can wait on it
        // currently there is no IsPageLoaded method on IPage so we can use in lambda
        #endregion
    }
}
