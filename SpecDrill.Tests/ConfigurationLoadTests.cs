using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpecDrill.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
