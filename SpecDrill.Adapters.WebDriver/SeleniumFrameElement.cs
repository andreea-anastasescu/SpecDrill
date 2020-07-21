using System;
using OpenQA.Selenium;
using SpecDrill.Infrastructure.Logging;
using SpecDrill.Infrastructure.Logging.Interfaces;
using SpecDrill.SecondaryPorts.AutomationFramework;
using SpecDrill.SecondaryPorts.AutomationFramework.Core;
using SpecDrill.SecondaryPorts.AutomationFramework.Model;

namespace SpecDrill.SecondaryPorts.Adapters.WebDriver
{
    internal class SeleniumFrameElement<T> : SeleniumElement, IFrameElement<T>
        where T: class, IPage
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
            return (T) targetPage;
        }
    }
}
