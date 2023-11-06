using Promethix.Framework.Ado.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethix.Framework.Ado.Implementation
{
    public class AdoContextGroupOptions
    {
        public AdoContextGroupExecutionOption ExecutionOption { get; private set; }

        public IsolationLevel? DefaultIsolationLevel { get; private set; }

        public AdoContextGroupOptions(AdoScopeOptions adoScopeOptions)
        {
            if (adoScopeOptions == null)
            {
                throw new ArgumentNullException(nameof(adoScopeOptions));
            }

            ExecutionOption = (AdoContextGroupExecutionOption)Enum.Parse(typeof(AdoContextGroupExecutionOption), adoScopeOptions.ScopeExecutionOption.ToString());
            DefaultIsolationLevel = adoScopeOptions.DefaultIsolationLevel;
        }
    }
}
