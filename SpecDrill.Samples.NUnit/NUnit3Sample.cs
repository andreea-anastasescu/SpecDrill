using FluentAssertions;
using NUnit.Framework;
using SpecDrill.Infrastructure.Configuration;
using SpecDrill.NUnit3;
using SpecDrill.Samples.NUnit3.PageObjects;
using System;
using System.Linq;

namespace SpecDrill.Samples.NUnit3
{
    [TestFixture]
    //[AllureNUnit]
    public class NUnit3Sample : NUnitBase
    {
        [OneTimeSetUp]
        public void ClassInitializer() => _ClassSetup();
        
        [Test]
        public void TestMethod1()
        {
            ConfigurationManager.Load();
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