using SpecDrill.Configuration;
using SpecDrill.Secondary.Ports.AutomationFramework;
using SpecDrill.Secondary.Ports.AutomationFramework.Core;

namespace SpecDrill.Secondary.Ports.AutomationFramework
{
    public interface IRuntimeServices
    {
        IBrowserDriverFactory GetBrowserFactoryBuilder(Settings settings);
        IElementLocatorFactory ElementLocatorFactory { get; }
        IElementFactory GetElementFactory(IBrowser browser);
    }
}
