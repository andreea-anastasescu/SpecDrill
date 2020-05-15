﻿using SpecDrill.Infrastructure.Configuration.WebDriver.Browser.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecDrill.Infrastructure.Configuration.WebDriver.Browser
{
    public class BrowserConfiguration
    {
        public string? Engine { get; set; }
        public string? BrowserName { get; set; }
        public bool IsRemote { get; set; }
        public string? SeleniumServerUri { get; set; }
        public DriversConfiguration? Drivers { get; set; }
        public WindowConfiguration? Window { get; set; }
        public /*CapabilitiesConfiguration*/ Dictionary<string, object>? Capabilities { get; set; }

    }
}
