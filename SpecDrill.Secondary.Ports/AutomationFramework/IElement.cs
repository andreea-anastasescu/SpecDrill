using SpecDrill.Secondary.Ports.AutomationFramework.Core;
using SpecDrill.Secondary.Ports.AutomationFramework.Model;

namespace SpecDrill.Secondary.Ports.AutomationFramework
{
    public interface IElement
    {
        /// <summary>
        /// Native Element
        /// </summary>
        SearchResult NativeElementSearchResult();

        /// <summary>
        /// Counts Occurences of element as described by locator (Ignores locator's Index property meaning it doesn't return 1 if index is specified)
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Checks if the element is in a read-only mode (read-only or disabled)
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// Checks if the element is drawn on screen and ready for interaction (shown and enabled)
        /// </summary>
        bool IsAvailable { get; }
        bool IsEnabled { get; }
        bool IsDisplayed { get; }

        /// <summary>
        /// Underlying Browser object responsible with browser interaction
        /// </summary>
        IBrowser Browser { get; }

        void Click(bool waitForSilence = false);
        void DoubleClick(bool waitForSilence = false);

        /// <summary>
        /// Sends text to input
        /// </summary>
        /// <param name="keys">Text to send into textbox</param>

        IElement SendKeys(string keys, bool waitForSilence = false);

        /// <summary>
        /// Lose element focus
        /// </summary>
        /// <returns></returns>
        void Blur(bool waitForSilence = false);

        /// <summary>
        /// Clears input
        /// </summary>
        /// <returns></returns>
        IElement Clear(bool waitForSilence = false);

        string Text { get; }

        /// <summary>
        /// Gets html element attribute value
        /// </summary>
        /// <param name="attributeName">Element atribute's name</param>
        /// <returns>attribute value, or null if attribute not present</returns>
        string GetAttribute(string attributeName);
        bool SetAttribute(string attributeName, string attributeValue);
        string GetCssValue(string cssValueName);

        void Hover(bool waitForSilence = false);

        void DragAndDropTo(IElement target);

        void DragAndDropAt(int offsetX, int offsetY);
        (int, int) GetCoordinates();
        (int, int, int, int) GetRectangle();
        IElement? Parent { get; }

        IElementLocator Locator { get; }

        IPage? ContainingPage { get; }
    }
}