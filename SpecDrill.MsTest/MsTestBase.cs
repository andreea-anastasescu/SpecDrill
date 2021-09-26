using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpecDrill.Infrastructure;
using SpecDrill.Secondary.Adapters.WebDriver;
using static SpecDrill.Secondary.Adapters.WebDriver.Init;
using SpecDrill.Tests;
using System;
using SpecDrill.Secondary.Ports.AutomationFramework;
using Microsoft.Extensions.DependencyInjection;

namespace SpecDrill.MsTest
{
    public class MsTestBase : UiScenarioBase
    {
        protected static TestContext? TestContext;
        [ClassInitialize]
        public static void _ClassSetup(TestContext testContext)
        {
            try
            {
                DI.ConfigureServices(services =>
                {
                    services.AddWebdriverSecondaryAdapter();
                   // return services;
                });
                DI.Apply();
                TestContext = testContext;
                ClassSetup();
            }
            catch (Exception e)
            {
                Logger.LogError($"Failed in ClassInitialize for test method [{testContext.TestName}] with {e}");
            }
        }
        protected static Action ClassSetup = () => { };


        [ClassCleanup]
        public static void _ClassCleanup()
        {
            ClassTeardown();
        }
        protected static Action ClassTeardown = () => { };


        [TestInitialize]
        public void _TestSetup()
        {
            if (TestContext == null) throw new Exception("TextContext is not initialized. SpecDrill.MsTest.TestBase.ClassSetup() was not invoked yet!\n Please add following code snippet to your test class:\n[ClassInitialize]\npublic static void ClassInitializer(TestContext testContext) => _ClassSetup(testContext);\n");
            var runtimeServices = DI.ServiceProvider.GetService<IRuntimeServices>();
            if (runtimeServices == null) throw new Exception("IRuntimeServices could not be resoved by DI ServiceProvider");
            _ScenarioSetup(runtimeServices, TestContext.TestName);
        }

        [TestCleanup]
        public void _TestCleanup()
        {
            if (TestContext == null) throw new Exception("TextContext is not initialized. SpecDrill.MsTest.TestBase.ClassSetup() was not invoked yet!\n Please add following code snippet to your test class:\n[ClassInitialize]\npublic static void ClassInitializer(TestContext testContext) => _ClassSetup(testContext);\n");
            _ScenarioTeardown(scenarioName: TestContext.TestName, isTestError: TestContext.CurrentTestOutcome == UnitTestOutcome.Failed);
        }

    }
}
