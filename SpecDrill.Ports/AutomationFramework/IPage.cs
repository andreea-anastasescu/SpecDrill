using System;
using SpecDrill;
using SpecDrill.SecondaryPorts.AutomationFramework.Model;

namespace SpecDrill.SecondaryPorts.AutomationFramework
{
    public interface IPage : IElement, IDisposable
    {
        string Title { get; }
        bool IsLoaded { get; }
        PageContextTypes ContextType { get; set; }
        void WaitForSilence();
        void RefreshPage();
    }
}
