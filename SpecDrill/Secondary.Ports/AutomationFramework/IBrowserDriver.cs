using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;

namespace SpecDrill.Secondary.Ports.AutomationFramework
{
    public interface IBrowserDriver : ISearchable
    {
        /// <summary>
        /// Navigates to specified Url
        /// </summary>
        /// <param name="url"></param>
        void GoToUrl(string url);

        /// <summary>
        /// Closes Browser Window
        /// </summary>
        void Exit();

        /// <summary>
        /// Current Page's Title
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Changes underlying framework's implicit wait timeout
        /// </summary>
        /// <param name="timeout"></param>
        void ChangeBrowserDriverTimeout(System.TimeSpan timeout);
        /// <summary>
        /// Finds elements matching provided locator
        /// </summary>
        /// <param name="locator"></param>
        /// <returns></returns>
        ReadOnlyCollection<ISearchable> FindElements(IElementLocator locator, ISearchable? searchRoot = null);
        /// <summary>
        /// returns native element. Cannot return IElement since we need an IBrowser instance for creation.
        /// </summary>
        /// <param name="locator"></param>
        /// <returns></returns>
        //object FindElement(IElementLocator locator);
        void JsLog(string logEntry);

        object? ExecuteJavaScript(string js, params object[] arguments);

        void MoveToElement(IElement element);

        void DragAndDrop(IElement draggable, IElement dropTarget);

        void DragAndDrop(IElement draggable, int offsetX, int offsetY);

        void ClickAndDrag(IElement from, IElement to, TimeSpan? duration = null);

        void RefreshPage();

        void MaximizePage();

        void Click(IElement element);
        void ClickJs(IElement element);
        void DoubleClick(IElement element);
        void DoubleClickJs(IElement element);

        IBrowserAlert? Alert { get; }

        bool IsAlertPresent { get; }
        Uri Url { get; }

        void SwitchToDocument();

        void SwitchToFrame(IElement seleniumFrameElement);

        void SetWindowSize(int initialWidth, int initialHeight);

        void SwitchToWindow<T>(IWindowElement<T> seleniumWindowElement) where T : IPage;

        void ScrollIntoView(IElement element, bool ifNeeded = true);
        double? ScrollDivVertically(IElement divElement, int deltaPixels);

        double? ScrollDivHorizontally(IElement divElement, int deltaPixels);
        
        void CloseLastWindow();
        string GetPdfText();
        void SaveScreenshot(string fileName);
        Dictionary<string, object> GetCapabilities();

        void AddCookie(Model.Cookie cookie);
        IEnumerable<Model.Cookie> AllCookies { get; }
        void DeleteAllCookies();
        void DeleteCookie(Model.Cookie cookie);
        void GetCookieByName(string name);
        void DeleteCookieByName(string name);
    }
}
