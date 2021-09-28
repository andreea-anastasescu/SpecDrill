using SpecDrill.Secondary.Ports.AutomationFramework;
using System;

namespace SpecDrill
{
    public class ElementFactory
    {
        internal static IElementFactory? Instance { get; set; }

        public static IElementFactory Factory
            => Instance ?? throw new Exception($"WebElement.ElementFactory was not provided with a IElementFactory instance!");
        
        public static IElement Create(IElement? parent, IElementLocator locator)
            => new Element(parent, locator);
        //TODO:
        public static ISelectElement CreateSelect(IElement? parent, IElementLocator locator)
            => new SelectElement(parent, locator);

        public static INavigationElement<T> CreateNavigation<T>(IElement? parent, IElementLocator locator, IElementLocator? targetLocator)
            where T : class, INavigationTargetElement
            => new NavigationElement<T>(parent, locator, targetLocator);

        public static ListElement<T> CreateList<T>(IElement? parent, IElementLocator elementLocator)
            where T : class, IElement
            => new(parent, elementLocator);
        //TODO:
        public static IFrameElement<T> CreateFrame<T>(IElement? parent, IElementLocator locator)
            where T : class, IPage
            => Factory.CreateFrame<T>(parent, locator);
        //TODO:
        public static IWindowElement<T> CreateWindow<T>(IElement? parent, IElementLocator locator)
            where T : class, IPage
            => Factory.CreateWindow<T>(parent, locator);
        //TODO: add SeleniumAlert
        public static T CreateControl<T>(IElement? parent, IElementLocator elementLocator)
            where T : class, IElement//IControl
        {
            return Browser.Instance.CreateControl<T>(parent, elementLocator);
        }
    }
}
