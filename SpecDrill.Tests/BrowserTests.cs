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
using SomeTests.PageObjects.Test001;

namespace SomeTests
{
    [TestClass]
    public class BrowserTests : MsTestBase
    {
        [ClassInitialize]
        public static void ClassInitializer(TestContext testContext) => _ClassSetup(testContext);

        [TestMethod]
        public void ShouldCorrectlyReadCapabilities()
        {
            var capabilities = Browser.GetCapabilities();
            // built-in
            capabilities.ContainsKey("platformName").Should().BeTrue();
            capabilities["platformName"].Should().NotBeNull();
            // custom
            capabilities.ContainsKey("x").Should().BeTrue();
            capabilities["x"].Should().Be("yy");
        }

        [TestMethod]
        public void ShouldReadCorrectUrl()
        {
            var calculatorPage = Browser.Open<Test001CalculatorPage>();
            Uri currentUrl = Browser.Url;

            currentUrl.IsFile.Should().BeTrue();
            currentUrl.PathAndQuery.EndsWith("/WebsiteMocks/Test001/calculator.html");
        }
    }
}
