using System;
using System.Collections.Generic;
using System.Text;

namespace SpecDrill.SecondaryPorts.AutomationFramework
{
    public interface IElementFactory
    {
        public IElement Create(IElement? parent, IElementLocator locator);
        public ISelectElement CreateSelect(IElement? parent, IElementLocator locator);
        public INavigationElement<T> CreateNavigation<T>(IElement? parent, IElementLocator locator) where T: class, IPage;
        public IFrameElement<T> CreateFrame<T>(IElement? parent, IElementLocator locator) where T: class, IPage;
        public IWindowElement<T> CreateWindow<T>(IElement? parent, IElementLocator locator) where T: class, IPage;
    }
}
