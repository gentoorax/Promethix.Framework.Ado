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
        public AdoScopeOptions adoScopeOptions { get; private set; }

        public AdoScopeOptionsBuilder()
        {
            adoScopeOptions = new AdoScopeOptions();
        }

        public AdoScopeOptionsBuilder WithScopeExecutionOption(AdoScopeExecutionOption scopeExecutionOption)
        {
            adoScopeOptions.ScopeExecutionOption = scopeExecutionOption;
            return this;
        }

        public AdoScopeOptionsBuilder WithDefaultIsolationLevel(IsolationLevel? isolationLevel)
        {
            adoScopeOptions.DefaultIsolationLevel = isolationLevel;
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
                adoScopeOptions.ScopeExecutionOption = GetEnumValue(adoScopeConfigSection, nameof(adoScopeOptions.ScopeExecutionOption), adoScopeOptions.ScopeExecutionOption);
                adoScopeOptions.DefaultIsolationLevel = GetEnumValueOrNull<IsolationLevel>(adoScopeConfigSection, nameof(adoScopeOptions.DefaultIsolationLevel));
            }
        }
    }
}
