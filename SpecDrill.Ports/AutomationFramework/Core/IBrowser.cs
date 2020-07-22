using SpecDrill.SecondaryPorts.AutomationFramework.Model;
using System;
using System.Collections.Generic;

namespace SpecDrill.SecondaryPorts.AutomationFramework.Core
{
    public interface IBrowser
    {
        T Open<T>()
            where T : class, IPage;

        T CreatePage<T>()
            where T : class, IPage;
        T CreateControl<T>(T? fromInstance)
            where T : class, IElement;

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

        /// <summary>
        /// Returns instance of element if present or null if not available.
        /// </summary>
        /// <param name="locator"></param>
        /// <returns></returns>
        SearchResult PeekElement(IElement element);

        void Exit();

        //IElement FindElement(IElementLocator locator);

        //IList<IElement> FindElements(IElementLocator locator);

        SearchResult FindNativeElement(IElementLocator locator);

        object? ExecuteJavascript(string script, params object[] arguments);

        void Hover(IElement element);

        [Obsolete("This method is deprecated. Use IElement.DragAndDropTo(IElement) or IElement.DragAndDropAt(int,int) instead!")]
        void DragAndDropElement(IElement startFromElement, IElement stopToElement);
        void DragAndDrop(IElement startFromElement, IElement stopToElement);

        void DragAndDrop(IElement startFromElement, int offsetX, int offsetY);

        bool IsJQueryDefined { get; }

        //bool LoadJQuery();

        void MaximizePage();

        void RefreshPage();

        void Click(IElement element);
        void DoubleClick(IElement element);

        IBrowserAlert Alert { get; }

        bool IsAlertPresent { get; }
        Uri Url { get; }

        void SwitchToDocument();
        void CloseLastWindow();
        string GetPdfText();
        void SaveScreenshot(string testClassName, string testMethodName);
    }
}