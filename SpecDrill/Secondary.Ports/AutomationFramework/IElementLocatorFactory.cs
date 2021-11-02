namespace SpecDrill.Secondary.Ports.AutomationFramework
{
    public interface IElementLocatorFactory
    {
        IElementLocator Create(By locatorKind, string locatorValue, int? index = null, bool isShadowRoot = false);

    }
}
