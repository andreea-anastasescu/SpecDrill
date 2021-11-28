using OpenQA.Selenium;
using SpecDrill.Secondary.Ports.AutomationFramework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecDrill.Secondary.Adapters.WebDriver;
public class SeleniumSearchable : ISearchable, ISearchContext
{
    ISearchContext webElement;
    IBrowserDriver browser;
    public SeleniumSearchable(IBrowserDriver browser, ISearchContext webElement) => (this.browser, this.webElement) = (browser, webElement);
    public static ISearchable Create(IBrowserDriver browser, ISearchContext webElement) => new SeleniumSearchable(browser, webElement);

    public ReadOnlyCollection<ISearchable> FindElements(IElementLocator locator)
        => browser.FindElements(locator, this);

    public ISearchable GetShadowRoot()
        => Create(this.browser, ((IWebElement)webElement).GetShadowRoot());

    public bool IsShadowRoot()
        => (bool?)browser.ExecuteJavaScript("return !!arguments[0].shadowRoot", webElement) ?? false;

    #region ISearchContext
    public IWebElement FindElement(OpenQA.Selenium.By by)
        => webElement.FindElement(by);

    public ReadOnlyCollection<IWebElement> FindElements(OpenQA.Selenium.By by)
        => webElement.FindElements(by);
    #endregion

    public object NativeElement { get => webElement; }
}
