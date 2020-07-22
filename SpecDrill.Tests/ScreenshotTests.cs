﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SomeTests.PageObjects.Test001;
using SpecDrill.MsTest;

namespace SomeTests
{
    [TestClass]
    public class ScreenshotTests : MsTestBase
    {
        [ClassInitialize]
        public static void ClassInitializer(TestContext testContext) => _ClassSetup(testContext);

        [TestMethod]
        public void ShouldSaveScreenshotSuccessfully()
        {
            var randomPage = Browser.Open<Test001CalculatorPage>();
            SaveScreenshot(TestContext.TestName);
        }
    }
}
