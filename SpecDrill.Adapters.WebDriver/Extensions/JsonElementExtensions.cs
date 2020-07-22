using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace SpecDrill.SecondaryPorts.Adapters.WebDriver.Extensions
{
    public static class JsonElementExtensions
    {
        public static object? ToObject(this JsonElement jsonElement)
            => jsonElement.ValueKind switch
            {
                JsonValueKind.Number => jsonElement.ToString(),
                JsonValueKind.String => jsonElement.ToString(),
                JsonValueKind.False => false,
                JsonValueKind.True => true,
                _ => null
            };

    }
}
