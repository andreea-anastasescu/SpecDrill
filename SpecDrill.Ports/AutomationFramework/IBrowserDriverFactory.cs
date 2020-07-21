using SpecDrill.Infrastructure.Enums;

namespace SpecDrill.SecondaryPorts.AutomationFramework
{
    public interface IBrowserDriverFactory
    {
        IBrowserDriver Create(BrowserNames browserName);
    }
}
