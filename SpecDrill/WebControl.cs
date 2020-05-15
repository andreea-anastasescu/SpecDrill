using SpecDrill.SecondaryPorts.AutomationFramework;
using System;
using System.Collections.Generic;
using SpecDrill.SecondaryPorts.AutomationFramework.Core;
using System.Collections;
using System.Text.RegularExpressions;

namespace SpecDrill
{
    public class WebControl : ElementBase, IControl
    {
        public WebControl(IElement? parent, IElementLocator locator) : base(parent, locator)
        {
        }

        public bool IsLoaded
        {
            get
            {
                return this.rootElement.IsAvailable;
            }
        }
    }
}
