using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecDrill.Adapters.WebDriver.Extensions
{
    internal static class TupleExtensions
    {
        public static bool Evaluate(this Tuple<bool, Exception?> testResult, bool throwException = false)
        {
            if (testResult == null || testResult.Item2 != null)
            {
                if (throwException)
                {
                    throw testResult?.Item2 ?? new Exception($"{nameof(testResult)} is (null)");
                }
                else
                {
                    return false;
                }
            }

            return testResult.Item1;
        }
    }
}
