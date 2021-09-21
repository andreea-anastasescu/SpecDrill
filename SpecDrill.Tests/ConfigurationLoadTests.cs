using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpecDrill.Infrastructure.Configuration;

namespace SomeTests
{
    [TestClass]
    public class ConfigurationLoadTests
    {
        [TestMethod]
        public void ShouldLoadCofiguration()
        {
            var settings = ConfigurationManager.Load();
            settings.Should().NotBeNull();
        }

        [TestMethod]
        public void ShouldLoadCapabilitiesCofiguration()
        {
            var settings = ConfigurationManager.Load(null, "specDrillConfigCapabilitiesError.json");
            settings.Should().NotBeNull();
            var capabilities = settings.WebDriver?.Browser?.Capabilities;
            capabilities.Should().NotBeNull();
            capabilities?["acceptSslCerts"].ToString().Should().Be("true");
        }
    }
}
