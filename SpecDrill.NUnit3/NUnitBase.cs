using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using SpecDrill.Configuration;
using SpecDrill.Infrastructure;
using SpecDrill.Infrastructure.Configuration;
using SpecDrill.Secondary.Adapters.WebDriver;
using SpecDrill.Secondary.Ports.AutomationFramework.Core;
using SpecDrill.Tests;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SpecDrill.NUnit3
{
    public class NUnitBase : UiScenarioBase
    {
        private static NUnitBase? LastRun { get; set; }
        public NUnitBase() : this(false) { }
        private bool RestartDriverPerTest { get; set; } = false;
        private bool DidFirstTestRan { get; set; } = false;
        public NUnitBase(bool restartDriverPerTest = false)
        {
            RestartDriverPerTest = restartDriverPerTest;
        }
        [OneTimeSetUp]
        public void _ClassSetup()
        {
            try
            {
                ClassSetup();
                if (!RestartDriverPerTest)
                    _ScenarioSetup(TestContext.CurrentContext.Test.Name);

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
            LastRun?._ScenarioTeardown(scenarioName: TestContext.CurrentContext.Test.Name, isTestError: new HashSet<ResultState>(new ResultState[] { ResultState.Failure, ResultState.ChildFailure }).Contains(TestContext.CurrentContext.Result.Outcome));
        }
        protected static Action ClassTeardown = () => { };


        [SetUp]
        public void _TestSetup()
        {
            LastRun = this;
            if (RestartDriverPerTest)
                _ScenarioSetup(TestContext.CurrentContext.Test.Name);
            else
            {
                if (!DidFirstTestRan)
                {
                    _ScenarioSetup(TestContext.CurrentContext.Test.Name);
                    DidFirstTestRan = true;
                }
            }
        }

        [TearDown]
        public void _TestCleanup()
        {
            if (RestartDriverPerTest)
            _ScenarioTeardown(scenarioName: TestContext.CurrentContext.Test.Name, isTestError: new HashSet<ResultState>(new ResultState[] {ResultState.Failure, ResultState.ChildFailure}).Contains(TestContext.CurrentContext.Result.Outcome));
        }

        [ModuleInitializer]
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