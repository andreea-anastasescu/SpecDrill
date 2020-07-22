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
    }
}
