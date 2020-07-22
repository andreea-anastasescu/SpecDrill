using SpecDrill.SecondaryPorts.AutomationFramework;

namespace SpecDrill
{
    public class WebControl : ElementBase, IControl
    {
        public WebControl(IElement? parent, IElementLocator locator) : base(parent, locator)
        {
        }

        public bool IsLoaded
        {
            get
            {
                return this.rootElement.IsAvailable;
            }
        }
    }
}
