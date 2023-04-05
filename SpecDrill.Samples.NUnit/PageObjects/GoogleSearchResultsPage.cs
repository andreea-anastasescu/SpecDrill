using SpecDrill;
using SpecDrill.Secondary.Ports.AutomationFramework;

#nullable disable

namespace SpecDrill.Samples.NUnit3.PageObjects
{
    public class GoogleSearchResultsPage : GoogleSearchPage
    {
        [Find(By.XPath, "//*[@id=\"rso\"]/div")]
        public ListElement<SearchResultItemControl> SearchResults { get; private set; }
    }
}
