using System;

namespace SpecDrill.Secondary.Ports.AutomationFramework
{
    public interface INavigationElement<out T> : IElement
        where T : class, INavigationTargetElement
    {
        T Click(Func<bool>? navigationSucceeded = null);
        T DoubleClick(Func<bool>? navigationSucceeded = null);
        T ClickJs(Func<bool>? navigationSucceeded = null);
    }
}