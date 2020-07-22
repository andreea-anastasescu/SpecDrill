using SpecDrill;
using SpecDrill.SecondaryPorts.AutomationFramework;

namespace SomeTests.PageObjects.Alerts
{
    public class DndHtml5Page : WebPage
    {
        [Find(By.Id, "drag1")]
        public IElement DivDraggable { get; private set; }

        [Find(By.Id, "div1")]
        public IElement DivDropTarget { get; private set; }
    }
}
