using SpecDrill;
using SpecDrill.Secondary.Ports.AutomationFramework;

namespace SomeTests.PageObjects.Test002
{
    public class GoogleSearchResultsPage : GoogleSearchPage
    {
        [Find(By.CssSelector, "div#search div.g")]
        public ListElement<SearchResultItemControl> SearchResults { get; private set; }
    }
}
