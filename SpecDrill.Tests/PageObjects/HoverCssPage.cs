using SpecDrill;
using SpecDrill.SecondaryPorts.AutomationFramework;

namespace SomeTests.PageObjects.Alerts
{
    public class HoverCssPage : WebPage
    {
        [Find(By.CssSelector, ".tooltip")]
        public IElement DivTooltip { get; private set; }

        [Find(By.CssSelector, ".tooltiptext")]
        public IElement DivTooltipText { get; private set; }
    }
}
