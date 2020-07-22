using System;

namespace SpecDrill.Exceptions
{
    public class DynamicMemberInitializationException : ApplicationException
    {
        public DynamicMemberInitializationException(string message) : base(message) { }
    }
}
