using SpecDrill.Secondary.Ports.AutomationFramework.Model;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;

namespace SpecDrill.Secondary.Ports.AutomationFramework.Core
{
    public interface IBrowser
    {
        T Open<T>(string? url = null)
            where T : class, IPage;
        
        WebPage Open(Type pageType, string? url = null);

        T CreatePage<T>()
            where T : class, INavigationTargetElement;
        WebPage CreatePage(Type pageType);
        T CreateControl<T>(IElement? parent, IElementLocator elementLocator)
            where T : class, IElement;
        T CreateTarget<T>(IElement? parent = null, IElementLocator? elementLocator = null)
            where T : class, INavigationTargetElement;

        void GoToUrl(string url);

        string PageTitle { get; }

        Dictionary<string, object> GetCapabilities();

        /// <summary>
        /// returns an IDisposable that changes browser driver's timeout and restores it to previous value at end of using scope/when disposed
        /// </summary>
        /// <param name="implicitTimeout"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        IDisposable ImplicitTimeout(TimeSpan implicitTimeout, string? message = null);

        /// <summary>
        /// Switches to specified frame.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="seleniumFrameElement"></param>
        void SwitchToFrame<T>(IFrameElement<T> seleniumFrameElement) where T : class, IPage;

        void SwitchToWindow<T>(IWindowElement<T> seleniumWindowElement) where T : class, IPage;

        void ScrollIntoView(IElement element);
        double? ScrollDivHorizontally(IElement divElement, int deltaPixels);
        double? ScrollDivVertically(IElement divElement, int deltaPixels);
        /// <summary>
        /// Returns instance of element if present or null if not available.
        /// </summary>
        /// <param name="locator"></param>
        /// <returns></returns>
        SearchResult PeekElement(IElement element);
        
        void Exit();

        SearchResult Find(IElementLocator locator, SearchResult? searchRoot = null);
        void JsLog(string logEntry);
        object? ExecuteJavascript(string script, params object[] arguments);

        void Hover(IElement element);

        [Obsolete("This method is deprecated. Use IElement.DragAndDropTo(IElement) or IElement.DragAndDropAt(int,int) instead!")]
        void DragAndDropElement(IElement startFromElement, IElement stopToElement);
        void DragAndDrop(IElement startFromElement, IElement stopToElement);

        void DragAndDrop(IElement startFromElement, int offsetX, int offsetY);
        void ClickAndDrag(IElement from, IElement to, TimeSpan? duration = null);
        bool IsJQueryDefined { get; }

        //bool LoadJQuery();

        void MaximizePage();
        void SetWindowSize(int initialWidth, int initialHeight);

        void RefreshPage();

        void Click(IElement element);
        void ClickJs(IElement element);
        void DoubleClick(IElement element);
        void DoubleClickJs(IElement element);

        IBrowserAlert Alert { get; }

        bool IsAlertPresent { get; }
        Uri Url { get; }

        void SwitchToDocument();
        void CloseLastWindow();
        string GetPdfText();
        void SaveScreenshot(string testClassName, string testMethodName);

        void AddCookie(Model.Cookie cookie);
        IEnumerable<Model.Cookie> AllCookies { get; }
        void DeleteAllCookies();
        void DeleteCookie(Model.Cookie cookie);
        void GetCookieByName(string name);
        void DeleteCookieByName(string name);
    }
}