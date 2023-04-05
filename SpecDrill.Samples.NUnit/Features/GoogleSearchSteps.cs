using FluentAssertions;
using SpecDrill.Infrastructure.Configuration;
using SpecDrill.Samples.NUnit3.PageObjects;
using SpecDrill.Secondary.Ports.AutomationFramework;
using SpecDrill.SpecFlow;
using System;
using TechTalk.SpecFlow;

namespace SpecDrill.Samples.NUnit3.Features
{
    [Binding]
    public class GoogleSearchSteps : UiSpecFlowBase
    {
        public GoogleSearchSteps(ScenarioContext scenarioContext, FeatureContext featureContext) : base(scenarioContext, featureContext) { }
        
        [Given(@"I have entered ""(.*)"" into Google search")]
        public void GivenIHaveEnteredIntoGoogleSearch(string searchTerm)
        {
            var googleSearchPage = Browser.Open<GoogleSearchPage>();
            var acceptButton = new Element(null, ElementLocatorFactory.Create(By.Id, "L2AGLb"));
            Wait.NoMoreThan(TimeSpan.FromSeconds(7))
                .Until(() => acceptButton.IsAvailable);
            if (acceptButton.IsAvailable)
                acceptButton.Click();

            googleSearchPage.TxtSearch.SendKeys("drill wiki");
            googleSearchPage.TxtSearch.Blur();
            Wait.Until(() =>
                googleSearchPage.BtnSearch.IsDisplayed
            );

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
