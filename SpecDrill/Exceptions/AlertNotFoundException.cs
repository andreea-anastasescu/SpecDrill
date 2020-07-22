using System;

namespace SpecDrill.Exceptions
{
    public class AlertNotFoundException : ApplicationException
    {
        public AlertNotFoundException(string message) : base(message) { }
    }
}
