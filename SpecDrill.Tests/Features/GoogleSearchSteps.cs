using FluentAssertions;
using SomeTests.PageObjects.Test002;
using SpecDrill;
using SpecDrill.SpecFlow;
using System.Linq;
using System;
using TechTalk.SpecFlow;
using SpecDrill.Secondary.Ports.AutomationFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SomeTests.Features
{
    [Binding]
    public class GoogleSearchSteps : UiSpecFlowBase
    {
        public GoogleSearchSteps(ScenarioContext scenarioContext, FeatureContext featureContext) 
            : base(scenarioContext, featureContext) { }

        [Given(@"I have entered ""(.*)"" into Google search")]
        public void GivenIHaveEnteredIntoGoogleSearch(string searchTerm)
        {
            var googleSearchPage = Browser.Open<GoogleSearchPage>();
            var acceptButton = new Element(null, ElementLocatorFactory.Create(By.XPath, "/html/body/div[2]/div[2]/div[3]/span/div/div/div[3]/button[2]"));
            Wait.NoMoreThan(TimeSpan.FromSeconds(7))
                .Until(() => acceptButton.IsAvailable);
            if (acceptButton.IsAvailable)
                acceptButton.Click();

            googleSearchPage.TxtSearch.SendKeys("drill wiki");
            googleSearchPage.TxtSearch.Blur();
           
            scenarioContext.Add("googleSearchPage", googleSearchPage);
        }

        [When(@"I press Search button")]
        public void WhenIPressSearchButton()
        {
            var googleSearchPage = scenarioContext["googleSearchPage"] as GoogleSearchPage;
            
            Assert.IsNotNull(googleSearchPage);
            
            Wait.Until(() =>
               googleSearchPage.BtnSearch.IsDisplayed
            );

            var resultsPage = googleSearchPage.BtnSearch.Click();
                        
            scenarioContext.Add("resultsPage", resultsPage);
        }

        [Then(@"You should get a ""(.*)"" entry in search results")]
        public void ThenYouShouldGetAEntryInSearchResults(string textToMatch)
        {
            var resultsPage = scenarioContext["resultsPage"] as GoogleSearchResultsPage;
            Assert.IsNotNull(resultsPage);
            var wikiResult = resultsPage.SearchResults.GetElementByText(textToMatch);
            wikiResult.Should().NotBeNull();
        }

    }
}
