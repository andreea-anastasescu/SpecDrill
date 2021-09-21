using SpecDrill.Secondary.Ports.AutomationFramework;

namespace SpecDrill.Samples.NUnit3.PageObjects
{
    public class GoogleSearchPage : WebPage
    {
        [Find(By.XPath, "//input[@name='q']")]
        public IElement TxtSearch { get; private set; }

        [Find(By.XPath, "//div[contains(@class,'FPdoLc')]//input[@name='btnK']")]
        public INavigationElement<GoogleSearchResultsPage> BtnSearch { get; private set; }
    }
}
