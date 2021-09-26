using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using SpecDrill.Configuration;
using SpecDrill.Infrastructure;
using SpecDrill.Infrastructure.Configuration;
using SpecDrill.Secondary.Adapters.WebDriver;
using SpecDrill.Secondary.Ports.AutomationFramework;
using SpecDrill.Secondary.Ports.AutomationFramework.Core;
using SpecDrill.Tests;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SpecDrill.NUnit3
{
    public class NUnitBase : UiScenarioBase
    {
        [OneTimeSetUp]
        public void _ClassSetup()
        {
            try
            {
                ClassSetup();
            }
            catch (Exception e)
            {
                Logger.LogError($"Failed in ClassInitialize for test method [{TestContext.CurrentContext.Test.Name}] with {e}");
            }
        }
        protected static Action ClassSetup = () => { };


        [OneTimeTearDown]
        public static void _ClassCleanup()
        {
            ClassTeardown();
        }
        protected static Action ClassTeardown = () => { };


        [SetUp]
        public void _TestSetup()
        {
            _ScenarioSetup(TestContext.CurrentContext.Test.Name);
        }

        [TearDown]
        public void _TestCleanup()
        {
            _ScenarioTeardown(scenarioName: TestContext.CurrentContext.Test.Name, isTestError: new HashSet<ResultState>(new ResultState[] {ResultState.Failure, ResultState.ChildFailure}).Contains(TestContext.CurrentContext.Result.Outcome));
        }

        [ModuleInitializer]
        public static void AssemblyInitialize()
        {
            Debugger.Break();
            DI.ConfigureServices(services =>
            {
                services.AddWebdriverSecondaryAdapter();
                services.AddSingleton<Settings>(sp => {
                    ConfigurationManager.Load();
                    return ConfigurationManager.Settings;
                    });
                services.AddSingleton<IBrowser, Browser>();
            });
            DI.Apply();
            Console.WriteLine("SpecDrill.NUnit3 Module Initializer - DI Config complete!");
        }


    }
}