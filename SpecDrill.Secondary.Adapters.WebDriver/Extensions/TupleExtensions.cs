using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecDrill.Secondary.Adapters.WebDriver.Extensions
{
    internal static class TupleExtensions
    {
        public static bool Evaluate(this (bool result, Exception? exception) testResult, bool throwException = false)
        {
            var (result, exception) = testResult;

            if (exception != null)
            {
                if (throwException)
                {
                    throw exception ?? new Exception($"{nameof(testResult)} is (null)");
                }
                else
                {
                    return false;
                }
            }

            return result;
        }
    }
}
