namespace SpecDrill.SecondaryPorts.AutomationFramework
{
    public enum By
    {
        Id,
        ClassName,
        CssSelector,
        XPath,
        Name,
        TagName,
        LinkText,
        PartialLinkText
    }

    public interface IElementLocator
    {
        By LocatorType { get; }
        string LocatorValue { get; }
        int? Index { get; }
        IElementLocator Copy();
        IElementLocator CopyWithIndex(int index);
    }
}