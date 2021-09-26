using Microsoft.Extensions.DependencyInjection;
using SpecDrill.Infrastructure;
using SpecDrill.Secondary.Adapters.WebDriver;
using SpecDrill.Secondary.Ports.AutomationFramework;
using SpecDrill.Tests;
using System;
using TechTalk.SpecFlow;

namespace SpecDrill.SpecFlow
{
    public class SpecFlowBase : ScenarioBase
    {
        protected ScenarioContext scenarioContext;
        protected FeatureContext featureContext;

        public SpecFlowBase(ScenarioContext scenarioContext, FeatureContext featureContext)
            => (this.scenarioContext, this.featureContext) = (scenarioContext, featureContext);

        [BeforeScenario]
        public void ScenarioSetUp()
        {
            if (scenarioContext == null)
                throw new Exception("Please set protected field ScenarioBase.scenarioContext via context injection by adding the following constructor to your [Binding] class:\n public {className}(ScenarioContext scenarioContext, FeatureContext featureContext) => (this.scenarioContext, this.featureContext) = (scenarioContext, featureContext);\n");
            var runtimeServices = DI.ServiceProvider.GetService<IRuntimeServices>();
            if (runtimeServices == null) throw new Exception("IRuntimeServices could not be resoved by DI ServiceProvider");
            _ScenarioSetup(runtimeServices, scenarioContext.ScenarioInfo.Title);
        }

        [AfterScenario]
        public void ScenarioTearDown()
        {
            if (scenarioContext == null)
                throw new Exception("Please set protected field ScenarioBase.scenarioContext via context injection by adding the following constructor to your [Binding] class:\n public {className}(ScenarioContext scenarioContext, FeatureContext featureContext) => (this.scenarioContext, this.featureContext) = (scenarioContext, featureContext);\n");

            _ScenarioTeardown(
                scenarioName: scenarioContext.ScenarioInfo.Title,
                isTestError: scenarioContext.ScenarioExecutionStatus == ScenarioExecutionStatus.TestError);
        }
    }

    public class UiSpecFlowBase : UiScenarioBase
    {
        protected ScenarioContext scenarioContext;
        protected FeatureContext? featureContext;

        public UiSpecFlowBase(ScenarioContext scenarioContext, FeatureContext featureContext)
            => (this.scenarioContext, this.featureContext) = (scenarioContext, featureContext);

        [BeforeScenario]
        public void ScenarioSetUp()
        {
            if (scenarioContext == null)
                throw new Exception("Please set protected field ScenarioBase.scenarioContext via context injection by adding the following constructor to your [Binding] class:\n public {className}(ScenarioContext scenarioContext, FeatureContext featureContext) => (this.scenarioContext, this.featureContext) = (scenarioContext, featureContext);\n");
            DI.ConfigureServices(services =>
            {
                services.AddWebdriverSecondaryAdapter();
            });
            DI.Apply();
            var runtimeServices = DI.ServiceProvider.GetService<IRuntimeServices>();
            if (runtimeServices == null) throw new Exception("IRuntimeServices could not be resoved by DI ServiceProvider");
            _ScenarioSetup(runtimeServices, scenarioContext.ScenarioInfo.Title);
        }

        [AfterScenario]
        public void ScenarioTearDown()
        {
            if (scenarioContext == null)
                throw new Exception("Please set protected field ScenarioBase.scenarioContext via context injection by adding the following constructor to your [Binding] class:\n public {className}(ScenarioContext scenarioContext, FeatureContext featureContext) => (this.scenarioContext, this.featureContext) = (scenarioContext, featureContext);\n");

            _ScenarioTeardown(
                scenarioName: scenarioContext.ScenarioInfo.Title,
                isTestError: scenarioContext.ScenarioExecutionStatus == ScenarioExecutionStatus.TestError);
        }
    }
}
