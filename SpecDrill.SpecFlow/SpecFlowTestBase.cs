using SpecDrill.SecondaryPorts.Adapters.WebDriver;
using SpecDrill.Infrastructure.Configuration;
using SpecDrill.Infrastructure.Logging.Interfaces;
using SpecDrill.SecondaryPorts.AutomationFramework.Core;
using SpecDrill.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SpecDrill.SpecFlow
{
    public class SpecFlowBase : ScenarioBase
    {
        protected ScenarioContext? scenarioContext;
        protected FeatureContext? featureContext;

        [BeforeScenario]
        public void ScenarioSetUp()
        {
            if (scenarioContext == null)
                throw new Exception("Please set protected field ScenarioBase.scenarioContext via context injection by adding the following constructor to your [Binding] class:\n public {className}(ScenarioContext scenarioContext, FeatureContext featureContext) => (this.scenarioContext, this.featureContext) = (scenarioContext, featureContext);\n");

            _ScenarioSetup(Runtime.GetServices(), scenarioContext.ScenarioInfo.Title);
        }

        [AfterScenario]
        public void ScenarioTearDown()
        {
            if (scenarioContext == null)
                throw new Exception("Please set protected field ScenarioBase.scenarioContext via context injection by adding the following constructor to your [Binding] class:\n public {className}(ScenarioContext scenarioContext, FeatureContext featureContext) => (this.scenarioContext, this.featureContext) = (scenarioContext, featureContext);\n");

            _ScenarioTeardown(
                scenarioName: scenarioContext.ScenarioInfo.Title, 
                isTestError : scenarioContext.ScenarioExecutionStatus == ScenarioExecutionStatus.TestError);
        }
    }
}
