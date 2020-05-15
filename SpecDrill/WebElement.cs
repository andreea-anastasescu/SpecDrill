using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpecDrill.Adapters.WebDriver;
using SpecDrill.SecondaryPorts.AutomationFramework;
using SpecDrill.SecondaryPorts.AutomationFramework.Core;
using SpecDrill.WebControls;

namespace SpecDrill
{
    public class WebElement
    {
        public static IElement Create(IElement? parent, IElementLocator locator)
        {
            return new SeleniumElement(Browser.Instance, parent, locator);
        }

        public static ISelectElement CreateSelect(IElement? parent, IElementLocator locator)
        {
            return new SeleniumSelectElement(Browser.Instance, parent, locator);
        }

        public static INavigationElement<T> CreateNavigation<T>(IElement? parent, IElementLocator locator)
            where T : class, IPage
        {
            return new SeleniumNavigationElement<T>(Browser.Instance, parent, locator);
        }

        public static ListElement<T> CreateList<T>(IElement? parent, IElementLocator elementLocator)
            where T : WebControl
        {
            return new ListElement<T>(parent, elementLocator);
        }

        public static IFrameElement<T> CreateFrame<T>(IElement? parent, IElementLocator locator)
            where T: class, IPage
        {
            return new SeleniumFrameElement<T>(Browser.Instance, parent, locator);
        }

        public static IWindowElement<T> CreateWindow<T>(IElement? parent, IElementLocator locator)
            where T : class, IPage
        {
            return new SeleniumWindowElement<T>(Browser.Instance, parent, locator);
        }

        public static T CreateControl<T>(IElement? parent, IElementLocator elementLocator)
            where T : class, IElement
        {
            var control = Activator.CreateInstance(typeof(T), parent, elementLocator) as T;
            return Browser.Instance.CreateControl<T>(control);
        }
    }
}
