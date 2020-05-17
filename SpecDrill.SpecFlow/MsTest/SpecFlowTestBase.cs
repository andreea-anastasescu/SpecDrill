using SpecDrill.Adapters.WebDriver;
using SpecDrill.Infrastructure.Configuration;
using SpecDrill.Infrastructure.Logging.Interfaces;
using SpecDrill.SecondaryPorts.AutomationFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SpecDrill.SpecFlow.MsTest
{
    public class SpecFlowTestBase
    {
        protected static readonly ILogger Log = Infrastructure.Logging.Log.Get<SpecFlowTestBase>();
        private Lazy<IBrowser> LazyBrowser = new Lazy<IBrowser>(() =>
        {
            try
            {
                return new Browser(Runtime.GetServices(), ConfigurationManager.Settings);
            }
            catch (Exception e)
            {
                Log.Log(LogLevel.Fatal, e.Message);
                throw;
            }
        });
        protected IBrowser Browser => LazyBrowser.Value;
        
        [BeforeScenario]
        public void ScenarioSetUp()
        {
            // No use yet.
        }

        [AfterScenario]
        public void ScenarioTearDown()
        {
            if (LazyBrowser.IsValueCreated)
            {
                Browser.Exit();
            }
        }
    }
}
