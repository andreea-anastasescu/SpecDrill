using System.Collections.Generic;

namespace SpecDrill.Infrastructure.Configuration.WebDriver.Appium
{
    public class AppiumConfiguration
    {
        public string? ServerUri { get; set; }
        public /*CapabilitiesConfiguration*/ Dictionary<string, object>? Capabilities { get; set; }
    }
}
