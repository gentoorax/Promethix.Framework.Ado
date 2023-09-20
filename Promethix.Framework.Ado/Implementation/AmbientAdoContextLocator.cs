using Promethix.Framework.Ado.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethix.Framework.Ado.Implementation
{
    public class AmbientAdoContextLocator : IAmbientAdoContextLocator
    {
        public TAdoContext GetContext<TAdoContext>() where TAdoContext : AdoContext
        {
            throw new NotImplementedException();
        }
    }
}
