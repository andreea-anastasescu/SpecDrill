﻿using Microsoft.Extensions.DependencyInjection;
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
        private static bool initialized = false;
        protected static Lazy<IBrowser> LazyBrowser { get; private set; } = new Lazy<IBrowser>(() => throw new Exception("Browser not initialized. _ScenarioSetup was not called yet!"));
        private static void SetInitialized(bool state) => UiScenarioBase.initialized = state;
        protected sealed override void DriverInit()
        {
            LazyBrowser = new Lazy<IBrowser>(() => DI.ServiceProvider.GetService<IBrowser>() ?? throw new Exception("IBrowser could not be resolved by DI ServiceProvider"));
            SetInitialized(true);
        }
        protected sealed override void DriverInitRecovery(Exception e)
        {
            if (!initialized)
                return;
            if (LazyBrowser.IsValueCreated)
            {
                Browser.Exit();
                LazyBrowser = new Lazy<IBrowser>(() => throw new Exception("Browser already exited. _ScenarioSetup failed!"));
                SetInitialized(false);
            }
            throw new Exception("Browser initialization exception!", e);
        }
        protected sealed override void DriverTeardown(string scenarioName, bool isTestError)
        {
            if (!initialized)
                return;
            if (isTestError &&
                  (ConfigurationManager.Settings?.WebDriver?.Screenshots?.Auto ?? false))
            {
                SaveScreenshot(scenarioName);
            }
            Browser?.Exit();
            
            
            LazyBrowser = new Lazy<IBrowser>(() => throw new Exception("Browser already exited. _ScenarioTeardown was called!"));
            SetInitialized(false);
        }
        protected sealed override void DriverTeardownRecovery(Exception e)
        {
            Logger.Log(LogLevel.Error, "Failed in DriverTeardown with {exception}", e);
        }
        protected void SaveScreenshot(string scenarioName) => Browser.SaveScreenshot(this.GetType().Name, scenarioName);
    }
    public class ScenarioBase
    {
        protected static readonly ILogger Logger = DI.GetLogger<ScenarioBase>();
        protected virtual void DriverInit() { }
        protected virtual void DriverInitRecovery(Exception e) { }
        protected void _ScenarioSetup(string scenarioName)
        {
            try
            {
                DriverInit();
                ScenarioSetup();
            }
            catch (Exception e)
            {
                Logger.LogError("Failed in TestInitialize for scenario: {scenarioName} with {exception}", scenarioName, e);
                DriverInitRecovery(e);
                return;
            }
        }

        protected virtual void ScenarioSetup() { }

        protected void _ScenarioTeardown(string scenarioName, bool isTestError)
        {
            try
            {
                Logger.Log(LogLevel.Information, "Cleaning up after {scenarioName} scenario.", scenarioName);

                ScenarioTeardown(scenarioName, isTestError);

            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, "Failed in ScenarioCleanup for scenario: {scenarioName} with {exception}", scenarioName, e);
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
