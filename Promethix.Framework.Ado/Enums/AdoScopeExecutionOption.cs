using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethix.Framework.Ado.Enums
{
    public enum AdoScopeExecutionOption
    {
        /// <summary>
        /// The default behavior of the scope. Will not be promoted to a distributed transaction.
        /// </summary>
        Standard,

        /// <summary>
        /// Implicit distributed transactions will be enabled at the scope level. You want this if working
        /// with a lot of databases and you want to enable unit of work / transactions for all operations
        /// within all scopes.
        /// This uses TransactionScope under the hood and TransactionScope with cover the AdoScope
        /// regardless of the AdoContext configuration.
        /// WARNING: This is advanced behaviour only enabled it if you know what you're doing.
        /// Requires MSDTC or ADO supported DTC controller.
        /// </summary>
        Distributed
    }
}
