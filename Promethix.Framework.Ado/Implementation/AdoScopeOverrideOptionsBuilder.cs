using Promethix.Framework.Ado.Enums;
using System.Data;

namespace Promethix.Framework.Ado.Implementation
{
    public class AdoScopeOverrideOptionsBuilder(AdoScopeOptions adoScopeOptions)
    {
        public AdoScopeOptions AdoScopeOptions { get; private set; } = adoScopeOptions;

        public bool HasExplicitOverrides { get; private set; }

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
