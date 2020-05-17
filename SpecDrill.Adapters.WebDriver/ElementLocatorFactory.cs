using SpecDrill.SecondaryPorts.AutomationFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpecDrill.Adapters.WebDriver
{
    internal class ElementLocatorFactory : IElementLocatorFactory
    {
        public IElementLocator Create(By locatorKind, string locatorValue)
            => new SeleniumElementLocator(locatorKind, locatorValue);
    }
}
