using System;
using System.Collections.Generic;
using System.Text;

namespace SpecDrill.Secondary.Ports.AutomationFramework
{
    public interface INavigationTargetElement : IElement
    {
        bool IsLoaded { get; }
        void WaitForSilence();
    }
}
