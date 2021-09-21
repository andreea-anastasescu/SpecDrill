using NUnit.Framework;
using NUnit.Framework.Interfaces;
using SpecDrill.Infrastructure.Logging.Interfaces;
using SpecDrill.Secondary.Adapters.WebDriver;
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
                ClassSetup();
            }
            catch (Exception e)
            {
                Log.Log(LogLevel.Error, $"Failed in ClassInitialize for test method [{TestContext.CurrentContext.Test.Name}] with {e}");
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
            _ScenarioSetup(Runtime.GetServices(), TestContext.CurrentContext.Test.Name);
        }

        [TearDown]
        public void _TestCleanup()
        {
            _ScenarioTeardown(scenarioName: TestContext.CurrentContext.Test.Name, isTestError: new HashSet<ResultState>(new ResultState[] {ResultState.Failure, ResultState.ChildFailure}).Contains(TestContext.CurrentContext.Result.Outcome));
        }

    }
}