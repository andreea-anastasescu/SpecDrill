using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecDrill.SecondaryPorts.AutomationFramework
{
    public interface IBrowserAlert
    {
        void Accept();
        void Dismiss();
        string Text { get; }
        void SendKeys(string keys);
    }
}
