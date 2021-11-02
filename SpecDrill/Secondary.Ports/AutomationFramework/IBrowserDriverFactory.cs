using SpecDrill.Infrastructure.Enums;

namespace SpecDrill.Secondary.Ports.AutomationFramework
{
    public interface IBrowserDriverFactory
    {
        IBrowserDriver Create(BrowserNames browserName);
    }
}
