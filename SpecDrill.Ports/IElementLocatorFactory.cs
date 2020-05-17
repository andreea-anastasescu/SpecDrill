using System;
using System.Collections.Generic;
using System.Text;

namespace SpecDrill.SecondaryPorts.AutomationFramework
{
    public interface IElementLocatorFactory
    {
        IElementLocator Create(By locatorKind, string locatorValue);
        
    }
}
