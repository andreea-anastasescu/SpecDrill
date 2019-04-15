using System;
using TechTalk.SpecFlow;
using SpecDrill.SpecFlow.MsTest;
using $rootnamespace$.PageObjects;
using System.Linq;
using FluentAssertions;

namespace $rootnamespace$.Features
{
    [Binding]
    public class GoogleSearchSteps : SpecFlowTestBase
    {
        [Given(@"I have entered ""(.*)"" into Google search")]
        public void GivenIHaveEnteredIntoGoogleSearch(string searchTerm)
        {
            var googleSearchPage = Browser.Open<GoogleSearchPage>();
            googleSearchPage.TxtSearch.SendKeys(searchTerm);
            ScenarioContext.Current.Add("googleSearchPage", googleSearchPage);
        }

        [When(@"I press Search button")]
        public void WhenIPressSearchButton()
        {
            var googleSearchPage = ScenarioContext.Current["googleSearchPage"] as GoogleSearchPage;
            var resultsPage = googleSearchPage.BtnSearch.Click();
            ScenarioContext.Current.Add("resultsPage", resultsPage);
        }

        [Then(@"You should get a ""(.*)"" entry in search results")]
        public void ThenYouShouldGetAEntryInSearchResults(string textToMatch)
        {
            var resultsPage = ScenarioContext.Current["resultsPage"] as GoogleSearchResultsPage;
            var wikiResult = resultsPage.SearchResults.FirstOrDefault(r => r.Link.Text.Contains(textToMatch));
            wikiResult.Should().NotBeNull();
        }

    }
}
