using SpecDrill;
using SpecDrill.Secondary.Ports.AutomationFramework;

namespace SomeTests.PageObjects.Alerts
{
    public class DndJQueryPage : WebPage
    {
        [Find(By.Id, "card5")]
        public IElement DivCard5 { get; private set; }

        [Find(By.CssSelector, "#cardSlots>div:nth-child(5)")]
        public IElement DivDropTargetCard5 { get; private set; }
    }
}
