using SpecDrill.SecondaryPorts.AutomationFramework;
using System;

namespace SpecDrill
{
    public class ElementLocator
    {
        public static IElementLocatorFactory? ElementLocatorFactory { get; set; }
        private static IElementLocatorFactory Factory
            => ElementLocatorFactory ?? throw new Exception($"WebElement.ElementFactory was not provided with a IElementFactory instance!");
        public static IElementLocator Create(By locatorKind, string locatorValue)
            => Factory.Create(locatorKind, locatorValue);
    }
}
