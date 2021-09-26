using Microsoft.Extensions.DependencyInjection;
using SpecDrill.Configuration;
using SpecDrill.Infrastructure;
using SpecDrill.Infrastructure.Configuration;
using SpecDrill.Secondary.Adapters.WebDriver;
using SpecDrill.Secondary.Ports.AutomationFramework;
using SpecDrill.Secondary.Ports.AutomationFramework.Core;
using SpecDrill.Tests;
using System;
using System.Runtime.CompilerServices;
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
            _ScenarioSetup(scenarioContext.ScenarioInfo.Title);
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
           
            _ScenarioSetup(scenarioContext.ScenarioInfo.Title);
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

        [ModuleInitializer]
        public static void AssemblyInitialize()
        {
            DI.ConfigureServices(services =>
            {
                services.AddWebdriverSecondaryAdapter();
                services.AddSingleton<Settings>(ConfigurationManager.Settings);
                services.AddSingleton<IBrowser, Browser>();
            });
            DI.Apply();
            Console.WriteLine("SpecDrill.MsTest Module Initializer - DI Config complete!");
        }
    }
}
