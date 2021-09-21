using System;
using System.Collections.Generic;
using System.Text;

namespace SpecDrill.Exceptions
{
    public class TargetPropertyIsNotWebControlException : ApplicationException
    {
        public TargetPropertyIsNotWebControlException(string message) : base(message) { }
    }
    public class WebControlTargetLocatorNotProvidedException : ApplicationException
    {
        public WebControlTargetLocatorNotProvidedException(string message) : base(message) { }
    }

    public class WebControlTargetPropertyNotFoundException : ApplicationException
    { 
        public WebControlTargetPropertyNotFoundException(string message) : base(message) {  }
    }

    public class NoFindTargetAttributeOnNavigationElementMemberNorFindAttributeOnTargetWebControlException : ApplicationException
    {
        public NoFindTargetAttributeOnNavigationElementMemberNorFindAttributeOnTargetWebControlException(string message) : base(message) {  }
    }
}
