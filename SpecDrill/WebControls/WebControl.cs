using SpecDrill.Secondary.Ports.AutomationFramework;

namespace SpecDrill
{
    public class WebControl : Element, IControl
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

        public virtual void WaitForSilence()
        {
        }
    }
}
