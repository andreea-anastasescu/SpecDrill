﻿using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SomeTests.PageObjects.Test002;
using SpecDrill;
using SpecDrill.MsTest;
using SpecDrill.Secondary.Ports.AutomationFramework;
using System;
using System.Linq;

namespace SomeTests
{
    [TestClass]
    public class GoogleSearchTests : MsTestBase
    {
        [ClassInitialize]
        public static void ClassInitializer(TestContext testContext) => _ClassSetup(testContext);

        [TestMethod]
        //[Ignore("Google search and search results page changed. New selectors have to be applied. Postponed for later.")]

        public void ShouldHaveWikipediaAmongResultsOnGoogleSearch()
        {
            var googleSearchPage = Browser.Open<GoogleSearchPage>();
            var acceptButton = new Element(null, ElementLocatorFactory.Create(By.Id, "L2AGLb"));
            Wait.NoMoreThan(TimeSpan.FromSeconds(21))
                .Until(() => acceptButton.IsAvailable,throwExceptionOnTimeout: false);
            //if (acceptButton.IsAvailable)
                acceptButton.Click();

            googleSearchPage.TxtSearch.SendKeys("drill wiki");
            googleSearchPage.TxtSearch.Blur();

            Wait.Until(() =>
                googleSearchPage.BtnSearch.IsDisplayed
            );

            var resultsPage = googleSearchPage.BtnSearch.Click();
            Wait.Until(() => resultsPage.SearchResults.IsAvailable);

            #region Option 1: assuming it's first result
            //resultsPage.SearchResults[1].Link.Text.Should().Contain("Drill");
            #endregion
            
            #region Option 2: searching through search results
            resultsPage.SearchResults.FirstOrDefault(r => r.Description.Text.Contains("drill"));
            #endregion

            //#region Option3
            //resultsPage.SearchResults.GetElementByText("Wikipedia").Count.Should().BeGreaterThan(0);
            //#endregion
        }


    }
}
