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
        [ClassInitialize]
        public static void ClassInitializer(TestContext testContext) => _ClassSetup(testContext);

        [TestMethod]
        public void ShouldOpenBrowserWhenHomepageIsOpened()
        {
            var virtualStoreLoginPage = Browser.Open<Test000LoginPage>();

            Wait.Until(() => virtualStoreLoginPage.IsLoaded);

            virtualStoreLoginPage.TxtUserName.SendKeys("alina").Blur();
            virtualStoreLoginPage.TxtUserName.Clear().SendKeys("cosmin");
            virtualStoreLoginPage.TxtPassword.SendKeys("abc123");

            Assert.AreEqual(virtualStoreLoginPage.DdlCountry.GetOptionsText().Count(), virtualStoreLoginPage.DdlCountry.OptionsCount);
            virtualStoreLoginPage.DdlCountry.SelectByValue("md");
            Assert.AreEqual("Moldova", virtualStoreLoginPage.DdlCountry.SelectedOptionText);

            virtualStoreLoginPage.DdlCity.SelectByText("Chisinau");
            Assert.AreEqual("Chisinau", virtualStoreLoginPage.DdlCity.SelectedOptionText);

            virtualStoreLoginPage.DdlCountry.SelectByIndex(1);
            Assert.AreEqual("Romania", virtualStoreLoginPage.DdlCountry.SelectedOptionText);

            var homePage = virtualStoreLoginPage.BtnLogin.Click();

            Assert.AreEqual("Virtual Store - Home", homePage.Title);

            Assert.AreEqual("Cosmin", homePage.LblUserName.Text);
            var loginPage = homePage.CtlMenu.LnkLogin.Click();

            Assert.AreEqual("Virtual Store - Login", loginPage.Title);
        }

        //TODO: Create Hover tests on css hover menu with at least 2 levels

        [TestMethod]
        public void ShouldReadListWhenListControlIsUsed()
        {

        }

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
                Assert.AreEqual("Moldova", virtualStoreLoginPage.DdlCountry.SelectedOptionText);

                virtualStoreLoginPage.DdlCity.SelectByText("Chisinau");
                Assert.AreEqual("Chisinau", virtualStoreLoginPage.DdlCity.SelectedOptionText);

                virtualStoreLoginPage.DdlCountry.SelectByIndex(1);
                Assert.AreEqual("Romania", virtualStoreLoginPage.DdlCountry.SelectedOptionText);

                var homePage = virtualStoreLoginPage.BtnLogin.Click();

                Assert.AreEqual("Virtual Store - Home", homePage.Title);

                Assert.AreEqual("Cosmin", homePage.LblUserName.Text);
                var loginPage = homePage.CtlMenu.LnkLogin.Click();

                Assert.AreEqual("Virtual Store - Login", loginPage.Title);
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

            using (var wait = Browser.ImplicitTimeout(TimeSpan.FromSeconds(3)))
            {
                var timeLimit = TimeSpan.FromSeconds(1);
                Action waitForNonExistingElement = () =>
                Wait.NoMoreThan(timeLimit).Until(() => nonExistingElement.IsAvailable);
                using (var benchmark = new BenchmarkScope("timing Wait.NoMoreThan(...)"))
                {
                    waitForNonExistingElement.Should().Throw<TimeoutException>();
                    benchmark.Elapsed.Should().BeCloseTo(timeLimit, TimeSpan.FromMilliseconds(300));
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

