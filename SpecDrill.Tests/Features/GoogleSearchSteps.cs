//using FluentAssertions;
//using SomeTests.PageObjects.Test002;
//using SpecDrill;
//using SpecDrill.SpecFlow;
//using System.Linq;
//using System;
//using TechTalk.SpecFlow;
//using SpecDrill.Secondary.Ports.AutomationFramework;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using OpenQA.Selenium;

//namespace SomeTests.Features
//{
//    [Binding]
//    public class GoogleSearchSteps : UiSpecFlowBase
//    {
//        public GoogleSearchSteps(ScenarioContext scenarioContext, FeatureContext featureContext) 
//            : base(scenarioContext, featureContext) { }

//        [Given(@"I have entered ""(.*)"" into Google search")]
//        public void GivenIHaveEnteredIntoGoogleSearch(string searchTerm)
//        {                                                                                // (open browser)
//            var googleSearchPage = Browser.Open<GoogleSearchPage>();                     // (goto 'Google Search Page')  ; $page           

//            var acceptButton = new Element(null, ElementLocatorFactory.Create(SpecDrill.Secondary.Ports.AutomationFramework.By.Id, "L2AGLb")); // (click $page.btnAccept)
//            Wait.NoMoreThan(TimeSpan.FromSeconds(7))
//                .Until(() => acceptButton.IsAvailable);
//            if (acceptButton.IsAvailable)                                                
//                acceptButton.Click();

//            googleSearchPage.TxtSearch.SendKeys("drill wiki");                          // (send keys 'drill wiki' to $page.TxtSearch)
//            googleSearchPage.TxtSearch.Blur();
           
//            scenarioContext.Add("googleSearchPage", googleSearchPage);
//        }

//        [When(@"I press Search button")]
//        public void WhenIPressSearchButton()
//        {
//            var googleSearchPage = scenarioContext["googleSearchPage"] as GoogleSearchPage;
            
//            Assert.IsNotNull(googleSearchPage);
            
//            Wait.Until(() =>
//               googleSearchPage.BtnSearch.IsDisplayed
//            );

//            var resultsPage = googleSearchPage.BtnSearch.Click();                         // (click $page.BtnSearch)
                        
//            scenarioContext.Add("resultsPage", resultsPage);
//        }

//        [Then(@"You should get a ""(.*)"" entry in search results")]
//        public void ThenYouShouldGetAEntryInSearchResults(string textToMatch)
//        {
//            var resultsPage = scenarioContext["resultsPage"] as GoogleSearchResultsPage;
//            Assert.IsNotNull(resultsPage);
//            var wikiResult = resultsPage.SearchResults.FirstOrDefault(x => x.Link.Text.Contains(textToMatch)); //GetElementByText(textToMatch);
//            wikiResult.Should().NotBeNull();                                            // (search list $page.SearchResults for $_.Link.Text.Contains('Drill Wiki')
//        }

//    }
//}
