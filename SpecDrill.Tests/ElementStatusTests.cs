using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SomeTests.PageObjects;
using SpecDrill;
using SpecDrill.AutomationScopes;
using SpecDrill.MsTest;
using SpecDrill.SecondaryPorts.AutomationFramework;
using SpecDrill.SecondaryPorts.AutomationFramework.Exceptions;
using System;

namespace SomeTests
{
    [TestClass]
    public class ElementStatusTests : MsTestBase
    {
        [ClassInitialize]
        public static void ClassInitializer(TestContext testContext) => _ClassSetup(testContext);

        [TestMethod]
        public void ShouldWaitForElementToBecomeDisabled()
        {
            var elStatusPage = Browser.Open<ElementStatusPage>();

            var disablingElement = WebElement.Create(null, ElementLocator.Create(By.Id, "willBeDisabled"));

            using (var benchmark = new BenchmarkScope("Time Until Disable"))
            {
                //Wait.NoMoreThan(TimeSpan.FromSeconds(2)).Until(() => disablingElement.IsAvailable);
                Wait.NoMoreThan(TimeSpan.FromSeconds(3)).Until(() => !disablingElement.IsEnabled);
                benchmark.Elapsed.Should().BeCloseTo(TimeSpan.FromSeconds(2), 300);
            }
        }

        [TestMethod]
        public void ShouldWaitForElementToBecomeVisible()
        {
            var elStatusPage = Browser.Open<ElementStatusPage>();

            var disablingElement = WebElement.Create(null, ElementLocator.Create(By.Id, "willBeDisplayed"));

            using (var benchmark = new BenchmarkScope("Time Until Display"))
            {
                Wait.NoMoreThan(TimeSpan.FromSeconds(3)).Until(() => disablingElement.IsDisplayed);
                benchmark.Elapsed.Should().BeCloseTo(TimeSpan.FromSeconds(2), 300);
            }
        }

        [TestMethod]
        public void ShouldNotCrashIfElementNotAvailable()
        {
            Browser.Open<ElementStatusPage>();

            var unrealElement = WebElement.Create(null, ElementLocator.Create(By.Id, "doesNotExist"));

            Assert.IsFalse(unrealElement.IsAvailable);
        }

        [TestMethod]
        public void ShouldThrowExceptionWhenTestingDisplayedAndElementNotPresent()
        {
            var elStatusPage = Browser.Open<ElementStatusPage>();
            var unrealElement = WebElement.Create(null, ElementLocator.Create(By.Id, "doesNotExist"));

            Action checkDisplayed = () => { var displayed = unrealElement.IsDisplayed; };

            checkDisplayed.Should().Throw<ElementNotFoundException>();
        }


        [TestMethod]
        public void ShouldThrowExceptionWhenTestingEnabledAndElementNotPresent()
        {
            var elStatusPage = Browser.Open<ElementStatusPage>();
            var unrealElement = WebElement.Create(null, ElementLocator.Create(By.Id, "doesNotExist"));

            Action checkDisplayed = () => { var displayed = unrealElement.IsEnabled; };

            checkDisplayed.Should().Throw<ElementNotFoundException>();
        }
    }
}
