using FluentAssertions;
using NUnit.Framework;
using SpecDrill.Infrastructure.Configuration;
using SpecDrill.NUnit3;
using SpecDrill.Samples.NUnit3.PageObjects;
using SpecDrill.Secondary.Ports.AutomationFramework;
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
        public void Test1() => TestMethod();
        [Test]
        public void Test2() => TestMethod();

        public void TestMethod()
        {
            //ConfigurationManager.Load();
            var googleSearchPage = Browser.Open<GoogleSearchPage>();
            var acceptButton = new Element(null, ElementLocatorFactory.Create(By.Id, "L2AGLb"));
            Wait.NoMoreThan(TimeSpan.FromSeconds(1))
                .Until(() => acceptButton.IsAvailable, throwExceptionOnTimeout: false);
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