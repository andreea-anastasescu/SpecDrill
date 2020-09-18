using FluentAssertions;using NUnit.Allure.Core;
using NUnit.Framework;
using SpecDrill.NUnit3;
using SpecDrill.Samples.NUnit3.PageObjects;
using System;
using System.Linq;

namespace SpecDrill.Samples.NUnit3
{
    [TestFixture]
    [AllureNUnit]
    public class UnitTest1 : NUnitBase
    {
        [OneTimeSetUp]
        public void ClassInitializer() => _ClassSetup();
        
        [Test]
        public void TestMethod1()
        {
            var googleSearchPage = Browser.Open<GoogleSearchPage>();
            googleSearchPage.TxtSearch.SendKeys("drill wiki");
            googleSearchPage.TxtSearch.Blur();
            Wait.Until(() => googleSearchPage.BtnSearch.IsDisplayed);
            var resultsPage = googleSearchPage.BtnSearch.Click();

            #region Option 1: assuming it's first result
            //resultsPage.SearchResults[1].Link.Text.Should().Contain("Wikipedia");
            #endregion

            #region Option 2: searching through search results
            Wait.Until(() => resultsPage.SearchResults.IsDisplayed);
            var wikiResult = resultsPage.SearchResults.FirstOrDefault(r => r.Link.Text.Contains("Wikipedia"));
            wikiResult.Should().NotBeNull();
            #endregion
        }
    }
}