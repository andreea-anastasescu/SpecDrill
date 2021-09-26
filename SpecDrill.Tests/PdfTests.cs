using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SomeTests.PageObjects.Pdf;
using SpecDrill.MsTest;

namespace SomeTests
{
    [TestClass]
    public class Pdftests : MsTestBase
    {
        [ClassInitialize]
        public static void ClassInitializer(TestContext testContext) => _ClassSetup(testContext);

        [TestMethod]
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