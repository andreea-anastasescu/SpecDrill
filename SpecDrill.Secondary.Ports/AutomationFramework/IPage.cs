using SpecDrill.Secondary.Ports.AutomationFramework.Model;
using System;

namespace SpecDrill.Secondary.Ports.AutomationFramework
{
    public interface IPage : INavigationTargetElement, IDisposable
    {
        string Title { get; }
        PageContextTypes ContextType { get; set; }
        void RefreshPage();
    }
}
