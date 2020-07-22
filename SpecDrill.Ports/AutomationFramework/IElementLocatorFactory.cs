namespace SpecDrill.SecondaryPorts.AutomationFramework
{
    public interface IElementLocatorFactory
    {
        IElementLocator Create(By locatorKind, string locatorValue);

    }
}
