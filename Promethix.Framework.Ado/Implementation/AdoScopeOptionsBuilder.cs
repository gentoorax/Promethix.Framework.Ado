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
    public class AdoScopeOptionsBuilder
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
    }
}
