using SpecDrill.Secondary.Ports.AutomationFramework;
using SpecDrill.Secondary.Ports.AutomationFramework.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpecDrill
{
    public class SelectElement : Element, ISelectElement
    {
        ISelectElement rootSelectElement;
        public SelectElement(IElement? parent, IElementLocator locator) : base(parent, locator)
        {
            this.rootElement = this.rootSelectElement = ElementFactory.Factory.CreateSelect(parent, locator);
        }

        public string SelectedOptionText => this.rootSelectElement.SelectedOptionText;

        public string SelectedOptionValue => this.rootSelectElement.SelectedOptionValue;

        public int OptionsCount => this.rootSelectElement.OptionsCount;

        public IEnumerable<string> GetOptionsText()
        {
            return this.rootSelectElement.GetOptionsText();
        }

        public string GetOptionTextByIndex(int optionIndex)
        {
            return this.rootSelectElement.GetOptionTextByIndex(optionIndex);
        }

        public ISelectElement SelectByIndex(int optionIndex)
        {
            return this.rootSelectElement.SelectByIndex(optionIndex);
        }

        public ISelectElement SelectByText(string optionText)
        {
            return this.rootSelectElement.SelectByText(optionText);
        }

        public ISelectElement SelectByValue(string optionValue)
        {
            return this.rootSelectElement.SelectByValue(optionValue);
        }
    }
}
