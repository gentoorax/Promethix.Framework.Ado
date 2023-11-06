using Microsoft.Extensions.Configuration;
using System;

namespace Promethix.Framework.Ado.Implementation
{
    public class OptionsBuilder
    {
        protected static string GetValue(IConfigurationSection section, string key)
        {
            return section?[key];
        }

        protected static TEnum GetEnumValue<TEnum>(IConfigurationSection section, string key, TEnum defaultValue) where TEnum : struct
        {
            string enumValue = section?[key];
            if (Enum.TryParse(enumValue, out TEnum parsedValue))
            {
                return parsedValue;
            }
            return defaultValue;
        }

        protected static TEnum? GetEnumValueOrNull<TEnum>(IConfigurationSection section, string key) where TEnum : struct
        {
            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            string enumValue = section[key];
            if (Enum.TryParse(enumValue, out TEnum parsedValue))
            {
                return parsedValue;
            }
            return null;
        }
    }
}