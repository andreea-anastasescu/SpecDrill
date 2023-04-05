using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SomeTests.PageObjects.Alerts;
using SpecDrill;
using SpecDrill.MsTest;
using SpecDrill.Secondary.Ports.AutomationFramework;
using System;

#nullable disable

namespace SomeTests
{
    [TestClass]
    public class DndTests : MsTestBase
    {
        [ClassInitialize]
        public static void ClassInitializer(TestContext testContext) => _ClassSetup(testContext);

        [TestMethod]
        public void ShouldHtml5DragAndDropSuccessfuly()
        {
            var hoverPage = Browser.Open<DndHtml5Page>();

            hoverPage.DivDraggable.DragAndDropTo(hoverPage.DivDropTarget);

            var droppedElement = ElementFactory.Create(null, ElementLocatorFactory.Create(By.CssSelector, "#div1>#drag1"));

            Wait.NoMoreThan(TimeSpan.FromSeconds(2)).Until(() => droppedElement.IsAvailable);

            droppedElement.IsAvailable.Should().BeTrue();
        }

        [TestMethod]
        public void ShouldJQueryDragAndDropSuccessfuly()
        {
            var hoverPage = Browser.Open<DndJQueryPage>();

            hoverPage.DivCard5.DragAndDropTo(hoverPage.DivDropTargetCard5);

            hoverPage.DivDropTargetCard5.GetAttribute("aria-disabled").Should().Be("true");
        }

        [TestMethod]
        [Ignore]
        public void ShouldJsPlumbDragAndDropSuccessfuly()
        {
            var hoverPage = Browser.Open<DndJsPlumbPage>();
            Wait.Until(() => hoverPage.DivDraggable.IsAvailable);
            hoverPage.DivDraggable.DragAndDropTo(hoverPage.DivDropTarget);

            hoverPage.DivDropTarget.GetAttribute("class").Should().Contain("jtk-endpoint-connected");
        }

        [Ignore]
        [TestMethod]
        public void WfDnDTest()
        {
            var homePage = Browser.Open<WfHomePage>();
            this.Browser.MaximizePage();
            Wait.Until(() => homePage.TdSomeProcessDefinition.IsAvailable);
            var wfDesignerPage = homePage.TdSomeProcessDefinition.DoubleClick();

            Wait.Until(() => wfDesignerPage.ADecision.IsAvailable);
            Wait.Until(() => wfDesignerPage.Canvas.IsAvailable);

            wfDesignerPage.ADecision.DoubleClick();

            Wait.Until(() => wfDesignerPage.DecisionActivity.IsAvailable);

            Wait.Until(() => wfDesignerPage.DecisionConnector.IsAvailable);

            wfDesignerPage.StartConnector.DragAndDropTo(wfDesignerPage.DecisionConnector);
            wfDesignerPage.DecisionActivity.DragAndDropAt(100, 0);

            wfDesignerPage.DecisionConnector.GetAttribute("class").Should().Contain("jsplumb-endpoint-connected");
        }
    }

    public class WfHomePage : WebPage
    {
        [FindAttribute(By.CssSelector, "#content-container > ng-component > div.overview.callout > table > tbody > tr:nth-child(8) > td:nth-child(2)")]
        public INavigationElement<WfDesigner> TdSomeProcessDefinition { get; private set; }
    }
    public class WfDesigner : WebPage
    {

        [FindAttribute(By.XPath, @"//*[@id='System.Activities.Statements.FlowDecision, System.Activities, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35']")]
        public IElement ADecision { get; private set; }

        [FindAttribute(By.Id, "canvas")]
        public IElement Canvas { get; private set; }

        [FindAttribute(By.XPath, "//*[@id='canvas']/div[2]")]
        public IElement StartConnector { get; private set; }

        [FindAttribute(By.XPath, "//*[@id='canvas']/div[5]")]
        public IElement DecisionConnector { get; private set; }

        [FindAttribute(By.CssSelector, "flowchart>div#canvas>flownode:nth-of-type(1)>div:nth-of-type(1)")]
        public IElement DecisionActivity { get; private set; }

    }

}
