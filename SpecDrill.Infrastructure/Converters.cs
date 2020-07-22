using System;

namespace SpecDrill.Infrastructure
{
    public static class Converters
    {
        public static T? ToEnum<T>(this string? enumValue)
            where T : struct, IConvertible
        {
            if (string.IsNullOrWhiteSpace(enumValue))
                return null;

            var result = enumValue.AsEnum<T>();

            if (!result.HasValue)
                throw new Exception($" {enumValue} is not a member of {typeof(T).Name} enum");

            return result.Value;
        }

        public static object? AsEnum(this string enumValue, Type enumType)
        {
            if (!enumType.IsEnum) return null;

            return Enum.TryParse(enumType, enumValue, out object result) ? result : null;
        }

        public static bool OfEnum(this string enumValue, Type enumType)
        {
            if (!enumType.IsEnum)
                return false;

            return Enum.TryParse(enumType, enumValue, out object _);
        }

        public static bool OfEnum<T>(this string enumValue)
            where T : struct, IConvertible
            => enumValue.OfEnum(typeof(T));
        public static T? AsEnum<T>(this string enumValue)
            where T : struct, IConvertible
            => (T?)enumValue.AsEnum(typeof(T));
    }
}
