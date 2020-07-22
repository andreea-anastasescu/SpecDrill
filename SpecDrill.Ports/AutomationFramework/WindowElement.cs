namespace SpecDrill.SecondaryPorts.AutomationFramework
{
    public interface IWindowElement<out T> : IElement
        where T : IPage
    {
        T Open();
    }
}