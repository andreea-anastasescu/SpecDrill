using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using SpecDrill.Secondary.Ports.AutomationFramework;
using SpecDrill.Secondary.Ports.AutomationFramework.Core;
using System;
using System.Collections.Generic;

namespace SpecDrill.Secondary.Adapters.WebDriver
{
    internal class SeleniumSelectElement : SeleniumElement, ISelectElement
    {
        public SeleniumSelectElement(IBrowser? browser, IElement? parent, IElementLocator locator) : base(browser, parent, locator)
        {
        }

        private OpenQA.Selenium.Support.UI.SelectElement SelectElement
        {
            get
            {
                if (!(this.Element is IWebElement element))
                    throw new InvalidCastException("cast to IWebElement failed!");

                return new OpenQA.Selenium.Support.UI.SelectElement(element);
            }
        }

        public ISelectElement SelectByText(string optionText)
        {
            this.SelectElement.SelectByText(optionText);

            Logger.LogInformation("SelectByText `{0}` @ {1}", optionText, this.locator);
            return this;
        }

        public ISelectElement SelectByValue(string optionValue)
        {
            this.SelectElement.SelectByValue(optionValue);

            Logger.LogInformation("SelectByValue `{0}` @ {1}", optionValue, this.locator);
            return this;
        }

        public ISelectElement SelectByIndex(int optionIndex)
        {
            this.SelectElement.SelectByIndex(optionIndex);

            Logger.LogInformation("SelectByIndex `{0}` @ {1}", optionIndex, this.locator);
            return this;
        }

        public string GetOptionTextByIndex(int optionIndex)
        {
            //TODO: rely on Browser.Find at some point
            return this.Element.FindElement(OpenQA.Selenium.By.XPath(string.Format("./descendant::option[{0}]", optionIndex))).Text;
        }

        public IEnumerable<string> GetOptionsText()
        {
            //TODO: rely on Browser.Find at some point
            var options = this.Element.FindElements(OpenQA.Selenium.By.XPath("./descendant::option"));
            foreach (var option in options)
            {
                yield return option.Text;
            }
        }

        public string SelectedOptionText
        {
            get { return this.SelectElement.SelectedOption.Text; }
        }

        public string SelectedOptionValue
        {
            get { return this.SelectElement.SelectedOption.GetAttribute("value"); }
        }

        public int OptionsCount
        {
            get { return this.SelectElement.Options.Count; }
        }
    }
}
