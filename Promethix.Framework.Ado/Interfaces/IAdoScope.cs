using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethix.Framework.Ado.Interfaces
{
    public interface IAdoScope : IDisposable
    {
        void Complete();
    }
}
