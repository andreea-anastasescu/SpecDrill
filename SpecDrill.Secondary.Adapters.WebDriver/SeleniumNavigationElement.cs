using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using SpecDrill.Secondary.Ports.AutomationFramework;
using SpecDrill.Secondary.Ports.AutomationFramework.Core;
using System;

namespace SpecDrill.Secondary.Adapters.WebDriver
{
    internal class SeleniumNavigationElement<T> : SeleniumElement, INavigationElement<T>
        where T : class, INavigationTargetElement
    {

        public IElementLocator? TargetLocator { get; private set; }
        public SeleniumNavigationElement(IBrowser? browser, IElement? parent, IElementLocator locator, IElementLocator? targetLocator) : base(browser, parent, locator)
        {
            TargetLocator = targetLocator;
        }

        private T Click(ClickType clickType, Func<bool>? navigationSucceeded, IElement? targetElement = null)
        {
            void Click(int attemptsLeft = 3)
            {
                try
                {
                    switch (clickType)
                    {
                        case ClickType.Double:
                            Browser.DoubleClick(this); break;
                        case ClickType.Js:
                            Browser.ClickJs(this); break;
                        case ClickType.Single:
                        default:
                            this.Element.Click();
                            break;
                    }
                }
                catch (StaleElementReferenceException e)
                {
                    Logger.LogError(e, "Retrying Click due to Stale Element");
                    Browser.JsLog(string.Concat("Retrying Click due to: ", e.Message));
                    if (attemptsLeft > 0)
                        Click(attemptsLeft - 1);
                }
            }


            // INavigationElement<T> where T : IPage
            // IOpenerElement<T> where T: IControl
            // either have Find on IControl class for detached WebControls
            // or have [Target] attribute with nameof(Member) for page member WebControls

            //CreateTarget<T>();
            INavigationTargetElement target = Browser.CreateTarget<T>(null, this.TargetLocator);
            //typeof(IPage).IsAssignableFrom(typeof(T)) ? Browser.CreatePage<T>() :
            //targetElement ?? throw new InvalidOperationException("Navigation elements targeting WebControls must provide a target element instance!");
            Wait.WithRetry(3, TimeSpan.FromSeconds(20)).Doing(() =>
            {
                Wait.Until(() => this.IsAvailable && this.IsVisible);
                Click();
            }).Until(
                () =>
                {
                    return (navigationSucceeded ?? (() => target.IsLoaded))();
                    //bool conditionMet = target.IsLoaded;
                    //Browser.JsLog($"navigation click condition met = { conditionMet }");
                    //return conditionMet;
                });
            target.WaitForSilence();
            return (T)target;
        }

        public T Click(Func<bool>? navigationSucceeded = null) => this.Click(ClickType.Single, navigationSucceeded);

        public T DoubleClick(Func<bool>? navigationSucceeded = null) => this.Click(ClickType.Double, navigationSucceeded);

        public T ClickJs(Func<bool>? navigationSucceeded = null) => this.Click(ClickType.Js, navigationSucceeded);
    }
}
