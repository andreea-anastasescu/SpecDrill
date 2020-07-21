using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpecDrill.MsTest;
using SpecDrill.Samples.MsTest.PageObjects;
using System.Linq;

namespace SpecDrill.Samples.MsTest
{
    [TestClass]
    public class UnitTest1 : MsTestBase
    {
        [ClassInitialize]
        public static void ClassInitializer(TestContext testContext) => _ClassSetup(testContext);
        [TestMethod]
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
            var wikiResult = resultsPage.SearchResults.FirstOrDefault(r => r.Link.Text.Contains("Wikipedia"));
            wikiResult.Should().NotBeNull();
            #endregion
        }
    }
}
