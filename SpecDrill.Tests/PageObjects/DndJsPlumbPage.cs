using SpecDrill;
using SpecDrill.Secondary.Ports.AutomationFramework;

namespace SomeTests.PageObjects.Alerts
{
    public class DndJsPlumbPage : WebPage
    {
        [Find(By.CssSelector, "#canvas > div:nth-child(18)")]
        public IElement DivDraggable { get; private set; }

        [Find(By.CssSelector, "#canvas > div:nth-child(15)")]
        public IElement DivDropTarget { get; private set; }
    }
}
