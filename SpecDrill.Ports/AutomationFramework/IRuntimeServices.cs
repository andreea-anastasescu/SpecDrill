using SpecDrill.Configuration;
using SpecDrill.SecondaryPorts.AutomationFramework.Core;

namespace SpecDrill.SecondaryPorts.AutomationFramework
{
    public interface IRuntimeServices
    {
        IBrowserDriverFactory GetBrowserFactoryBuilder(Settings settings);
        IElementLocatorFactory ElementLocatorFactory { get; }
        IElementFactory GetElementFactory(IBrowser browser);
    }
}
