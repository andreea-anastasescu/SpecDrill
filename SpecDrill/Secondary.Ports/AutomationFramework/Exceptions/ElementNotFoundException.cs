using System;

namespace SpecDrill.Secondary.Ports.AutomationFramework.Exceptions
{
    public class ElementNotFoundException : Exception
    {
        public ElementNotFoundException(string message) : base(message) { }
    }
}
