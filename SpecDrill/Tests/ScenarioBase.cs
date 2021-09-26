using Microsoft.Extensions.Logging;
using SpecDrill.Infrastructure;
using SpecDrill.Infrastructure.Configuration;
using SpecDrill.Secondary.Ports.AutomationFramework;
using SpecDrill.Secondary.Ports.AutomationFramework.Core;
using System;

namespace SpecDrill.Tests
{
    public class UiScenarioBase : ScenarioBase
    {
        protected IBrowser Browser => LazyBrowser.Value;
        protected Lazy<IBrowser> LazyBrowser { get; private set; } = new Lazy<IBrowser>(() => throw new Exception("Browser not initialized. _ScenarioSetup was not called yet!"));
        protected sealed override void DriverInit(IRuntimeServices services)
        {
            LazyBrowser = new Lazy<IBrowser>(() =>
            {
                try
                {
                    return new Browser(services, ConfigurationManager.Settings);
                }
                catch (Exception e)
                {
                    Logger.LogCritical(e.Message);
                    throw;
                }
            });
        }
        protected sealed override void DriverInitRecovery(Exception e)
        {
            if (LazyBrowser.IsValueCreated)
            {
                Browser.Exit();
                LazyBrowser = new Lazy<IBrowser>(() => throw new Exception("Browser already exited. _ScenarioSetup failed!"));
            }
            throw new Exception("Browser initialization exception!", e);
        }
        protected sealed override void DriverTeardown(string scenarioName, bool isTestError)
        {
            if (isTestError &&
                  (ConfigurationManager.Settings?.WebDriver?.Screenshots?.Auto ?? false))
            {
                SaveScreenshot(scenarioName);
            }
            Browser?.Exit();
            LazyBrowser = new Lazy<IBrowser>(() => throw new Exception("Browser already exited. _ScenarioTeardown was called!"));
        }
        protected sealed override void DriverTeardownRecovery(Exception e)
        {
            Logger.Log(LogLevel.Error, $"Failed in DriverTeardown with {e}");
        }
        protected void SaveScreenshot(string scenarioName) => Browser.SaveScreenshot(this.GetType().Name, scenarioName);
    }
    public class ScenarioBase
    {
        protected static readonly ILogger Logger = DI.GetLogger<ScenarioBase>();
        protected virtual void DriverInit(IRuntimeServices services) { }
        protected virtual void DriverInitRecovery(Exception e) { }
        protected void _ScenarioSetup(IRuntimeServices services, string scenarioName)
        {
            try
            {
                DriverInit(services);
                ScenarioSetup();
            }
            catch (Exception e)
            {
                Logger.LogError($"Failed in TestInitialize for scenario: {scenarioName} with {e}");
                DriverInitRecovery(e);
                return;
            }
        }

        protected virtual void ScenarioSetup() { }
        
        protected void _ScenarioTeardown(string scenarioName, bool isTestError)
        {
            try
            {
                Logger.Log(LogLevel.Information, $"Cleaning up after {scenarioName} scenario.");

                ScenarioTeardown(scenarioName, isTestError);
               
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, $"Failed in ScenarioCleanup for scenario: {scenarioName} with {e}");
            }
            finally
            {
                try
                {
                    DriverTeardown(scenarioName, isTestError);
                }
                catch (Exception e)
                {
                    DriverTeardownRecovery(e);
                }
            }

            
        }

        protected virtual void DriverTeardown(string scenarioName, bool isTestError) { }
        protected virtual void DriverTeardownRecovery(Exception e) { }
        protected virtual void ScenarioTeardown(string scenarioName, bool isTestError) { }
    }
}
