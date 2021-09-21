using SpecDrill.Secondary.Ports.AutomationFramework;

namespace SpecDrill.Samples.NUnit3.PageObjects
{
    public class SearchResultItemControl : WebControl
    {
        [Find(By.XPath, "//div/div/div[1]/a/h3")]
        public IElement Link { get; private set; }

        [Find(By.XPath, "//div[1]/div[1]/div[2]/div[1]")]
        public IElement Description { get; private set; }

        public SearchResultItemControl(IElement parent, IElementLocator locator) : base(parent, locator) { }
    }
}
