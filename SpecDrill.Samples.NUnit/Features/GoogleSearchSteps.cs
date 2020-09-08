using FluentAssertions;
using SpecDrill.Samples.NUnit3.PageObjects;
using SpecDrill.SpecFlow;
using System;
using TechTalk.SpecFlow;

namespace SpecDrill.Samples.NUnit3.Features
{
    [Binding]
    public class GoogleSearchSteps : SpecFlowBase
    {
        public GoogleSearchSteps(ScenarioContext scenarioContext, FeatureContext featureContext) => (this.scenarioContext, this.featureContext) = (scenarioContext, featureContext);
        [Given(@"I have entered ""(.*)"" into Google search")]
        public void GivenIHaveEnteredIntoGoogleSearch(string searchTerm)
        {
            var googleSearchPage = Browser.Open<GoogleSearchPage>();
            googleSearchPage.TxtSearch.SendKeys(searchTerm + "\x1B");
            googleSearchPage.TxtSearch.Blur();
            scenarioContext.Add("googleSearchPage", googleSearchPage);
        }
        
        [When(@"I press Search button")]
        public void WhenIPressSearchButton()
        {
            var googleSearchPage = scenarioContext["googleSearchPage"] as GoogleSearchPage;
            var resultsPage = googleSearchPage.BtnSearch.Click();
            scenarioContext.Add("resultsPage", resultsPage);
        }
        
        [Then(@"You should get a ""(.*)"" entry in search results")]
        public void ThenYouShouldGetAEntryInSearchResults(string textToMatch)
        {
            var resultsPage = scenarioContext["resultsPage"] as GoogleSearchResultsPage;
            var wikiResult = resultsPage.SearchResults.GetElementByText(textToMatch);
            wikiResult.Should().NotBeNull();
        }
    }
}
