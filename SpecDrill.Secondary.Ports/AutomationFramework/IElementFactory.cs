namespace SpecDrill.Secondary.Ports.AutomationFramework
{
    public interface IElementFactory
    {
        public IElement Create(IElement? parent, IElementLocator locator);
        public ISelectElement CreateSelect(IElement? parent, IElementLocator locator);
        public INavigationElement<T> CreateNavigation<T>(IElement? parent, IElementLocator locator, IElementLocator? targetLocator) where T : class, INavigationTargetElement;
        public IFrameElement<T> CreateFrame<T>(IElement? parent, IElementLocator locator) where T : class, IPage;
        public IWindowElement<T> CreateWindow<T>(IElement? parent, IElementLocator locator) where T : class, IPage;
    }
}
