using Promethix.Framework.Ado.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethix.Framework.Ado.Implementation
{
    public class AdoScopeOverrideOptionsBuilder
    {
        public AdoScopeOptions AdoScopeOptions { get; private set; }

        public bool HasExplicitOverrides { get; private set;}

        public AdoScopeOverrideOptionsBuilder(AdoScopeOptions adoScopeOptions)
        {
            AdoScopeOptions = adoScopeOptions;
        }

        public AdoScopeOverrideOptionsBuilder WithAdoScopeExecutionOption(AdoContextGroupExecutionOption adoScopeExecutionOption)
        {
            HasExplicitOverrides = true;
            AdoScopeOptions.ScopeExecutionOption = adoScopeExecutionOption;
            return this;
        }

        public AdoScopeOverrideOptionsBuilder WithIsolationLevel(IsolationLevel? isolationLevel)
        {
            HasExplicitOverrides = true;
            AdoScopeOptions.IsolationLevel = isolationLevel;
            return this;
        }

        public AdoScopeOverrideOptionsBuilder WithJoinOption(AdoScopeOption joinOption)
        {
            // Default is JoinExisting
            if (joinOption != AdoScopeOption.JoinExisting)
            {
                HasExplicitOverrides = true;
            }

            AdoScopeOptions.JoinOption = joinOption;
            return this;
        }
    }
}
