using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecDrill.Infrastructure.Configuration.WebDriver.Browser.Drivers
{
    public class DriverConfiguration
    {
        public string? Path { get; set; }
        public string? BrowserBinaryPath { get; set; }

        public List<string>? Arguments { get; set; }
    }
}

