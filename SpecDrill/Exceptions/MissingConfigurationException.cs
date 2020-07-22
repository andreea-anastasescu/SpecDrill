using System;

namespace SpecDrill.Exceptions
{
    public class MissingConfigurationException : ApplicationException
    {
        public MissingConfigurationException(string message) : base(message) { }
    }
}
