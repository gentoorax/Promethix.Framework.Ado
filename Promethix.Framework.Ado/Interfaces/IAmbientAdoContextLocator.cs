using Promethix.Framework.Ado.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethix.Framework.Ado.Interfaces
{
    public interface IAmbientAdoContextLocator
    {
        TAdoContext GetContext<TAdoContext>() where TAdoContext : AdoContext;
    }
}
