using SpecDrill.SecondaryPorts.AutomationFramework.Model;
using System;

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
