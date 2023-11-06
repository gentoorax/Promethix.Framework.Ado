using Promethix.Framework.Ado.Enums;
using System.Data;

namespace Promethix.Framework.Ado.Implementation
{
    public class AdoScopeOptions
    {
        public AdoScopeExecutionOption ScopeExecutionOption { get; internal set; }

        public IsolationLevel? DefaultIsolationLevel { get; internal set; }
    }
}