using SpecDrill.SecondaryPorts.AutomationFramework;
using SpecDrill.SecondaryPorts.AutomationFramework.Core;
using SpecDrill.SecondaryPorts.AutomationFramework.Model;
using System;

namespace SpecDrill
{
    public class ElementBase : IElement
    {
        protected IElementLocator locator;
        protected IBrowser? browser => SpecDrill.Browser.Instance;
        protected IElement? parent;
        protected IElement rootElement;
        public ElementBase(IElement? parent, IElementLocator locator)
        {

            this.parent = parent;
            this.locator = locator;
            this.rootElement = WebElement.Create(parent, locator);
        }

        #region IElement

        public SearchResult NativeElementSearchResult
        {
            get
            {
                return this.rootElement.NativeElementSearchResult;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return this.rootElement.IsReadOnly;
            }
        }

        public bool IsAvailable
        {
            get
            {
                return this.rootElement.IsAvailable;
            }
        }

        public bool IsEnabled
        {
            get
            {
                return this.rootElement.IsEnabled;
            }
        }

        public bool IsDisplayed
        {
            get
            {
                return this.rootElement.IsDisplayed;
            }
        }

        public IBrowser Browser
        {
            get
            {
                return this.browser ?? throw new Exception("Browser could not be instantiated!");
            }
        }

        public virtual string Text
        {
            get
            {
                return this.rootElement.Text;
            }
        }

        public IElement? Parent
        {
            get
            {
                return this.parent;
            }
        }

        public IElementLocator Locator => this.locator;

        public IPage? ContainingPage => this.rootElement.ContainingPage;

        public int Count
        {
            get
            {
                return this.rootElement.Count;
            }
        }

        public void DoubleClick(bool waitForSilence = false) => this.rootElement.DoubleClick(waitForSilence);

        public void Click(bool waitForSilence = false) => this.rootElement.Click(waitForSilence);

        public IElement SendKeys(string keys, bool waitForSilence = false)
        {
            return this.rootElement.SendKeys(keys, waitForSilence);
        }

        public void Blur(bool waitForSilence = false)
        {
            this.rootElement.Blur(waitForSilence);

        }

        public IElement Clear(bool waitForSilence = false)
        {
            return this.rootElement.Clear(waitForSilence);
        }

        public string GetAttribute(string attributeName)
        {
            return this.rootElement.GetAttribute(attributeName);
        }

        public string GetCssValue(string cssValueName)
        {
            return this.rootElement.GetCssValue(cssValueName);
        }

        public void Hover(bool waitForSilence = false)
        {
            this.rootElement.Hover(waitForSilence);
        }

        public void DragAndDropTo(IElement target)
        {
            this.rootElement.DragAndDropTo(target);
        }

        public void DragAndDropAt(int offsetX, int offsetY)
        {
            this.rootElement.DragAndDropAt(offsetX, offsetY);
        }
        #endregion
    }
}
