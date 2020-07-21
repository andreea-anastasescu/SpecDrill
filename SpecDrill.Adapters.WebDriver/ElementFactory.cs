using SpecDrill.SecondaryPorts.AutomationFramework;
using SpecDrill.SecondaryPorts.AutomationFramework.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpecDrill.SecondaryPorts.Adapters.WebDriver
{
    internal class ElementFactory : IElementFactory
    {
        private readonly IBrowser browser;
        public ElementFactory(IBrowser browser) { this.browser = browser; }
        public IElement Create(IElement? parent, IElementLocator locator)
            => new SeleniumElement(this.browser, parent, locator);

        public ISelectElement CreateSelect(IElement? parent, IElementLocator locator)
            => new SeleniumSelectElement(this.browser, parent, locator);
        public INavigationElement<T> CreateNavigation<T>(IElement? parent, IElementLocator locator)
            where T : class, IPage
            => new SeleniumNavigationElement<T>(this.browser, parent, locator);
        public IFrameElement<T> CreateFrame<T>(IElement? parent, IElementLocator locator)
            where T : class, IPage
            => new SeleniumFrameElement<T>(this.browser, parent, locator);
        public IWindowElement<T> CreateWindow<T>(IElement? parent, IElementLocator locator)
            where T: class, IPage
            => new SeleniumWindowElement<T>(this.browser, parent, locator);

    }
}
