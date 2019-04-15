using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SomeTests.PageObjects.Test000;
using SpecDrill;
using SpecDrill.MsTest;
using FluentAssertions;
using SpecDrill.AutomationScopes;
using SomeTests.PageObjects.Alerts;
using SpecDrill.SecondaryPorts.AutomationFramework;

namespace SomeTests
{
    [TestClass]
    public class AlertTests : TestBase
    {
        [TestMethod]
        public void ShouldWaitForAlertAndAccept()
        {
            var alertPage = Browser.Open<AlertPage>();
            Browser.Click(WebElement.Create(null, ElementLocator.Create(By.ClassName, "alert")));
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
            Browser.Click(WebElement.Create(null, ElementLocator.Create(By.LinkText, "Confirm")));
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
            Browser.Click(WebElement.Create(null, ElementLocator.Create(By.LinkText, "Confirm")));
            var twoSeconds = TimeSpan.FromSeconds(2);
            Wait.NoMoreThan(twoSeconds).Until(() => Browser.IsAlertPresent);
            Browser.IsAlertPresent.Should().BeTrue();
            Browser.Alert.Text.Should().Contain("Sunny");
            Browser.Alert.Dismiss();
            Browser.IsAlertPresent.Should().BeFalse();
        }
    }
}
