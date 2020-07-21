using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecDrill.SecondaryPorts.AutomationFramework.Exceptions
{
    public class ElementNotFoundException : Exception
    {
        public ElementNotFoundException(string message) : base(message) { }
    }
}
