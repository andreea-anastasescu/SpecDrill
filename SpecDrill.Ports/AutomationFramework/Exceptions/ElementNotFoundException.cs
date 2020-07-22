using System;

namespace SpecDrill.SecondaryPorts.AutomationFramework.Exceptions
{
    public class ElementNotFoundException : Exception
    {
        public ElementNotFoundException(string message) : base(message) { }
    }
}
