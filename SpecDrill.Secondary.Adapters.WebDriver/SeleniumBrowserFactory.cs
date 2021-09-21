using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Opera;
//using OpenQA.Selenium.Appium.Android;
//using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Safari;
using SpecDrill.Configuration;
using SpecDrill.Infrastructure;
using SpecDrill.Infrastructure.Enums;
using SpecDrill.Infrastructure.Logging;
using SpecDrill.Infrastructure.Logging.Interfaces;
using SpecDrill.Secondary.Ports.AutomationFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
//using OpenQA.Selenium.Appium.Enums;

namespace SpecDrill.Secondary.Adapters.WebDriver
{
    internal static class DesiredCapabilitiesExtensions
    {
        private static ILogger Log = Infrastructure.Logging.Log.Get(typeof(DesiredCapabilitiesExtensions));

        public static bool ValidateCapability(string key, object value, Type enumType)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                Log.Info($"Cannot Add Capability (key is null or whitespace)");
                return false;
            }
            if (value == null)
            {
                Log.Info($"Cannot add capability {key}. Its value is null.");
                return false;
            }

            var stringValue = value as string;

            if (stringValue != null && string.IsNullOrWhiteSpace(stringValue))
            {
                Log.Info($"Cannot add capability {key}. Its value is empty.");
                return false;
            }

            if (stringValue != null)
            {
                if (!enumType.IsEnum)
                {
                    Log.Info($"Expected `{enumType}` to be an enum.");
                }

                if (!stringValue.OfEnum(enumType))
                {
                    Log.Info($"Cannot add capability {key}. Its value is not in [{string.Join(", ", Enum.GetNames(enumType))}].");
                    return false;
                }
            }
            return true;
        }
        //public static DesiredCapabilities AddCapability(this DesiredCapabilities capabilities, string key, object value, Type enumType = null)
        //{
        //    if (ValidateCapability(key, value, enumType))
        //    {
        //        capabilities.SetCapability(key, value);
        //    }
        //    return capabilities;
        //}

        //public static DriverOptions AddCapability(this DriverOptions driverOptions, string key, object value, Type enumType = null)
        //{
        //    if (ValidateCapability(key, value, enumType))
        //    {
        //        driverOptions.AddAdditionalCapability(key, value);
        //    }
        //    return driverOptions;
        //}
    }
    internal class SeleniumBrowserFactory : IBrowserDriverFactory
    {
        private static ILogger Log = Infrastructure.Logging.Log.Get<SeleniumBrowserFactory>();
        private readonly Dictionary<BrowserNames, Func<IBrowserDriver>> driverFactory;

        private readonly Settings? configuration = null;
        public SeleniumBrowserFactory(Settings configuration)
        {
            var aPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            Log.Info($"Assembly path = {aPath}");
            this.configuration = configuration;
            driverFactory = new Dictionary<BrowserNames, Func<IBrowserDriver>>
            {
                { BrowserNames.chrome, () =>
                {
                    Log.Info("Initializig Chrome driver...");
                    var bdp = GetBrowserDriversPath(configuration?.WebDriver?.Browser?.Drivers?.Chrome?.Path ?? "");
                    return SeleniumBrowserDriver.Create(new ChromeDriver(bdp, BuildChromeOptions()), this.configuration);
                }},
                { BrowserNames.ie, () =>
                {
                    Log.Info("Initializig IE driver...");
                    return SeleniumBrowserDriver.Create(new InternetExplorerDriver(GetBrowserDriversPath(configuration?.WebDriver?.Browser?.Drivers?.Ie?.Path ?? ""), BuildInternetExplorerOptions()), this.configuration);
                    } },
                { BrowserNames.edge, () =>
                {
                    Log.Info("Initializig Edge driver...");
                    return SeleniumBrowserDriver.Create(new EdgeDriver(GetBrowserDriversPath(configuration?.WebDriver?.Browser?.Drivers?.Edge?.Path ?? ""), BuildEdgeOptions()), this.configuration);
                    } },
                { BrowserNames.firefox, () =>
                    {
                        Log.Info("Initializig Firefox driver...");
                        
                        var binPath = configuration?.WebDriver?.Browser?.Drivers?.Firefox?.BrowserBinaryPath ?? "";
                        var fds = FirefoxDriverService.CreateDefaultService(configuration?.WebDriver?.Browser?.Drivers?.Firefox?.Path ?? "");

                        if (!string.IsNullOrWhiteSpace(binPath))
                        {
                            fds.FirefoxBinaryPath = binPath;
                        }

                        CodePagesEncodingProvider.Instance.GetEncoding(437);
                        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                        return SeleniumBrowserDriver.Create(new FirefoxDriver(fds, BuildFirefoxOptions(), TimeSpan.FromSeconds(60)), this.configuration);
                    } },
                { BrowserNames.opera, () => {
                    Log.Info("Initializig Opera driver...");
                    return SeleniumBrowserDriver.Create(new OperaDriver(configuration?.WebDriver?.Browser?.Drivers?.Opera?.Path ?? "", BuildOperaOptions()), this.configuration);
                } },
                { BrowserNames.safari, () =>
                {
                    Log.Info("Initializig Safari driver...");
                    return SeleniumBrowserDriver.Create(new SafariDriver(configuration?.WebDriver?.Browser?.Drivers?.Safari?.Path ?? "", BuildSafariOptions()), this.configuration);
                } }
            };
        }

        public IBrowserDriver Create(BrowserNames browserName)
        {
            var mode = (configuration?.WebDriver?.Mode ?? "").ToEnum<Modes>();
            Log.Info($"Browser mode:{mode}");
            switch (mode)
            {
                case Modes.browser:
                    var isRemote = configuration?.WebDriver?.Browser?.IsRemote ?? false;
                    Log.Info($"WebDriver.IsRemote = {isRemote}");
                    if (isRemote)
                    {
                        switch (browserName)
                        {
                            case BrowserNames.chrome:
                                {
                                    ChromeOptions chromeOptions = BuildChromeOptions();
                                    return CreateRemoteWebDriver(chromeOptions.ToCapabilities());
                                }
                            case BrowserNames.firefox:
                                {
                                    CodePagesEncodingProvider.Instance.GetEncoding(437);
                                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                                    FirefoxOptions firefoxOptions = BuildFirefoxOptions();
                                    return CreateRemoteWebDriver(firefoxOptions.ToCapabilities());
                                }
                            case BrowserNames.opera:
                                {
                                    OperaOptions operaOptions = BuildOperaOptions();
                                    return CreateRemoteWebDriver(operaOptions.ToCapabilities());
                                }
                            case BrowserNames.safari:
                                {
                                    SafariOptions safariOptions = BuildSafariOptions();
                                    return CreateRemoteWebDriver(safariOptions.ToCapabilities());
                                }
                            case BrowserNames.ie:
                                {
                                    InternetExplorerOptions ieOptions = BuildInternetExplorerOptions();
                                    return CreateRemoteWebDriver(ieOptions.ToCapabilities());
                                }
                            case BrowserNames.edge:
                                {
                                    EdgeOptions edgeOptions = BuildEdgeOptions();
                                    return CreateRemoteWebDriver(edgeOptions.ToCapabilities());
                                }
                            default:
                                throw new ArgumentOutOfRangeException($"SpecDrill: Value Not Supported `{browserName}`!");
                        }
                    }
                    return driverFactory[browserName]();
                case Modes.appium:

                    AppiumOptions appiumOptions = new AppiumOptions();
                    var configuredCapabilities = configuration?.WebDriver?.Appium?.Capabilities;
                    ExtendCapabilities(appiumOptions, configuredCapabilities);

                    IWebDriver driver;
                    const string PLATFORM_NAME = "platformName";
                    EnsureCapabilityIsConfigured(configuredCapabilities, PLATFORM_NAME);
                    var appiumServerUri = configuration?.WebDriver?.Appium?.ServerUri ?? "";
                    switch (configuredCapabilities![PLATFORM_NAME].ToString().ToEnum<PlatformNames>())
                    {
                        //case PlatformNames.Android:
                        //    driver = new AndroidDriver<AndroidElement>(new Uri(appiumServerUri), appiumOptions);
                        //    return SeleniumBrowserDriver.Create(driver, this.configuration);
                        //case PlatformNames.iOS:
                        //    driver = new IOSDriver<AndroidElement>(new Uri(appiumServerUri), appiumOptions);
                        //    return SeleniumBrowserDriver.Create(driver, this.configuration);
                        default:
                            driver = new RemoteWebDriver(new Uri(appiumServerUri), appiumOptions);
                            return SeleniumBrowserDriver.Create(driver, this.configuration);
                    }
            }
            throw new Exception($"Browser mode `{mode}` not supported!");
        }

        private string GetBrowserDriversPath(string driverPath)
        {
            driverPath = Environment.ExpandEnvironmentVariables(driverPath);
            bool isUnixPath = driverPath.Contains("/");
            bool isWindowsRelativePath = !driverPath.Contains(":\\");
            Console.WriteLine($"isUnixPath = {isUnixPath}, isWindowsRelativePath = {isWindowsRelativePath}");
            Console.WriteLine($"Driver Path = {driverPath}");
            Log.Info($"isUnixPath = {isUnixPath}, isWindowsRelativePath = {isWindowsRelativePath}");
            Log.Info($"Driver Path = {driverPath}");

            if (!isUnixPath && isWindowsRelativePath)
            {
                var currentPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                return Path.Combine(currentPath,driverPath);
            }

            return driverPath;
        
        }

        public IBrowserDriver CreateRemoteWebDriver(ICapabilities desiredCapabilities)
        {
            return SeleniumBrowserDriver.Create(
                            new RemoteWebDriver(
                                new Uri(configuration?.WebDriver?.Browser?.SeleniumServerUri ?? ""), desiredCapabilities), this.configuration);
        }

        #region Capabilities Utility Methods
        private void EnsureCapabilityIsConfigured(Dictionary<string, object>? configCapabilities, string CAPABILITY_NAME)
        {
            if (configCapabilities == null)
            {
                var message = $"Configuration section webdriver/browser/options is missing from specDrillConfig.json !";
                Log.Warning(message);
                throw new Exception(message);
            }
            if (!configCapabilities.ContainsKey(CAPABILITY_NAME) || configCapabilities[CAPABILITY_NAME] == null || string.IsNullOrEmpty(configCapabilities[CAPABILITY_NAME].ToString()))
                throw new MissingMemberException($"SpecDrill: Value Not Present `{CAPABILITY_NAME}`!");
        }

        private void ExtendCapabilities<T>(T options, Dictionary<string, object>? configuredCapabilities, bool forceGlobal = true)
            where T : DriverOptions
        {
            if (configuredCapabilities == null)
            {
                Log.Warning($"Configuration section webdriver/browser/options is missing from specDrillConfig.json !");
                return;
            }
            foreach (var kvp in configuredCapabilities)
            {
                try
                {
                    if (forceGlobal) 
                    {
                        var type = typeof(T);
                        var addAdditionalCapabilityMethodInfo = type.GetMethod("AddAdditionalCapability",
                            new Type[] { typeof(string), typeof(object), typeof(bool) });
                        if (addAdditionalCapabilityMethodInfo == null) throw new InvalidCastException($"Type {type.Name} does not have a definition for AddAdditionalCapability(string, object, bool) !");
                        addAdditionalCapabilityMethodInfo.Invoke(options, new object[] { kvp.Key, kvp.Value.ToString(), true });
                    } 
                    else 
                    {
                        options.AddAdditionalCapability(kvp.Key, kvp.Value.ToString());
                    }
                }
                catch (ArgumentException)
                {
                    Log.Info($"Key {kvp.Key}={options.ToCapabilities().GetCapability(kvp.Key)} already exists. Dropping version with value {kvp.Value}");
                }
                catch (Exception)
                {
                    Log.Info($"Error adding Key {kvp.Key} with value {kvp.Value}");
                    throw;
                }
            }
        }
        #endregion

        #region Option Builders
        private SafariOptions BuildSafariOptions()
        {
            var safariOptions = new SafariOptions();
            ExtendCapabilities(safariOptions, configuration?.WebDriver?.Browser?.Capabilities, forceGlobal: false);
            return safariOptions;
        }

        private EdgeOptions BuildEdgeOptions()
        {
            var edgeOptions = new EdgeOptions();
            var edgeCommandLineArguments = configuration?.WebDriver?.Browser?.Drivers?.Edge?.Arguments;
            if (edgeCommandLineArguments != null && edgeCommandLineArguments.Count > 0)
            {
                Log.Warning($"Specified command line argument(s) for Edge were igonred !");
            }
            ExtendCapabilities(edgeOptions, configuration?.WebDriver?.Browser?.Capabilities, forceGlobal: false);
            return edgeOptions;
        }

        private InternetExplorerOptions BuildInternetExplorerOptions()
        {
            var ieOptions = new InternetExplorerOptions();
            ieOptions.BrowserCommandLineArguments = string.Join(" ", configuration?.WebDriver?.Browser?.Drivers?.Ie?.Arguments ?? new List<string>());
            ieOptions.ForceCreateProcessApi = !string.IsNullOrWhiteSpace(ieOptions.BrowserCommandLineArguments);
            ExtendCapabilities(ieOptions, configuration?.WebDriver?.Browser?.Capabilities);
            return ieOptions;
        }

        private OperaOptions BuildOperaOptions()
        {
            var options = new OperaOptions();
            options.AddArguments(configuration?.WebDriver?.Browser?.Drivers?.Opera?.Arguments ?? new List<string>());
            ExtendCapabilities(options, configuration?.WebDriver?.Browser?.Capabilities);
            return options;
        }

        private FirefoxOptions BuildFirefoxOptions()
        {
            var ffOptions = new FirefoxOptions();
            ffOptions.AddArguments(configuration?.WebDriver?.Browser?.Drivers?.Firefox?.Arguments ?? new List<string>());
            var fp = ffOptions.Profile ?? new FirefoxProfile();
           
            //fp.AcceptUntrustedCertificates = true;
            //fp.AssumeUntrustedCertificateIssuer = false;
            ffOptions.Profile = fp;
            var binPath = configuration?.WebDriver?.Browser?.Drivers?.Firefox?.BrowserBinaryPath;
            if (!string.IsNullOrWhiteSpace(binPath))
            {
                ffOptions.BrowserExecutableLocation = binPath;
            }
            ExtendCapabilities(ffOptions, configuration?.WebDriver?.Browser?.Capabilities);
            return ffOptions;
        }

        private ChromeOptions BuildChromeOptions()
        {
            Log.Info("Building ChromeOptions.");
            var chromeOptions = new ChromeOptions();
            var driverArguments = configuration?.WebDriver?.Browser?.Drivers?.Chrome?.Arguments ?? new List<string>();
            chromeOptions.AddArguments(driverArguments);
            Log.Info($@"configuration.WebDriver.Browser.Drivers.Chrome.Arguments: {(driverArguments.Any() ? 
                driverArguments.Aggregate((a, b) => $"{a} {b}") : 
                string.Empty)}");
            chromeOptions.AddArgument($"window-size={configuration?.WebDriver?.Browser?.Window?.InitialWidth},{configuration?.WebDriver?.Browser?.Window?.InitialHeight}");

            ExtendCapabilities(chromeOptions, configuration?.WebDriver?.Browser?.Capabilities);

            return chromeOptions;
        }
        #endregion
    }
}
