namespace SpecDrill.Secondary.Ports.AutomationFramework
{
    public interface IBrowserAlert
    {
        void Accept();
        void Dismiss();
        string Text { get; }
        void SendKeys(string keys);
    }
}
