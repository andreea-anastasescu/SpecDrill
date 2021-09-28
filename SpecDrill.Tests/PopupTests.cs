using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SomeTests.PageObjects.Popups;
using SpecDrill;
using SpecDrill.Exceptions;
using SpecDrill.MsTest;
using SpecDrill.Secondary.Ports.AutomationFramework;
using System;

namespace SomeTests
{
    [TestClass]
    public class PopupTests : MsTestBase
    {
        [ClassInitialize]
        public static void ClassInitializer(TestContext testContext) => _ClassSetup(testContext);

        // Test Cases:
        // Success
        // 1 - [FindTarget] - property [implemented]
        // 2 - [FindTarget] - selector
        // 3 - Control class [Find]
        // Error
        // 1 - property does not exist
        // 1 - property exists but type not supported
        // 3 - Control class has no Find attribute defined

        

        [TestMethod]
        public void ShouldBeAbleToOpenAndClosePopupByPropertyName()
        {
            var popupPage = Browser.Open<PopupPage>();
            var popup = popupPage.BtnOpenPopupPN.Click();
            popup.IsAvailable.Should().BeTrue();
            popup.Message.Text.Should().Contain("Hello");
            popup.Close.Click();
            popup.IsAvailable.Should().BeFalse();
        }


        [TestMethod]
        public void ShouldBeAbleToOpenAndClosePopupsViaListElement()
        {
            var popupPage = Browser.Open<PopupPage>();
            var popup = popupPage.PopupOpeners[1].Click();
            popup.IsAvailable.Should().BeTrue();
            popup.Message.Text.Should().Contain("Hello");
            popup.Close.Click();
            popup.IsAvailable.Should().BeFalse();
        }

        [TestMethod]
        public void ShouldBeAbleToOpenAndClosePopupBySelector()
        {
            var popupPage = Browser.Open<PopupPage>();
            var popup = popupPage.BtnOpenPopupS.Click();
            popup.IsAvailable.Should().BeTrue();
            popup.Message.Text.Should().Contain("Hello");
            popup.Close.Click();
            popup.IsAvailable.Should().BeFalse();
        }

        [TestMethod]
        [ExpectedException(typeof(NoFindTargetAttributeOnNavigationElementMemberNorFindAttributeOnTargetWebControlException))]
        public void ShouldThrowWenAttemptingToOpenPopupWithoutFindNorFindTargetAttributes()
        {
            var popupPage = Browser.Open<ErrPopupNoTargetLocator>();
        }

        [TestMethod]
        [ExpectedException(typeof(TargetPropertyIsNotWebControlException))]
        public void ShouldThrowWenPopupPageFindTargetPropertyIsNotINavigationTargetOrHasNoFindAttribute()
        {
            var popupPage = Browser.Open<ErrPopupPageFindTargetPropertyIsNotINavigationTargetOrHasNoFindAttribute>();
        }
    }
}
