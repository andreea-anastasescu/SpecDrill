using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpecDrill.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpecDrill.Infrastructure.Configuration;
using FluentAssertions;

namespace SpecDrill.Tests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void ShouldHaveCorrectValuesWhenReadingJsonConfigurationFile()
        {
            var configuration = ConfigurationManager.Load(JsonConfigurationFileContents);
            Assert.IsNotNull(configuration);
            Assert.IsNotNull(configuration.WebDriver);
            Assert.AreEqual("chrome", configuration.WebDriver.Browser.BrowserName);
            Assert.AreEqual(60000, configuration.WebDriver.MaxWait);
            Assert.AreEqual(12, configuration.Homepages.Length);
            Assert.AreEqual("Test000LoginPage", configuration.Homepages[0].PageObjectType);

            configuration.WebDriver.Browser.IsRemote.Should().BeFalse();
            configuration.WebDriver.Appium.ServerUri.Should().BeEquivalentTo("127.0.0.1:4723");
        }

        private static string JsonConfigurationFileContents
        {
            get
            {
                return @"{
                            ""webdriver"": {
                            ""mode"": ""browser"", // master switch:  browser, appium
                            ""maxWait"": 60000,
                            ""waitPollingFrequency"": 200,
                            ""browser"": { // settings for browser mode -> targets browser drivers or selenium server
                                            ""engine"": ""webdriver"", // webdriver, watin (not supported for now) ...
                                ""browserName"": ""chrome"", // targeted browser name : chrome, ie, firefox, opera, safari
                                ""isRemote"": false, // when true, seleniumServerUri must be set
                                ""seleniumServerUri"": ""http://localhost:5555/wd/hub"",
                                ""drivers"": {
                                                ""chrome"": {
                                                    ""path"": ""..\\..\\..\\packages\\Selenium.WebDriver.ChromeDriver.2.32.0\\driver\\win32"" //""C:\\Your Browser Drivers Path""//
                                },
                                ""ie"": { ""path"": """" },
                                ""firefox"": { ""path"": """" },
                                ""opera"": { ""path"": """" },
                                ""safari"": { ""path"": """" }
                                            },
                                ""window"": {
                                                ""isMaximized"": false, // if true, remaining browserWindow properties will be ignored
                                ""initialWidth"": 1280, // defaults to 800
                                ""initialHeight"": 800 // defaults to 600
                                }
                                        },
                            ""appium"": { // settings for appium mode -> targets Appium server
                                            ""serverUri"": ""127.0.0.1:4723"",
                                ""capabilities"": {
                                                ""automationName"": ""Appium"", // Appium, Selendroid
                                ""platformName"": ""Android"", // Android, iOS, FirefoxOS
                                ""deviceName"": ""192.168.166.101:5555"",
                                ""browserName"": ""Chrome"",
                                ""udid"": ""192.168.166.101:5555"",
                                // platform-specific capabilities
                                ""android"": { },
                                ""iOS"": { },
                                ""firefoxOS"": { }
                                            }
                                        }
                                    },
                            ""homepages"": [
                            {
                                ""pageObjectType"": ""Test000LoginPage"",
                                ""url"": ""../../../WebsiteMocks/Test000/login.html"", // relative url path to executing assembly's location
                                ""isFileSystemPath"": true
                            },
                            {
                                ""pageObjectType"": ""Test000GatewayPage"",
                                ""url"": ""../../../WebsiteMocks/Test000/gateway.html"", // relative url path to executing assembly's location
                                ""isFileSystemPath"": true
                            },
                            {
                                ""pageObjectType"": ""Test001CalculatorPage"",
                                ""url"": ""../../../WebsiteMocks/Test001/calculator.html"", // relative url path to executing assembly's location
                                ""isFileSystemPath"": true
                            },
                            {
                                ""pageObjectType"": ""PdfIndexPage"",
                                ""url"": ""../../../WebsiteMocks/Pdf/pdfIndex.html"", // relative url path to executing assembly's location
                                ""isFileSystemPath"": true
                            },
                            {
                                ""pageObjectType"": ""HoverCssPage"",
                                ""url"": ""../../../WebsiteMocks/hover_css.html"", // relative url path to executing assembly's location
                                ""isFileSystemPath"": true
                            },
                            {
                                ""pageObjectType"": ""AlertPage"",
                                ""url"": ""../../../WebsiteMocks/Alerts/alert.html"",
                                ""isFileSystemPath"": true
                            },
                            {
                                ""pageObjectType"": ""DndHtml5Page"",
                                ""url"": ""../../../WebsiteMocks/dnd_html5.html"",
                                ""isFileSystemPath"": true
                            },
                            {
                                ""pageObjectType"": ""DndJQueryPage"",
                                ""url"": ""../../../WebsiteMocks/dnd_jquery.html"",
                                ""isFileSystemPath"": true
                            },
                            {
                                ""pageObjectType"": ""ElementStatusPage"",
                                ""url"": ""../../../WebsiteMocks/element_status.html"",
                                ""isFileSystemPath"": true
                            },
                            {
                                ""pageObjectType"": ""DndJsPlumbPage"",
                                ""url"": ""https://jsplumbtoolkit.com/community/demo/flowchart/index.html""
                            },
                            {
                                ""pageObjectType"": ""GoogleSearchPage"",
                                ""url"": ""http://www.google.com"",
                                ""isFileSystemPath"": false
                            },
                            {
                                ""pageObjectType"": ""DashboardPage"",
                                ""url"": ""http://ap-gda01-cihrp:8080/"",
                                ""isFileSystemPath"": false
                            }
                            ]
                        }
                        ";
            }
        }
    }
}
