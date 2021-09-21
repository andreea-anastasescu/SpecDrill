namespace SpecDrill.Secondary.Ports.AutomationFramework
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
        bool IsShadowRoot { get; }
        IElementLocator Copy();
        IElementLocator CopyWithIndex(int index, bool isShadowRoot = false);
    }
}