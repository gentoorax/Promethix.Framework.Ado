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
    public class AdoContextOptionsBuilder : OptionsBuilder
    {
        public string Name { get; private set; }

        public string ProviderName { get; private set; }

        public string ConnectionString { get; private set; }

        public AdoContextExecutionOption ExecutionOption { get; private set; }

        public IsolationLevel? OverrideDefaultIsolationLevel { get; private set; }

        public AdoContextOptionsBuilder WithNamedContext(string name, IConfiguration configuration = null)
        {
            Name = name;

            if (configuration != null)
            {
                TryPopulateConfiguration(configuration);
            }

            return this;
        }

        private void TryPopulateConfiguration(IConfiguration configuration)
        {
            IConfigurationSection connectionStringsSection = configuration.GetSection($"ConnectionStrings:{Name}");
            IConfigurationSection adoContextConfigSection = configuration.GetSection($"AdoContextOptions:{Name}");

            ConnectionString = GetValue(adoContextConfigSection, "ConnectionString") ?? connectionStringsSection?.Value;
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
