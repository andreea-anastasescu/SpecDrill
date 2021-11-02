using FluentAssertions;
using NUnit.Framework;
using SomeTests.PageObjects.Popups;
using SpecDrill;
using SpecDrill.Exceptions;
using SpecDrill.Infrastructure;
using SpecDrill.NUnit3;
using System;

namespace SomeTests
{
    [TestFixture]
    public class PopupTests : NUnitBase
    {
        public PopupTests() : base(false) {
        
        }
        // Test Cases:
        // Success
        // 1 - [FindTarget] - property [implemented]
        // 2 - [FindTarget] - selector
        // 3 - Control class [Find]
        // Error
        // 1 - property does not exist
        // 1 - property exists but type not supported
        // 3 - Control class has no Find attribute defined

        [Test]
        public void ShouldBeAbleToOpenAndClosePopupByPropertyName()
        {
            var popupPage = Browser.Open<PopupPage>();
            var popup = popupPage.BtnOpenPopupPN.Click();
            popup.IsAvailable.Should().BeTrue();
            popup.Message.Text.Should().Contain("Hello");
            popup.Close.Click();
            popup.IsAvailable.Should().BeFalse();
        }


        [Test]
        public void ShouldBeAbleToOpenAndClosePopupsViaListElement()
        {
            var popupPage = Browser.Open<PopupPage>();
            var popup = popupPage.PopupOpeners[1].Click();
            popup.IsAvailable.Should().BeTrue();
            popup.Message.Text.Should().Contain("Hello");
            popup.Close.Click();
            popup.IsAvailable.Should().BeFalse();
        }

        [Test]
        public void ShouldBeAbleToOpenAndClosePopupBySelector()
        {
            var popupPage = Browser.Open<PopupPage>();
            var popup = popupPage.BtnOpenPopupS.Click();
            popup.IsAvailable.Should().BeTrue();
            popup.Message.Text.Should().Contain("Hello");
            popup.Close.Click();
            popup.IsAvailable.Should().BeFalse();
        }

        [Test]
        public void ShouldThrowWenAttemptingToOpenPopupWithoutFindNorFindTargetAttributes()
        {
            Action @try = () =>
            {
                var popupPage = Browser.Open<ErrPopupNoTargetLocator>();
            };

            @try.Should()
               .Throw<NoFindTargetAttributeOnNavigationElementMemberNorFindAttributeOnTargetWebControlException>();

        }

        [Test]
        public void ShouldThrowWenPopupPageFindTargetPropertyIsNotINavigationTargetOrHasNoFindAttribute()
        {
            Action @try = () =>
            {
                var popupPage = Browser.Open<ErrPopupPageFindTargetPropertyIsNotINavigationTargetOrHasNoFindAttribute>();
            };
            @try.Should().Throw<TargetPropertyIsNotWebControlException>();
        }
    }
}
