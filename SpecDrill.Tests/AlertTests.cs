using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SomeTests.PageObjects.Alerts;
using SpecDrill;
using SpecDrill.MsTest;
using SpecDrill.Secondary.Ports.AutomationFramework;
using System;

namespace SomeTests
{
    [TestClass]
    public class AlertTests : MsTestBase
    {
        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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
