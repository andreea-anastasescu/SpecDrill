using SpecDrill;
using SpecDrill.Secondary.Ports.AutomationFramework;

namespace SomeTests.PageObjects
{
    class ElementStatusPage : WebPage
    {
        [Find(By.CssSelector, "#canvas > div:nth-child(18)")]
        public IElement DivDraggable { get; private set; }

        [Find(By.CssSelector, "#canvas > div:nth-child(15)")]
        public IElement DivDropTarget { get; private set; }
    }
}
