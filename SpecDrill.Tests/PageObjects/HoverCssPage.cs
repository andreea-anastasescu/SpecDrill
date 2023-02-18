using SpecDrill;
using SpecDrill.Secondary.Ports.AutomationFramework;

#nullable disable

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
