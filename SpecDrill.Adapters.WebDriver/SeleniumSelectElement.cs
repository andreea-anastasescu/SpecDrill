using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SpecDrill.Infrastructure.Logging;
//using SpecDrill.Infrastructure.Logging.Interface;
using SpecDrill.SecondaryPorts.AutomationFramework;
using SpecDrill.SecondaryPorts.AutomationFramework.Core;

namespace SpecDrill.SecondaryPorts.Adapters.WebDriver
{
    internal class SeleniumSelectElement : SeleniumElement, ISelectElement
    {
        //protected static ILogger Log = Infrastructure.Logging.Log.Get<SeleniumSelectElement>();

        //protected IBrowser browser;
        //protected IElementLocator locator;
        public SeleniumSelectElement(IBrowser? browser, IElement? parent, IElementLocator locator) : base(browser, parent, locator)
        {
        }

        private SelectElement SelectElement
        {
            get
            {
                var element = this.Element as IWebElement;
                if (element == null)
                    throw new NullReferenceException("cast to IWebElement failed!");
                return new SelectElement(element);
            }
        }

        public ISelectElement SelectByText(string optionText)
        {
            this.SelectElement.SelectByText(optionText);

            Log.Info("SelectByText `{0}` @ {1}", optionText, this.locator);
            return this;
        }

        public ISelectElement SelectByValue(string optionValue)
        {
            this.SelectElement.SelectByValue(optionValue);

            Log.Info("SelectByValue `{0}` @ {1}", optionValue, this.locator);
            return this;
        }

        public ISelectElement SelectByIndex(int optionIndex)
        {
            this.SelectElement.SelectByIndex(optionIndex);

            Log.Info("SelectByIndex `{0}` @ {1}", optionIndex, this.locator);
            return this;
        }

        public string GetOptionTextByIndex(int optionIndex)
        {
            return this.Element.FindElement(OpenQA.Selenium.By.XPath(string.Format("./descendant::option[{0}]", optionIndex))).Text;
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
