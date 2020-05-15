using SpecDrill.Infrastructure.Configuration.WebDriver.Appium.Capabilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecDrill.Infrastructure.Configuration.WebDriver.Appium
{
    public class AppiumConfiguration
    {
        public string? ServerUri { get; set; }
        public /*CapabilitiesConfiguration*/ Dictionary<string, object>? Capabilities { get; set; }
    }
}
