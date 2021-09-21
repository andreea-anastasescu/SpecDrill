using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpecDrill.MsTest;
using SpecDrill.Samples.MsTest.PageObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SpecDrill.Samples.MsTest
{
    [TestClass]
    public class MsTestSample : MsTestBase
    {
        [ClassInitialize]
        public static void ClassInitializer(TestContext testContext) => _ClassSetup(testContext);
        [TestMethod]
        public void GoogleSearchScenario()
        {
            var googleSearchPage = Browser.Open<GoogleSearchPage>();
            var acceptButton = new Element(null, ElementLocatorFactory.Create(Secondary.Ports.AutomationFramework.By.XPath, "/html/body/div[2]/div[2]/div[3]/span/div/div/div[3]/button[2]"));
            Wait.NoMoreThan(TimeSpan.FromSeconds(7))
                .Until(() => acceptButton.IsAvailable);
            if (acceptButton.IsAvailable)
                acceptButton.Click();
            googleSearchPage.TxtSearch.SendKeys("drill wiki");
            googleSearchPage.TxtSearch.Blur();
            Wait.Until(() => 
                googleSearchPage.BtnSearch.IsDisplayed
            );
            var resultsPage = googleSearchPage.BtnSearch.Click();

            #region Option 1: assuming it's first result
            //resultsPage.SearchResults[1].Link.Text.Should().Contain("Wikipedia");
            #endregion

            #region Option 2: searching through search results
            var wikiResult = resultsPage.SearchResults.FirstOrDefault(r =>
            {
                var txt = r.Link.Text;
                return txt.Contains("Drill");
                });
            wikiResult.Should().NotBeNull();
            #endregion
        }
    }
}
