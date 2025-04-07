using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethix.Framework.Ado.Exceptions
{
    public class AdoScopeException : InvalidOperationException
    {
        public AdoScopeException(string message) : base(message) { }

        public AdoScopeException(string message, Exception innerException)
            : base(message, innerException) { }

        public AdoScopeException()
        {
        }
    }

}
