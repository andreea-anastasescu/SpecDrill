using SpecDrill.Exceptions;
using SpecDrill.Secondary.Ports.AutomationFramework;
using SpecDrill.Secondary.Ports.AutomationFramework.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpecDrill
{
    internal class NavigationElement<T> : Element, INavigationElement<T>
        where T : class, INavigationTargetElement
    {
        INavigationElement<T> rootNavigationElement;
        public NavigationElement(IElement? parent, IElementLocator locator, IElementLocator? targetLocator) : base(parent, locator)
        {
            if (typeof(IControl).IsAssignableFrom(typeof(T)) && targetLocator == null)
                throw new WebControlTargetLocatorNotProvidedException($"When Navigating to WebControl : IControl elements, targetLocator parameter must be provided!");
            this.rootElement = this.rootNavigationElement = ElementFactory.Factory.CreateNavigation<T>(parent, locator, targetLocator);
        }

        public T Click(Func<bool>? navigationSucceeded = null)
        {
            return this.rootNavigationElement.Click(navigationSucceeded);
        }

        public T ClickJs(Func<bool>? navigationSucceeded = null)
        {
            return this.rootNavigationElement.ClickJs(navigationSucceeded);
        }

        public T DoubleClick(Func<bool>? navigationSucceeded = null)
        {
            return this.rootNavigationElement.DoubleClick(navigationSucceeded);
        }
    }
 }
