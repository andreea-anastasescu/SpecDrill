using SpecDrill.SecondaryPorts.AutomationFramework;
using SpecDrill.WebControls;
using System;

namespace SpecDrill
{
    public class WebElement
    {
        public static IElementFactory? ElementFactory { get; set; }
        private static IElementFactory Factory
            => ElementFactory ?? throw new Exception($"WebElement.ElementFactory was not provided with a IElementFactory instance!");
        public static IElement Create(IElement? parent, IElementLocator locator)
            => Factory.Create(parent, locator);

        public static ISelectElement CreateSelect(IElement? parent, IElementLocator locator)
            => Factory.CreateSelect(parent, locator);

        public static INavigationElement<T> CreateNavigation<T>(IElement? parent, IElementLocator locator)
            where T : class, IPage
            => Factory.CreateNavigation<T>(parent, locator);

        public static ListElement<T> CreateList<T>(IElement? parent, IElementLocator elementLocator)
            where T : WebControl
            => new ListElement<T>(parent, elementLocator);

        public static IFrameElement<T> CreateFrame<T>(IElement? parent, IElementLocator locator)
            where T : class, IPage
            => Factory.CreateFrame<T>(parent, locator);

        public static IWindowElement<T> CreateWindow<T>(IElement? parent, IElementLocator locator)
            where T : class, IPage
            => ElementFactory!.CreateWindow<T>(parent, locator);

        public static T CreateControl<T>(IElement? parent, IElementLocator elementLocator)
            where T : class, IElement
        {
            var control = Activator.CreateInstance(typeof(T), parent, elementLocator) as T;
            return Browser.Instance.CreateControl<T>(control);
        }
    }
}
