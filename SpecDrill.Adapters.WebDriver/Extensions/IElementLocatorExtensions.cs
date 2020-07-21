using SpecDrill.SecondaryPorts.AutomationFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Selenium = OpenQA.Selenium;


namespace SpecDrill.SecondaryPorts.Adapters.WebDriver.Extensions
{
    public static class IElementLocatorExtensions
    {
        private static readonly Dictionary<By, Func<string, Selenium.By>> LocatorMap = new Dictionary<By, Func<string, Selenium.By>>()
                                                                                 {
                                                                                     { By.Id , Selenium.By.Id},    
                                                                                     { By.ClassName, Selenium.By.ClassName},
                                                                                     { By.CssSelector , Selenium.By.CssSelector},
                                                                                     { By.XPath , Selenium.By.XPath},
                                                                                     { By.Name , Selenium.By.Name},
                                                                                     { By.LinkText , Selenium.By.LinkText},
                                                                                     { By.PartialLinkText , Selenium.By.PartialLinkText},
                                                                                     { By.TagName , Selenium.By.TagName},
                                                                                 };
        public static Selenium.By ToSelenium(this IElementLocator locator)
        {
            return LocatorMap[locator.LocatorType](locator.LocatorValue);
        }
    }
}
