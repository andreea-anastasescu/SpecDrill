using OpenQA.Selenium;
using SpecDrill.Secondary.Ports.AutomationFramework;
using SpecDrill.Secondary.Ports.AutomationFramework.Core;
using SpecDrill.Secondary.Ports.AutomationFramework.Model;
using System;

namespace SpecDrill.Secondary.Adapters.WebDriver
{
    internal class SeleniumFrameElement<T> : SeleniumElement, IFrameElement<T>
        where T : class, IPage
    {
        public SeleniumFrameElement(IBrowser? browser, IElement? parent, IElementLocator locator) : base(browser, parent, locator)
        {
        }

        public T SwitchTo() => Open();
        public T Open()
        {
            Wait.NoMoreThan(TimeSpan.FromSeconds(30)).Until(() => this.IsAvailable);
            Browser.SwitchToFrame(this);
            IPage targetPage = Browser.CreatePage<T>();
            targetPage.ContextType = PageContextTypes.Frame;
            Wait.Until(() => targetPage.IsLoaded);
            targetPage.WaitForSilence();
            return (T)targetPage;
        }
    }
}
