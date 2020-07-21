namespace SpecDrill.SecondaryPorts.AutomationFramework
{
    public interface ISelectElement : IElement
    {
        ISelectElement SelectByText(string optionText);
        ISelectElement SelectByValue(string optionValue);
        ISelectElement SelectByIndex(int optionIndex);
        string GetOptionTextByIndex(int optionIndex);
        string SelectedOptionText { get; }
        string SelectedOptionValue { get; }
        int OptionsCount { get; }
    }
}