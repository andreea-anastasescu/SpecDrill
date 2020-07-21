using SpecDrill.Configuration;
using SpecDrill.SecondaryPorts.AutomationFramework.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpecDrill.SecondaryPorts.AutomationFramework
{
    public interface IRuntimeServices
    {
        IBrowserDriverFactory GetBrowserFactoryBuilder(Settings settings);
        IElementLocatorFactory ElementLocatorFactory { get; }
        IElementFactory GetElementFactory(IBrowser browser);
    }
}
