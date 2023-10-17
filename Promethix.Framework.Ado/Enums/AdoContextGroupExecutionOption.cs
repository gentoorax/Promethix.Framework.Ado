using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethix.Framework.Ado.Enums
{
    public enum AdoContextGroupExecutionOption

    {
        /// <summary>
        /// The default behavior of the scope. Will not be promoted to a distributed transaction.
        /// </summary>
        Standard,

        /// <summary>
        /// Encapsulates the scope in a distributed transaction (Requires MSDTC or ADO supported DTC controller).
        /// </summary>
        Distributed
    }
}
