using System;

namespace SpecDrill.Exceptions
{
    public class InvalidAttributeTargetException : ApplicationException
    {
        public InvalidAttributeTargetException(string message) : base(message) { }
    }
}
