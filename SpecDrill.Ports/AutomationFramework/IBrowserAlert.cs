namespace SpecDrill.SecondaryPorts.AutomationFramework
{
    public interface IBrowserAlert
    {
        void Accept();
        void Dismiss();
        string Text { get; }
        void SendKeys(string keys);
    }
}
