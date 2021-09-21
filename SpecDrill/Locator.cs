using SpecDrill.Secondary.Ports.AutomationFramework;
using System;

namespace SpecDrill
{
    public class ElementLocatorFactory
    {
        internal static IElementLocatorFactory? Instance { get; set; }
        public static IElementLocatorFactory Factory
            => Instance ?? throw new Exception($"WebElement.ElementFactory was not provided with a IElementFactory instance!");
        public static IElementLocator Create(By locatorKind, string locatorValue)
            => Factory.Create(locatorKind, locatorValue);
    }
}
