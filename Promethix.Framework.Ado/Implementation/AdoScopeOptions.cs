using Promethix.Framework.Ado.Enums;
using System.Data;

namespace Promethix.Framework.Ado.Implementation
{
    public class AdoScopeOptions
    {
        public AdoScopeOptions()
        {
            JoinOption = AdoScopeOption.JoinExisting;
            ScopeExecutionOption = AdoContextGroupExecutionOption.Standard;
        }

        public AdoScopeOption JoinOption { get; internal set; }

        public AdoContextGroupExecutionOption ScopeExecutionOption { get; internal set; }

        public IsolationLevel? IsolationLevel { get; internal set; }
    }
}