using FluentAssertions;
using NUnit.Framework;
using SomeTests.PageObjects.Alerts;
using SpecDrill;
using SpecDrill.NUnit3;
using SpecDrill.Secondary.Ports.AutomationFramework;
using System;

namespace SomeTests
{
    [TestFixture]
    public class AlertTests : NUnitBase
    {
        [Test]
        public void ShouldWaitForAlertAndAccept()
        {
            var alertPage = Browser.Open<AlertPage>();
            ElementFactory.Create(null, ElementLocatorFactory.Create(By.ClassName, "alert")).Click();
            var twoSeconds = TimeSpan.FromSeconds(2);
            Wait.NoMoreThan(twoSeconds).Until(() => Browser.IsAlertPresent);
            Browser.IsAlertPresent.Should().BeTrue();
            Browser.Alert.Text.Should().Contain("Servus");
            Browser.Alert.Accept();
            Browser.IsAlertPresent.Should().BeFalse();
        }

        [Test]
        public void ShouldWaitForConfirmAndAccept()
        {
            var alertPage = Browser.Open<AlertPage>();
            Browser.GetCapabilities();
            Browser.Click(ElementFactory.Create(null, ElementLocatorFactory.Create(By.LinkText, "Confirm")));
            var twoSeconds = TimeSpan.FromSeconds(2);
            Wait.NoMoreThan(twoSeconds).Until(() => Browser.IsAlertPresent);
            Browser.IsAlertPresent.Should().BeTrue();
            Browser.Alert.Text.Should().Contain("Sunny");
            Browser.Alert.Accept();
            Browser.IsAlertPresent.Should().BeFalse();
        }

        [Test]
        public void ShouldWaitForConfirmAndDismiss()
        {
            var alertPage = Browser.Open<AlertPage>();
            Browser.Click(ElementFactory.Create(null, ElementLocatorFactory.Create(By.LinkText, "Confirm")));
            var twoSeconds = TimeSpan.FromSeconds(2);
            Wait.NoMoreThan(twoSeconds).Until(() => Browser.IsAlertPresent);
            Browser.IsAlertPresent.Should().BeTrue();
            Browser.Alert.Text.Should().Contain("Sunny");
            Browser.Alert.Dismiss();
            Browser.IsAlertPresent.Should().BeFalse();
        }
    }
}
