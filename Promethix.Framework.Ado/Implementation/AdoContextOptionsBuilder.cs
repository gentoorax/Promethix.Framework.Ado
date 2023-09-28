using Microsoft.Extensions.Configuration;
using Promethix.Framework.Ado.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethix.Framework.Ado.Implementation
{
    public class AdoContextOptionsBuilder
    {
        public string Name { get; private set; }

        public string ProviderName { get; private set; }

        public string ConnectionString { get; private set; }

        public AdoContextExecutionOption ExecutionOption { get; private set; }

        public IsolationLevel? OverrideDefaultIsolationLevel { get; private set; }

        public AdoContextOptionsBuilder WithNamedConnection(string name, IConfiguration configuration = null)
        {
            Name = name;

            if (configuration != null)
            {
                TryPopulateConfiguration(configuration);
            }

            return this;
        }

        private static string GetValue(IConfigurationSection section, string key)
        {
            return section?[key];
        }

        private static TEnum GetEnumValue<TEnum>(IConfigurationSection section, string key, TEnum defaultValue) where TEnum : struct
        {
            string enumValue = section?[key];
            if (Enum.TryParse(enumValue, out TEnum parsedValue))
            {
                return parsedValue;
            }
            return defaultValue;
        }

        private static TEnum? GetEnumValueOrNull<TEnum>(IConfigurationSection section, string key) where TEnum : struct
        {
            string enumValue = section[key];
            if (Enum.TryParse(enumValue, out TEnum parsedValue))
            {
                return parsedValue;
            }
            return null;
        }

        private void TryPopulateConfiguration(IConfiguration configuration)
        {
            IConfigurationSection connectionStringsSection = configuration.GetSection($"ConnectionStrings:{Name}");
            IConfigurationSection adoContextConfigSection = configuration.GetSection($"AdoContextOptions:{Name}");

            ConnectionString = connectionStringsSection?.Value ?? GetValue(adoContextConfigSection, Name);
            ProviderName = GetValue(adoContextConfigSection, "ProviderName");
            ExecutionOption = GetEnumValue(adoContextConfigSection, "ExecutionOption", AdoContextExecutionOption.Transactional);
            OverrideDefaultIsolationLevel = GetEnumValueOrNull<IsolationLevel>(adoContextConfigSection, "OverrideDefaultIsolationLevel");
        }


        public AdoContextOptionsBuilder WithProviderName(string providerName)
        {
            ProviderName = providerName;
            return this;
        }

        public AdoContextOptionsBuilder WithConnectionString(string connectionString)
        {
            ConnectionString = connectionString;
            return this;
        }

        public AdoContextOptionsBuilder WithExecutionOption(AdoContextExecutionOption executionOption)
        {
            ExecutionOption = executionOption;
            return this;
        }

        public AdoContextOptionsBuilder WithDefaultIsolationLevel(IsolationLevel? isolationLevel)
        {
            OverrideDefaultIsolationLevel = isolationLevel;
            return this;
        }
    }
}
