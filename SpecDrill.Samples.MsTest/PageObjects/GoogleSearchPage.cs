using SpecDrill;
using SpecDrill.Secondary.Ports.AutomationFramework;

#nullable disable

namespace SpecDrill.Samples.MsTest.PageObjects
{
    public class GoogleSearchPage : WebPage
    {
        [Find(By.XPath, "//input[@name='q'] | //*[@id=\"APjFqb\"]")]
        public IElement TxtSearch { get; private set; }
        
        [Find(By.XPath, "//div[contains(@class,'FPdoLc')]//input[@name='btnK']")]
        public INavigationElement<GoogleSearchResultsPage> BtnSearch { get; private set; }
    }
}
