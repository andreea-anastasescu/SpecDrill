using System.Collections.Generic;

namespace SpecDrill.Infrastructure.Configuration.WebDriver.Browser.Drivers
{
    public class DriverConfiguration
    {
        public string? Path { get; set; }
        public string? BrowserBinaryPath { get; set; }

        public List<string>? Arguments { get; set; }
        public Dictionary<string, object>? Preferences { get; set; }
    }
}

