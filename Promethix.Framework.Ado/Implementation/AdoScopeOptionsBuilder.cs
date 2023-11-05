using Microsoft.Extensions.Configuration;
using Promethix.Framework.Ado.Enums;
using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethix.Framework.Ado.Implementation
{
    public class AdoScopeOptionsBuilder : OptionsBuilder
    {
        public AdoContextGroupExecutionOption ScopeExecutionOption { get; private set; }

        public IsolationLevel? OverrideDefaultIsolationLevel { get; private set; }

        public AdoScopeOptionsBuilder WithScopeExecutionOption(AdoContextGroupExecutionOption scopeExecutionOption)
        {
            ScopeExecutionOption = scopeExecutionOption;
            return this;
        }

        public AdoScopeOptionsBuilder WithDefaultIsolationLevel(IsolationLevel? isolationLevel)
        {
            OverrideDefaultIsolationLevel = isolationLevel;
            return this;
        }

        public AdoScopeOptionsBuilder WithScopeConfiguration(IConfigurationRoot configuration)
        {
            if (configuration != null)
            {
                TryPopulateConfiguration(configuration);
            }

            return this;
        }

        private void TryPopulateConfiguration(IConfigurationRoot configuration)
        {
            IConfigurationSection adoScopeConfigSection = configuration.GetSection($"AdoScopeOptions");

            // Configure AdoScope level options
            if (adoScopeConfigSection != null)
            {
                ScopeExecutionOption = GetEnumValue(adoScopeConfigSection, nameof(ScopeExecutionOption), ScopeExecutionOption);
                OverrideDefaultIsolationLevel = GetEnumValueOrNull<IsolationLevel>(adoScopeConfigSection, nameof(OverrideDefaultIsolationLevel));
            }
        }
    }
}
