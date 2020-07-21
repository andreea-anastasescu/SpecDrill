using System;
using OpenQA.Selenium;
using SpecDrill.Infrastructure.Logging;
using SpecDrill.Infrastructure.Logging.Interfaces;
using SpecDrill.SecondaryPorts.AutomationFramework;
using SpecDrill.SecondaryPorts.AutomationFramework.Core;
using SpecDrill.SecondaryPorts.AutomationFramework.Model;

namespace SpecDrill.SecondaryPorts.Adapters.WebDriver
{
    internal class SeleniumWindowElement<T> : SeleniumElement, IWindowElement<T>
        where T: class, IPage
    {
        public SeleniumWindowElement(IBrowser? browser, IElement? parent, IElementLocator locator) : base(browser, parent, locator)
        {
        }
        public T Open()
        {
            Wait.NoMoreThan(TimeSpan.FromSeconds(7)).Until(() => this.IsAvailable);
            Browser.SwitchToWindow(this);
            IPage targetPage = Browser.CreatePage<T>();
            targetPage.ContextType = PageContextTypes.Window;
            Wait.NoMoreThan(TimeSpan.FromSeconds(7)).Until(() => targetPage.IsLoaded);
            targetPage.WaitForSilence();
            return (T) targetPage;
        }
    }
}
