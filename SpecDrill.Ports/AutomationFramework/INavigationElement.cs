namespace SpecDrill.SecondaryPorts.AutomationFramework
{
    public interface INavigationElement<out T> : IElement
        where T: IPage
    {
        T Click();
        T DoubleClick();
    }
}