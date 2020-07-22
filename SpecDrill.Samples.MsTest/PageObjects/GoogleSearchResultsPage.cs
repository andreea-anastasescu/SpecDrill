using SpecDrill.SecondaryPorts.AutomationFramework;
using SpecDrill.WebControls;

namespace SpecDrill.Samples.MsTest.PageObjects
{
    public class GoogleSearchResultsPage : GoogleSearchPage
    {
        [Find(By.CssSelector, "div#search div.g")]
        public ListElement<SearchResultItemControl> SearchResults { get; private set; }
    }
}
