using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpecDrill.Adapters.WebDriver;
using SpecDrill.Infrastructure;
using SpecDrill.Infrastructure.Configuration;
using SpecDrill.Infrastructure.Logging.Interfaces;
using SpecDrill.SecondaryPorts.AutomationFramework.Core;
using System;
using System.Diagnostics;

namespace SpecDrill.MsTest
{
    public class TestBase
    {
        protected static ILogger Log = Infrastructure.Logging.Log.Get<TestBase>();
        private IBrowser browser;

        public IBrowser Browser => browser;
        [ClassInitialize]
        public static void _ClassSetup(TestContext testContext)
        {
            try
            {
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
            ClassCleanup();
        }
        protected static Action ClassCleanup = () => { };


        [TestInitialize]
        public void _TestSetup()
        {
            try
            {
                browser = new Browser(Runtime.GetServices(), ConfigurationManager.Settings);
                TestSetup();
            }
            catch (Exception e)
            {
                Log.Log(LogLevel.Error, $"Failed in TestInitialize for {TestContext.TestName} with {e}");
                if (Browser != null)
                    Browser.Exit();
                throw new Exception("Browser initialization exception!", e);
            }
        }

        public TestContext TestContext { get; set; }

        [TestCleanup]
        public void _TestCleanup()
        {
            try
            {
                if (TestContext.CurrentTestOutcome == UnitTestOutcome.Failed &&
                    (ConfigurationManager.Settings.WebDriver.Screenshots.Auto ?? false))
                {
                    SaveScreenshot();
                }

                TestCleanup();
            }
            catch (Exception e)
            {
                Log.Log(LogLevel.Error, $"Failed in TestCleanup for {TestContext.TestName} with {e}");
            }

            Browser?.Exit();
        }

        public virtual void TestSetup()
        {
           
        }

        public virtual void TestCleanup()
        {
        }

        public void SaveScreenshot() => Browser.SaveScreenshot(this.GetType().Name, TestContext.TestName);
    }
}
