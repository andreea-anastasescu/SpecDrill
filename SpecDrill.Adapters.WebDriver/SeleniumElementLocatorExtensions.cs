using OpenQA.Selenium;
using SpecDrill.SecondaryPorts.AutomationFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecDrill.Adapters.WebDriver
{
    public static class SeleniumElementLocatorExtensions
    {
        public static OpenQA.Selenium.By? ToSeleniumLocator(this IElementLocator seleniumLocator)
        {
            switch (seleniumLocator.LocatorType)
            {
                case SpecDrill.SecondaryPorts.AutomationFramework.By.ClassName:
                    return OpenQA.Selenium.By.ClassName(seleniumLocator.LocatorValue);
                case SpecDrill.SecondaryPorts.AutomationFramework.By.CssSelector:
                    return OpenQA.Selenium.By.CssSelector(seleniumLocator.LocatorValue);
                case SpecDrill.SecondaryPorts.AutomationFramework.By.Id:
                    return OpenQA.Selenium.By.Id(seleniumLocator.LocatorValue);
                case SpecDrill.SecondaryPorts.AutomationFramework.By.LinkText:
                    return OpenQA.Selenium.By.LinkText(seleniumLocator.LocatorValue);
                case SpecDrill.SecondaryPorts.AutomationFramework.By.Name:
                    return OpenQA.Selenium.By.Name(seleniumLocator.LocatorValue);
                case SpecDrill.SecondaryPorts.AutomationFramework.By.PartialLinkText:
                    return OpenQA.Selenium.By.PartialLinkText(seleniumLocator.LocatorValue);
                case SpecDrill.SecondaryPorts.AutomationFramework.By.TagName:
                    return OpenQA.Selenium.By.TagName(seleniumLocator.LocatorValue);
                case SpecDrill.SecondaryPorts.AutomationFramework.By.XPath:
                    return OpenQA.Selenium.By.XPath(seleniumLocator.LocatorValue);
            }
            return null;
        }
    }
}
