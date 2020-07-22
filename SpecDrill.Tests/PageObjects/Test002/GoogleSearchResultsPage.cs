using SpecDrill;
using SpecDrill.SecondaryPorts.AutomationFramework;
using SpecDrill.WebControls;

namespace SomeTests.PageObjects.Test002
{
    public class GoogleSearchResultsPage : GoogleSearchPage
    {
        [Find(By.CssSelector, "div#search div.g")]
        public ListElement<SearchResultItemControl> SearchResults { get; private set; }
    }
}
