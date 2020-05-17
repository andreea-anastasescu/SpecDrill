using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SomeTests.PageObjects.Test000;
using SpecDrill;
using SpecDrill.MsTest;
using FluentAssertions;
using SpecDrill.AutomationScopes;
using SomeTests.PageObjects.Alerts;
using SpecDrill.SecondaryPorts.AutomationFramework;
using SomeTests.PageObjects.Pdf;

namespace SomeTests
{
    [TestClass]
    public class Pdftests : TestBase
    {
        [TestMethod]
        [Ignore]
        public void ShouldReadPdfText()
        {
            var alertPage = Browser.Open<PdfIndexPage>();
            using (var pdfPage = alertPage.LnkViewPdf.Open())
            {
                pdfPage.Text.Should().Contain("a3b2c1");
            }
        }
    }
}