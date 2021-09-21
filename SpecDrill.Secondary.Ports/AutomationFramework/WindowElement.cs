namespace SpecDrill.Secondary.Ports.AutomationFramework
{
    public interface IWindowElement<out T> : IElement
        where T : IPage
    {
        T Open();
    }
}