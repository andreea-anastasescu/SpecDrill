using OpenQA.Selenium;
using SpecDrill.Secondary.Ports.AutomationFramework;
using SpecDrill.Secondary.Ports.AutomationFramework.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Selenium = OpenQA.Selenium;


namespace SpecDrill.Secondary.Adapters.WebDriver.Extensions
{
    public static class IElementExtensions
    {
        public static IWebElement ToWebElement(this IElement element)
        {
            using (element.Browser.ImplicitTimeout(TimeSpan.FromSeconds(.5d)))
            {
                if (!(element.NativeElementSearchResult().Elements.FirstOrDefault() is IWebElement webElement))
                {
                    throw new ElementNotFoundException($"SpecDrill: Element ({element.Locator}) Not Found!");
                }

                return webElement;
            }
        }
    }
}
