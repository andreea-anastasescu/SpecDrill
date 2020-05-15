using SpecDrill.Infrastructure.Configuration.WebDriver.Appium;
using SpecDrill.Infrastructure.Configuration.WebDriver.Browser;
using SpecDrill.Infrastructure.Configuration.WebDriver.Screenshots;

namespace SpecDrill.Configuration.WebDriver
{
    public class WebDriverConfiguration
    {
        public string? Mode { get; set; }

        public int MaxWait { get; set; }

        public int WaitPollingFrequency { get; set; }

        public ScreenshotsConfiguration? Screenshots { get; set; }
        public BrowserConfiguration? Browser { get; set; }
        public AppiumConfiguration? Appium { get; set; }
    }
}
