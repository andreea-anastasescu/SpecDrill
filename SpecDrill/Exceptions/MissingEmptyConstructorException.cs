using System;

namespace SpecDrill.Exceptions
{
    public class MissingEmptyConstructorException : ApplicationException
    {
        public MissingEmptyConstructorException(string message) : base(message) { }
        public MissingEmptyConstructorException(string message, Exception innerException) : base(message, innerException) { }
    }
}
