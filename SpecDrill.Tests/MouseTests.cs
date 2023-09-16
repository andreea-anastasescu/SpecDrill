using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SomeTests.PageObjects;
using SpecDrill;
using System;
using System.Diagnostics;
using SpecDrill.MsTest;
using System.Windows.Markup;
using SpecDrill.Secondary.Ports.AutomationFramework;

namespace SomeTests
{
    public class ShadyMeadowsHomePage : WebPage
    {
        [Find(By.XPath, "//*[@id=\"root\"]/div/div/div[4]/div/div/div[3]/button")]

        public INavigationElement<RoomPage>? BookThisRoom { get; set; }
    }
    public class RoomPage : WebPage
    {
        [Find(By.XPath, "//*[@id=\"root\"]/div/div/div[4]/div/div[2]/div[2]/div/div[2]/div[5]/div[1]/div[2]")]
        public IElement? StartingCell { get; set; }
        [Find(By.XPath, "//*[@id=\"root\"]/div/div/div[4]/div/div[2]/div[2]/div/div[2]/div[5]/div[1]/div[3]")]
        public IElement? EndingCell { get; set; }
    }

    [TestClass]
    public class MouseTests : MsTestBase
    {
        [ClassInitialize]
        public static void ClassInitializer(TestContext testContext) => _ClassSetup(testContext);
        [TestMethod]
        //[ExpectedException(typeof(TimeoutException))]
        public void ShouldClickAndDrag()
        {
            var homePage = Browser.Open<ShadyMeadowsHomePage>();
            var roomPage = homePage.BookThisRoom!.Click();
            Wait.Until(() => roomPage.StartingCell!.IsVisible);
            Browser.ClickAndDrag(roomPage.StartingCell!, roomPage.EndingCell!, TimeSpan.FromSeconds(.2));
            roomPage.StartingCell!.IsVisible.Should().BeFalse();
            roomPage.EndingCell!.IsVisible.Should().BeFalse();
            System.Threading.Thread.Sleep(5000);
        }

    }
}