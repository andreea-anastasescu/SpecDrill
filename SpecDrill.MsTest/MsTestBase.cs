using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpecDrill.Infrastructure.Configuration;
using SpecDrill.Infrastructure.Logging.Interfaces;
using SpecDrill.SecondaryPorts.Adapters.WebDriver;
using SpecDrill.Tests;
using System;

namespace SpecDrill.MsTest
{
    public class MsTestBase : ScenarioBase
    {
        object syncRoot = new object();
        protected static TestContext? TestContext;
        [ClassInitialize]
        public static void _ClassSetup(TestContext testContext)
        {
            try
            {
                TestContext = testContext;
                ClassSetup();
            }
            catch (Exception e)
            {
                Log.Log(LogLevel.Error, $"Failed in ClassInitialize for test method [{testContext.TestName}] with {e}");
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
            _ScenarioSetup(Runtime.GetServices(), TestContext.TestName);
        }

        [TestCleanup]
        public void _TestCleanup()
        {
            if (TestContext == null) throw new Exception("TextContext is not initialized. SpecDrill.MsTest.TestBase.ClassSetup() was not invoked yet!\n Please add following code snippet to your test class:\n[ClassInitialize]\npublic static void ClassInitializer(TestContext testContext) => _ClassSetup(testContext);\n");
            _ScenarioTeardown(scenarioName: TestContext.TestName, isTestError : TestContext.CurrentTestOutcome == UnitTestOutcome.Failed);
        }

    }
}
