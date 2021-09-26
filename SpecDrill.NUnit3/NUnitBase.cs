using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using SpecDrill.Infrastructure;
using SpecDrill.Secondary.Adapters.WebDriver;
using SpecDrill.Secondary.Ports.AutomationFramework;
using SpecDrill.Tests;
using System;
using System.Collections.Generic;

namespace SpecDrill.NUnit3
{
    public class NUnitBase : UiScenarioBase
    {
        [OneTimeSetUp]
        public void _ClassSetup()
        {
            try
            {
                DI.ConfigureServices(services =>
                {
                    services.AddWebdriverSecondaryAdapter();
                    return services;
                });
                DI.Apply();
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
            var runtimeServices = DI.ServiceProvider.GetService<IRuntimeServices>();
            if (runtimeServices == null) throw new Exception("IRuntimeServices could not be resoved by DI ServiceProvider");
            _ScenarioSetup(runtimeServices, TestContext.CurrentContext.Test.Name);
        }

        [TearDown]
        public void _TestCleanup()
        {
            _ScenarioTeardown(scenarioName: TestContext.CurrentContext.Test.Name, isTestError: new HashSet<ResultState>(new ResultState[] {ResultState.Failure, ResultState.ChildFailure}).Contains(TestContext.CurrentContext.Result.Outcome));
        }

    }
}