using System;

namespace SpecDrill.Secondary.Ports.AutomationFramework
{
    public interface IFrameElement<out T> : IElement
        where T : IPage
    {
        T Open();

        [Obsolete("using IFrameElement<>.Open() is now recommended.")]
        T SwitchTo();
    }
}