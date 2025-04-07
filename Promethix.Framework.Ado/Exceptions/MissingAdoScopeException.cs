using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethix.Framework.Ado.Exceptions
{
    public class MissingAdoScopeException : AdoScopeException
    {
        public MissingAdoScopeException(Type contextType)
            : base($"""
            AdoScope was not detected in the current execution context.
            You likely accessed `{contextType?.Name ?? "UnknownContext"}` outside of a valid scope.

            Ensure all database operations are wrapped with:
            `using var scope = adoScopeFactory.Create();`
            """)
        {
            if (contextType == null)
            {
                throw new ArgumentNullException(nameof(contextType));
            }
        }

        public MissingAdoScopeException()
        {
        }

        public MissingAdoScopeException(string message) : base(message)
        {
        }

        public MissingAdoScopeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
