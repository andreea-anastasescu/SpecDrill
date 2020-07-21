using SpecDrill.Infrastructure.Configuration;
using SpecDrill.Infrastructure.Logging.Interfaces;
using SpecDrill.SecondaryPorts.AutomationFramework;
using SpecDrill.SecondaryPorts.AutomationFramework.Core;
using System;

namespace SpecDrill.Tests
{
    public class ScenarioBase
    {

        protected static readonly ILogger Log = Infrastructure.Logging.Log.Get<ScenarioBase>();
        protected IBrowser Browser => LazyBrowser.Value;

        protected Lazy<IBrowser> LazyBrowser { get; private set; } = new Lazy<IBrowser>(() => throw new Exception("Browser not initialized. _ScenarioSetup was not called yet!"));


        protected void _ScenarioSetup(IRuntimeServices services, string scenarioName)
        {
            try
            {
                LazyBrowser = new Lazy<IBrowser>(() =>
                {
                    try
                    {
                        return new Browser(services, ConfigurationManager.Settings);
                    }
                    catch (Exception e)
                    {
                        Log.Log(LogLevel.Fatal, e.Message);
                        throw;
                    }
                });
                ScenarioSetup();
            }
            catch (Exception e)
            {
                Log.Log(LogLevel.Error, $"Failed in TestInitialize for scenario: {scenarioName} with {e}");
                if (LazyBrowser.IsValueCreated)
                {
                    Browser.Exit();
                }
                throw new Exception("Browser initialization exception!", e);
            }
        }
        protected virtual void ScenarioSetup()
        {

        }
        protected void _ScenarioTeardown(string scenarioName, bool isTestError)
        {
            try
            {
                Log.Log(LogLevel.Info, $"Cleaning up after {scenarioName} scenario.");
                if (isTestError &&
                    (ConfigurationManager.Settings?.WebDriver?.Screenshots?.Auto ?? false))
                {
                    SaveScreenshot(scenarioName);
                }

                ScenarioTeardown();
            }
            catch (Exception e)
            {
                Log.Log(LogLevel.Error, $"Failed in ScenarioCleanup for scenario: {scenarioName} with {e}");
            }

            Browser?.Exit();
        }


        protected virtual void ScenarioTeardown()
        {
        }

        protected void SaveScreenshot(string scenarioName) => Browser.SaveScreenshot(this.GetType().Name, scenarioName);
    }
}
