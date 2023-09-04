using SpecDrill.Secondary.Ports.AutomationFramework;
using SpecDrill.Secondary.Ports.AutomationFramework.Core;
using SpecDrill.Secondary.Ports.AutomationFramework.Model;
using System;
using static System.Net.Mime.MediaTypeNames;

namespace SpecDrill
{
    public class Element : IElement
    {
        protected IElementLocator locator;
        protected IBrowser? browser => SpecDrill.Browser.Instance;
        protected IElement? parent;
        protected IElement rootElement;
        public Element(IElement? parent, IElementLocator locator)
        {

            this.parent = parent;
            this.locator = locator;
            
            this.rootElement = ElementFactory.Factory.Create(parent, locator);
        }

        #region IElement

        public SearchResult NativeElementSearchResult()
            => this.rootElement.NativeElementSearchResult();

        public bool IsReadOnly
        {
            get
            {
                bool isReadOnly = this.rootElement.IsReadOnly;
                Browser.JsLog($"SpecDrill IsReadOnly ~> {this.Locator} = '{isReadOnly}'");
                return isReadOnly;
            }
        }

        public bool IsAvailable
        {
            get
            {
                bool isAvailable = this.rootElement.IsAvailable;
                Browser.JsLog($"SpecDrill IsAvailable ~> {this.Locator} = '{isAvailable}'");
                return isAvailable;
            }
        }

        public bool IsEnabled
        {
            get
            {
                bool isEnabled = this.rootElement.IsEnabled;
                Browser.JsLog($"SpecDrill IsEnabled ~> {this.Locator} = '{isEnabled}'");
                return isEnabled;
            }
        }

        public bool IsDisplayed
        {
            get
            {
                bool isDisplayed = this.rootElement.IsDisplayed;
                Browser.JsLog($"SpecDrill IsDisplayed ~> {this.Locator} = '{isDisplayed}'");
                return isDisplayed;
            }
        }

        public bool IsVisible
        {
            get
            {
                bool isVisible = this.rootElement.IsVisible;
                Browser.JsLog($"SpecDrill IsVisible ~> {this.Locator} = '{isVisible}'");
                return isVisible;
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
                var text = this.rootElement.Text;
                Browser.JsLog($"SpecDrill Text ~> {this.Locator} = '{text}'");
                return text;
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

        public void DoubleClick(bool waitForSilence = false)
        {
            Browser.JsLog($"SpecDrill DoubleClick (waitForSilence={waitForSilence}) ~> {this.Locator}");
            this.rootElement.DoubleClick(waitForSilence);
        }

        public void Click(bool waitForSilence = false)
        {
            Browser.JsLog($"SpecDrill Click (waitForSilence={waitForSilence}) ~> {this.Locator}");
            this.rootElement.Click(waitForSilence);
        }

        public IElement SendKeys(string keys, bool waitForSilence = false)
        {
            Browser.JsLog($"SpecDrill SendKeys (keys={keys}, waitForSilence={waitForSilence}) ~> {this.Locator}");
            return this.rootElement.SendKeys(keys, waitForSilence);
        }

        public void Blur(bool waitForSilence = false)
        {
            Browser.JsLog($"SpecDrill Blur (waitForSilence={waitForSilence}) ~> {this.Locator}");
            this.rootElement.Blur(waitForSilence);

        }

        public IElement Clear(bool waitForSilence = false)
        {
            Browser.JsLog($"SpecDrill Clear (waitForSilence={waitForSilence}) ~> {this.Locator}");
            return this.rootElement.Clear(waitForSilence);
        }

        public string GetAttribute(string attributeName)
        {
            var attributeValue = this.rootElement.GetAttribute(attributeName);
            Browser.JsLog($"SpecDrill GetAttribute (attributeName={attributeName}) ~> {this.Locator} = `{attributeValue}`");
            return attributeValue;
        }
        public bool SetAttribute(string attributeName, string attributeValue)
        {
            Browser.JsLog($"SpecDrill SetAttribute (attributeName={attributeName}, attributeValue={attributeValue}) ~> {this.Locator}");
            return this.rootElement.SetAttribute(attributeName, attributeValue);
        }
        public string GetCssValue(string cssValueName)
        {
            var cssValue = this.rootElement.GetCssValue(cssValueName);
            Browser.JsLog($"SpecDrill GetCssValue (cssValueName={cssValueName}) ~> {this.Locator} = `{cssValue}`");
            return cssValue;
        }

        public void Hover(bool waitForSilence = false)
        {
            Browser.JsLog($"SpecDrill Hover (waitForSilence={waitForSilence}) ~> {this.Locator}");
            this.rootElement.Hover(waitForSilence);
        }

        public void DragAndDropTo(IElement target)
        {
            Browser.JsLog($"SpecDrill DragAndDropTo (target={target.Locator}) ~> {this.Locator}");
            this.rootElement.DragAndDropTo(target);
            //Browser.DragAndDrop(this.rootElement, target);
        }
        public void ClickAndDragTo(IElement target)
        { 
            Browser.JsLog($"SpecDrill ClickAndDragTo (target={target.Locator}) ~> {this.Locator}");
            this.rootElement.ClickAndDragTo(target);
            //Browser.ClickAndDrag(this.rootElement, target);
        }
        public void DragAndDropAt(int offsetX, int offsetY)
        {
            Browser.JsLog($"SpecDrill DragAndDropAt (offsetX={offsetX}, offsetY={offsetY}) ~> {this.Locator}");
            Browser.DragAndDrop(this.rootElement, offsetX, offsetY);
        }

        public (int, int) GetCoordinates()
            => this.rootElement.GetCoordinates();

        public (int, int, int, int) GetRectangle()
            => this.rootElement.GetRectangle();
        #endregion
    }
}
