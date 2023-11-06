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
        public AdoScopeOptionsBuilder AdoScopeOptionsBuilder { get; private set; }

        public AdoScopeExecutionOption AdoScopeExecutionOption { get; private set; }

        public bool HasExplicitOverrides { get; private set;}

        public AdoScopeOverrideOptionsBuilder(AdoScopeOptionsBuilder adoScopeOptionsBuilder)
        {
            AdoScopeOptionsBuilder = adoScopeOptionsBuilder;
        }

        public AdoScopeOverrideOptionsBuilder WithAdoScopeExecutionOption(AdoScopeExecutionOption adoScopeExecutionOption)
        {
            HasExplicitOverrides = true;
            AdoScopeExecutionOption = adoScopeExecutionOption;
            return this;
        }

        public AdoScopeOverrideOptionsBuilder WithDefaultIsolationLevel(IsolationLevel? isolationLevel)
        {
            HasExplicitOverrides = true;
            AdoScopeOptionsBuilder.WithDefaultIsolationLevel(isolationLevel);
            return this;
        }
    }
}
