using SpecDrill.SecondaryPorts.AutomationFramework;

namespace SpecDrill.Samples.MsTest.PageObjects
{
    public class SearchResultItemControl : WebControl
    {
        [Find(By.CssSelector, "div a")]
        public IElement Link { get; private set; }

        [Find(By.CssSelector, "div.rc>div.s>span.st")]
        public IElement Description { get; private set; }

        public SearchResultItemControl(IElement parent, IElementLocator locator) : base(parent, locator) { }
    }
}
