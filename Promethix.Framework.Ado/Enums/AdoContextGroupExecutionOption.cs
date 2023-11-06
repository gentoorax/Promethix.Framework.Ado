using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethix.Framework.Ado.Enums
{
    /// <summary>
    /// Sets the internal behaviour of the AdoContextGroup.
    /// </summary>
    public enum AdoContextGroupExecutionOption
    {
        /// <summary>
        /// The default behavior of the AdoContextGroup. Will not be promoted to a distributed transaction.
        /// </summary>
        Standard,

        /// <summary>
        /// Encapsulates the AdoContextGroup in a distributed transaction.
        /// Requires MSDTC or ADO supported DTC controller.
        /// Used for explicit distributed transactions and AdoScope with Distributed execution option.
        /// </summary>
        Distributed
    }
}
