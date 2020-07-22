using System;

namespace SpecDrill.Exceptions
{
    public class MissingHomepageConfigEntryException : ApplicationException
    {
        public MissingHomepageConfigEntryException(string message) : base(message) { }
    }
}
