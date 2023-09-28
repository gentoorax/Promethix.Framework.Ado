using Promethix.Framework.Ado.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethix.Framework.Ado.Interfaces
{
    public interface IAdoContextOptionsRegistry
    {
        void AddContextOptions<TAdoContext>(AdoContextOptionsBuilder optionsBuilder);

        bool TryGetContextOptions<TAdoContext>(out AdoContextOptionsBuilder optionsBuilder);
    }
}
