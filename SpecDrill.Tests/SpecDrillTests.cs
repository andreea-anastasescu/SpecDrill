using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SomeTests.PageObjects.Alerts;
using SomeTests.PageObjects.Test000;
using SpecDrill;
using SpecDrill.AutomationScopes;
using SpecDrill.MsTest;
using SpecDrill.Secondary.Ports.AutomationFramework;
using System;
using System.Linq;

namespace SomeTests
{
    [TestClass]
    public class SpecDrillTests : MsTestBase
    {
        [TestMethod]
        public void ShouldOpenBrowserWhenHomepageIsOpened()
        {
            var virtualStoreLoginPage = Browser.Open<Test000LoginPage>();

            Wait.Until(() => virtualStoreLoginPage.IsLoaded);

            virtualStoreLoginPage.TxtUserName.SendKeys("alina").Blur();
            virtualStoreLoginPage.TxtUserName.Clear().SendKeys("cosmin");
            virtualStoreLoginPage.TxtPassword.SendKeys("abc123");

            virtualStoreLoginPage.DdlCountry.GetOptionsText().Count().Should().Be(virtualStoreLoginPage.DdlCountry.OptionsCount);
            virtualStoreLoginPage.DdlCountry.SelectByValue("md");
            virtualStoreLoginPage.DdlCountry.SelectedOptionText.Should().Be("Moldova");

            virtualStoreLoginPage.DdlCity.SelectByText("Chisinau");
            virtualStoreLoginPage.DdlCity.SelectedOptionText.Should().Be("Chisinau");

            virtualStoreLoginPage.DdlCountry.SelectByIndex(1);
            virtualStoreLoginPage.DdlCountry.SelectedOptionText.Should().Be("Romania");

            var homePage = virtualStoreLoginPage.BtnLogin.Click();

            homePage.Title.Should().Be("Virtual Store - Home");

            homePage.LblUserName.Text.Should().Be("Cosmin");
            var loginPage = homePage.CtlMenu.LnkLogin.Click();

            loginPage.Title.Should().Be("Virtual Store - Login");
        }

        //TODO: Create Hover tests on css hover menu with at least 2 levels
        [TestMethod]
        public void ShouldBeAbleToNavigateWithinFrame()
        {
            var gatewayPage = Browser.Open<Test000GatewayPage>();


            using (var virtualStoreLoginPage = gatewayPage.FrmPortal.Open())
            {
                virtualStoreLoginPage.TxtUserName.SendKeys("alina").Blur();
                virtualStoreLoginPage.TxtUserName.Clear().SendKeys("cosmin");
                virtualStoreLoginPage.TxtPassword.SendKeys("abc123");

                virtualStoreLoginPage.DdlCountry.SelectByValue("md");
                virtualStoreLoginPage.DdlCountry.SelectedOptionText.Should().Be("Moldova");

                virtualStoreLoginPage.DdlCity.SelectByText("Chisinau");
                virtualStoreLoginPage.DdlCity.SelectedOptionText.Should().Be("Chisinau");

                virtualStoreLoginPage.DdlCountry.SelectByIndex(1);
                virtualStoreLoginPage.DdlCountry.SelectedOptionText.Should().Be("Romania");

                var homePage = virtualStoreLoginPage.BtnLogin.Click();

                homePage.Title.Should().Be("Virtual Store - Home");

                homePage.LblUserName.Text.Should().Be("Cosmin");
                var loginPage = homePage.CtlMenu.LnkLogin.Click();

                loginPage.Title.Should().Be("Virtual Store - Login");
            }

            Wait.Until(() => gatewayPage.LblGwText.IsAvailable);

            gatewayPage.LblGwText.Text.Should().Contain("Gateway");
        }

        [TestMethod]
        public void Issue_6_ShouldCorrectlySelectItemsWhenAccessedByIndex()
        {
            var gatewayPage = Browser.Open<Test000GatewayPage>();
            var item1Text = gatewayPage.UList[1].Text;
            item1Text.Should().Be("O1");
            var item2Text = gatewayPage.UList[2].Text;
            item2Text.Should().Be("O2");
        }

        [TestMethod]
        public void Issue_11_ShouldNoBlockForLongerThanSpecifiedWhenCallingWaitForNoMoreThan()
        {
            var gatewayPage = Browser.Open<Test000GatewayPage>();
            var nonExistingElement = ElementFactory.Create(null, ElementLocatorFactory.Create(SpecDrill.Secondary.Ports.AutomationFramework.By.CssSelector, ".abc-xyz"));
            var seconds = 2;
            using (var wait = Browser.ImplicitTimeout(TimeSpan.FromSeconds(.25))) 
                //TODO: Implicit Timeout affects explicit waits (NoMoreThan). So impicit wait cannot be greater than explicit wait currently. TBD: if that should remain the case or explicit wait can override the implicit timeout to a small enough value, not to affect explicit wait interval (e.g. make the ImplicitTimeout .5s when explicityly waiting.
            {
                var timeLimit = TimeSpan.FromSeconds(seconds);
                Action waitForNonExistingElement = () =>
                    Wait.NoMoreThan(timeLimit).Until(() => nonExistingElement.IsAvailable);
                using (var benchmark = new BenchmarkScope("timing Wait.NoMoreThan(...)"))
                {
                    waitForNonExistingElement.Should().Throw<TimeoutException>();
                    benchmark.Elapsed.Should().BeCloseTo(timeLimit, TimeSpan.FromMilliseconds(500));
                }
            }
        }

        [TestMethod]
        public void ShouldWaitForAlertAndAccept()
        {
            var alertPage = Browser.Open<AlertPage>();
            Browser.Click(ElementFactory.Create(null, ElementLocatorFactory.Create(By.ClassName, "alert")));
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

