using SpecDrill.Secondary.Ports.AutomationFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpecDrill.Secondary.Adapters.WebDriver
{
    internal class ElementLocatorFactory : IElementLocatorFactory
    {
        public IElementLocator Create(By locatorKind, string locatorValue, int? index = null, bool isShadowRoot = false)
            => index == null ?
            new SeleniumElementLocator(locatorKind, locatorValue, isShadowRoot) :
            new SeleniumElementLocator(locatorKind, locatorValue, index.Value, isShadowRoot);
    }
}
