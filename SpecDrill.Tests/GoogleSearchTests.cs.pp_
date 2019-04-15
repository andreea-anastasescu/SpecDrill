using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using $rootnamespace$.PageObjects;
using SpecDrill;
using SpecDrill.MsTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace $rootnamespace$
{
    [TestClass]
    public class GoogleSearchTests : TestBase
    {
        [TestMethod]
        public void ShouldHaveWikipediaAmongResultsOnGoogleSearch()
        {
            var googleSearchPage = Browser.Open<GoogleSearchPage>();
            googleSearchPage.TxtSearch.SendKeys("drill wiki");
            googleSearchPage.TxtSearch.Blur();
            var resultsPage = googleSearchPage.BtnSearch.Click();

            #region Option 1: assuming it's first result
            //resultsPage.SearchResults[1].Link.Text.Should().Contain("Wikipedia");
            #endregion

            #region Option 2: searching through search results
            var wikiResult = resultsPage.SearchResults.FirstOrDefault(r => r.Link.Text.Contains("Wikipedia"));
            wikiResult.Should().NotBeNull();
            #endregion
        }
    }
}
