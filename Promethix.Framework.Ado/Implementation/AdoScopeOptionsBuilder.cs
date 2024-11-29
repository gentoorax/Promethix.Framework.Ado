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
        public AdoScopeOptions AdoScopeOptions { get; private set; }

        public AdoScopeOptionsBuilder()
        {
            AdoScopeOptions = new AdoScopeOptions();
        }

        public AdoScopeOptionsBuilder WithScopeExecutionOption(AdoContextGroupExecutionOption scopeExecutionOption)
        {
            AdoScopeOptions.ScopeExecutionOption = scopeExecutionOption;
            return this;
        }

        public AdoScopeOptionsBuilder WithDefaultIsolationLevel(IsolationLevel? isolationLevel)
        {
            AdoScopeOptions.IsolationLevel = isolationLevel;
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
                AdoScopeOptions.ScopeExecutionOption = GetEnumValue(adoScopeConfigSection, nameof(AdoScopeOptions.ScopeExecutionOption), AdoScopeOptions.ScopeExecutionOption);
                AdoScopeOptions.IsolationLevel = GetEnumValueOrNull<IsolationLevel>(adoScopeConfigSection, nameof(AdoScopeOptions.IsolationLevel));
            }
        }
    }
}
