using SpecDrill;
using SpecDrill.Secondary.Ports.AutomationFramework;

#nullable disable

namespace SomeTests.PageObjects.Test002
{
    public class GoogleSearchResultsPage : GoogleSearchPage
    {
        [Find(By.XPath, "//*[@id=\"rso\"]/div")]
        public ListElement<SearchResultItemControl> SearchResults { get; private set; }
    }
}
