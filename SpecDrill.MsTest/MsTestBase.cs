using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpecDrill.Configuration;
using SpecDrill.Infrastructure;
using SpecDrill.Infrastructure.Configuration;
using SpecDrill.Secondary.Adapters.WebDriver;
using SpecDrill.Secondary.Ports.AutomationFramework.Core;
using SpecDrill.Tests;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static SpecDrill.Secondary.Adapters.WebDriver.Init;
using System.Collections.Concurrent;
using System.Linq;

namespace SpecDrill.MsTest
{
    public class MsTestBase : UiScenarioBase
    {
        protected const string UNNAMED_TEST = "!UnnamedTest!";
        private static ConcurrentDictionary<string, MsTestBase> _instances = new();
        public TestContext? TestContext { get; set; }
        private string? FixtureKey { get; set; }

        public static MsTestBase GetInstance(string fixtureKey)
            => (_instances.TryGetValue(fixtureKey, out MsTestBase? instance)) ?
                    instance :
                    throw new KeyNotFoundException($"Test Fixture {fixtureKey} not found in MsTest instances list!");

        private static bool DidFirstTestRan { get; set; } = false;
        private bool RestartDriverPerTest { get; set; } = false;
        public MsTestBase() : this(true) { }
        public MsTestBase(bool restartDriverPerTest = true)
        {

            FixtureKey = this.GetType().Name;
            this.RestartDriverPerTest = restartDriverPerTest;
            MsTestBase._instances.TryAdd(FixtureKey, this);
        }

        [ClassInitialize]
        public static void _ClassSetup(TestContext testContext)
        {
            //NOTE: here test context is the one corresponding to first test in this class hence only FullyQualifiedTestClassName is relevant
            try
            {
                Logger.LogInformation($"*** _ClassSetup of {testContext.FullyQualifiedTestClassName}");
                ClassSetup();
            }
            catch (Exception e)
            {
                Logger.LogError($"Failed in ClassInitialize for [{testContext.FullyQualifiedTestClassName}] with {e}");
            }
        }

        protected static Action ClassSetup = () => { };

        [ClassCleanup(InheritanceBehavior.BeforeEachDerivedClass)]
        //[ClassCleanup(InheritanceBehavior.BeforeEachDerivedClass)]
        public void _ClassCleanup()
        {
            ClassTeardown();

            _instances.TryGetValue(_instances.Keys.Last(), out MsTestBase? fixtureInstance);



            if (fixtureInstance == null)
            {
                Logger.LogWarning("No tests were run for this fixture!");
                return;
            }
            if (fixtureInstance.TestContext == null)
            {
                Logger.LogError("Last fixture not run. TestContext is null !");
                return;
            }

            if (!fixtureInstance.RestartDriverPerTest)
            {
                Logger.LogInformation($"*** Dispose of LAST: {fixtureInstance.TestContext.FullyQualifiedTestClassName} for LAST:{fixtureInstance.TestContext.TestName}");


                if (DidFirstTestRan)
                {
                    fixtureInstance._ScenarioTeardown(scenarioName: fixtureInstance.TestContext.TestName??UNNAMED_TEST, isTestError: fixtureInstance.TestContext.CurrentTestOutcome == UnitTestOutcome.Failed);
                    DidFirstTestRan = false;
                }
            }
        }
        protected static Action ClassTeardown = () => { };


        [TestInitialize]
        public void _TestSetup()
        {
            if (TestContext == null) throw new Exception("TextContext is not initialized. SpecDrill.MsTest.TestBase.ClassSetup() was not invoked yet!\n Please add following code snippet to your test class:\n[ClassInitialize]\npublic static void ClassInitializer(TestContext testContext) => _ClassSetup(testContext);\n");
            if (RestartDriverPerTest)
                _ScenarioSetup(TestContext.TestName??UNNAMED_TEST);
            else
            {
                if (!DidFirstTestRan)
                {
                    _ScenarioSetup(TestContext.TestName??UNNAMED_TEST);
                    DidFirstTestRan = true;
                }
            }
        }

        [TestCleanup]
        public void _TestCleanup()
        {
            if (TestContext == null) throw new Exception("TextContext is not initialized. SpecDrill.MsTest.TestBase.ClassSetup() was not invoked yet!\n Please add following code snippet to your test class:\n[ClassInitialize]\npublic static void ClassInitializer(TestContext testContext) => _ClassSetup(testContext);\n");
            if (RestartDriverPerTest)
                _ScenarioTeardown(scenarioName: TestContext.TestName??UNNAMED_TEST, isTestError: TestContext.CurrentTestOutcome == UnitTestOutcome.Failed);
        }

#pragma warning disable CA2255 // The 'ModuleInitializer' attribute should not be used in libraries
        [ModuleInitializer]
#pragma warning restore CA2255 // The 'ModuleInitializer' attribute should not be used in libraries
        public static void AssemblyInitialize()
        {
            DI.ConfigureServices(services =>
            {
                services.AddWebdriverSecondaryAdapter();
                services.AddSingleton(sp =>
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
