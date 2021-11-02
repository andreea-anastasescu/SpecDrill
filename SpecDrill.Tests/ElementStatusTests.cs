using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using SomeTests.PageObjects;
using SpecDrill;
using SpecDrill.AutomationScopes;
using SpecDrill.MsTest;
using SpecDrill.NUnit3;
using SpecDrill.Secondary.Ports.AutomationFramework;
using SpecDrill.Secondary.Ports.AutomationFramework.Exceptions;
using System;

namespace SomeTests
{
    [TestFixture]
    public class ElementStatusTests : NUnitBase
    {
        [Test]
        public void ShouldWaitForElementToBecomeDisabled()
        {
            var elStatusPage = Browser.Open<ElementStatusPage>();

            var disablingElement = ElementFactory.Create(null, ElementLocatorFactory.Create(By.Id, "willBeDisabled"));

            using (var benchmark = new BenchmarkScope("Time Until Disable"))
            {
                //Wait.NoMoreThan(TimeSpan.FromSeconds(2)).Until(() => disablingElement.IsAvailable);
                Wait.NoMoreThan(TimeSpan.FromSeconds(3)).Until(() => !disablingElement.IsEnabled);
                benchmark.Elapsed.Should().BeCloseTo(TimeSpan.FromSeconds(2), TimeSpan.FromMilliseconds(300));
            }
        }

        [Test]
        public void ShouldWaitForElementToBecomeVisible()
        {
            var elStatusPage = Browser.Open<ElementStatusPage>();

            var disablingElement = ElementFactory.Create(null, ElementLocatorFactory.Create(By.Id, "willBeDisplayed"));

            using (var benchmark = new BenchmarkScope("Time Until Display"))
            {
                Wait.NoMoreThan(TimeSpan.FromSeconds(3)).Until(() => disablingElement.IsDisplayed);
                benchmark.Elapsed.Should().BeCloseTo(TimeSpan.FromSeconds(2), TimeSpan.FromMilliseconds(300));
            }
        }

        [Test]
        public void ShouldNotCrashIfElementNotAvailable()
        {
            Browser.Open<ElementStatusPage>();

            var unrealElement = ElementFactory.Create(null, ElementLocatorFactory.Create(By.Id, "doesNotExist"));

            unrealElement.IsAvailable.Should().BeFalse();
        }

        [Test]
        public void ShouldThrowExceptionWhenTestingDisplayedAndElementNotPresent()
        {
            var elStatusPage = Browser.Open<ElementStatusPage>();
            var unrealElement = ElementFactory.Create(null, ElementLocatorFactory.Create(By.Id, "doesNotExist"));

            Action checkDisplayed = () => { var displayed = unrealElement.IsDisplayed; };

            checkDisplayed.Should().Throw<ElementNotFoundException>();
        }


        [Test]
        public void ShouldThrowExceptionWhenTestingEnabledAndElementNotPresent()
        {
            var elStatusPage = Browser.Open<ElementStatusPage>();
            var unrealElement = ElementFactory.Create(null, ElementLocatorFactory.Create(By.Id, "doesNotExist"));

            Action checkDisplayed = () => { var displayed = unrealElement.IsEnabled; };

            checkDisplayed.Should().Throw<ElementNotFoundException>();
        }
    }
}
