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
    public class HoverTests : MsTestBase
    {
        [ClassInitialize]
        public static void ClassInitializer(TestContext testContext) => _ClassSetup(testContext);

        [TestMethod]
        public void ShouldRevealTooltipOnHover()
        {
            var hoverPage = Browser.Open<HoverCssPage>();
            hoverPage.DivTooltip.Hover();

            Wait.NoMoreThan(TimeSpan.FromSeconds(2)).Until(() => hoverPage.DivTooltipText.IsAvailable);

            hoverPage.DivTooltipText.IsAvailable.Should().BeTrue();
        }
    }
}
