using SpecDrill.Infrastructure.Configuration.WebDriver.Browser.Drivers.Chrome;
using SpecDrill.Infrastructure.Configuration.WebDriver.Browser.Drivers.Firefox;
using SpecDrill.Infrastructure.Configuration.WebDriver.Browser.Drivers.Ie;
using SpecDrill.Infrastructure.Configuration.WebDriver.Browser.Drivers.Opera;

namespace SpecDrill.Infrastructure.Configuration.WebDriver.Browser
{
    public class DriversConfiguration
    {
        public ChromeConfiguration? Chrome { get; set; }
        public IeConfiguration? Ie { get; set; }
        public OperaConfiguration? Opera { get; set; }
        public FirefoxConfiguration? Firefox { get; set; }
        public SafariConfiguration? Safari { get; set; }
        public EdgeConfiguration? Edge { get; set; }
    }
}