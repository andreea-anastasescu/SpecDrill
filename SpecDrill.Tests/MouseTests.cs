using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SomeTests.PageObjects;
using SpecDrill;
using System;
using System.Diagnostics;
using SpecDrill.MsTest;
using System.Windows.Markup;

namespace SomeTests
{
    [TestClass]
    public class MouseTests : MsTestBase
    {
        [ClassInitialize]
        public static void ClassInitializer(TestContext testContext) => _ClassSetup(testContext);
        [TestMethod]
        [Ignore]
        //[ExpectedException(typeof(TimeoutException))]
        public void ShouldClickAndDrag()
        {
            var gsp = Browser.Open<GoogleMapsPage>();
            (int, int) startingCoords = gsp.MapCanvas.GetCoordinates();
            gsp.MapCanvas.DragAndDropAt(100, 100);

            this.Browser.ClickAndDrag(from: (700, 700), offsetX: 100, offsetY: 100);
            // using Actions API the drag does not work... to try out other options!
            //this.Browser.ClickAndDrag();
            System.Threading.Thread.Sleep(2000);
        }

    }
}