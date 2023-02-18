using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SpecDrill.Configuration;
using SpecDrill.Infrastructure;
using SpecDrill.Infrastructure.Configuration;
using SpecDrill.Secondary.Adapters.WebDriver;
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
            Logger.LogInformation("SpedFlowBase - ScenarioSetUp");
            if (scenarioContext == null)
                throw new Exception("Please set protected field ScenarioBase.scenarioContext via context injection by adding the following constructor to your [Binding] class:\n public {className}(ScenarioContext scenarioContext, FeatureContext featureContext) => (this.scenarioContext, this.featureContext) = (scenarioContext, featureContext);\n");
            _ScenarioSetup(scenarioContext.ScenarioInfo.Title);
            Logger.LogInformation("end SpedFlowBase - ScenarioSetUp");
        }

        [AfterScenario]
        public void ScenarioTearDown()
        {
            Logger.LogInformation("SpedFlowBase - ScenarioTearDown");
            if (scenarioContext == null)
                throw new Exception("Please set protected field ScenarioBase.scenarioContext via context injection by adding the following constructor to your [Binding] class:\n public {className}(ScenarioContext scenarioContext, FeatureContext featureContext) => (this.scenarioContext, this.featureContext) = (scenarioContext, featureContext);\n");

            _ScenarioTeardown(
                scenarioName: scenarioContext.ScenarioInfo.Title,
                isTestError: scenarioContext.ScenarioExecutionStatus == ScenarioExecutionStatus.TestError);
            Logger.LogInformation("end SpedFlowBase - ScenarioTearDown");
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
            Logger.LogInformation("ScenarioSetUp");
            if (scenarioContext == null)
                throw new Exception("Please set protected field ScenarioBase.scenarioContext via context injection by adding the following constructor to your [Binding] class:\n public {className}(ScenarioContext scenarioContext, FeatureContext featureContext) => (this.scenarioContext, this.featureContext) = (scenarioContext, featureContext);\n");
           
            _ScenarioSetup(scenarioContext.ScenarioInfo.Title);
            Logger.LogInformation("end ScenarioSetUp");
        }

        [AfterScenario]
        public void ScenarioTearDown()
        {
            Logger.LogInformation("ScenarioTearDown");
            if (scenarioContext == null)
                throw new Exception("Please set protected field ScenarioBase.scenarioContext via context injection by adding the following constructor to your [Binding] class:\n public {className}(ScenarioContext scenarioContext, FeatureContext featureContext) => (this.scenarioContext, this.featureContext) = (scenarioContext, featureContext);\n");

            _ScenarioTeardown(
                scenarioName: scenarioContext.ScenarioInfo.Title,
                isTestError: scenarioContext.ScenarioExecutionStatus == ScenarioExecutionStatus.TestError);
            Logger.LogInformation("end ScenarioTearDown");
        }

#pragma warning disable CA2255 // The 'ModuleInitializer' attribute should not be used in libraries
        [ModuleInitializer]
#pragma warning restore CA2255 // The 'ModuleInitializer' attribute should not be used in libraries
        public static void AssemblyInitialize()
        {
            DI.ConfigureServices(services =>
            {
                services.AddWebdriverSecondaryAdapter();
                services.AddSingleton<Settings>(sp =>
                {
                    ConfigurationManager.Load();
                    return ConfigurationManager.Settings;
                });
                services.AddTransient<IBrowser, Browser>();
            });
            DI.Apply();
            Console.WriteLine("SpecDrill.MsTest Module Initializer - DI Config complete!");
        }
    }
}
