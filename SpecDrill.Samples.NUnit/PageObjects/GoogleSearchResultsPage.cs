using SpecDrill.Secondary.Ports.AutomationFramework;

namespace SpecDrill.Samples.NUnit3.PageObjects
{
    public class GoogleSearchResultsPage : GoogleSearchPage
    {
        [Find(By.CssSelector, "div#search div.g")]
        public ListElement<SearchResultItemControl> SearchResults { get; private set; }
    }
}
