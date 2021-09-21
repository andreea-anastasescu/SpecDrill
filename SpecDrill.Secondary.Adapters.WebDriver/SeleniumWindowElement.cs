using OpenQA.Selenium;
using SpecDrill.Infrastructure.Logging;
using SpecDrill.Infrastructure.Logging.Interfaces;
using SpecDrill.Secondary.Ports.AutomationFramework;
using SpecDrill.Secondary.Ports.AutomationFramework.Core;
using SpecDrill.Secondary.Ports.AutomationFramework.Model;
using System;

namespace SpecDrill.Secondary.Adapters.WebDriver
{
    internal class SeleniumWindowElement<T> : SeleniumElement, IWindowElement<T>
        where T : class, IPage
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
            return (T)targetPage;
        }
    }
}
