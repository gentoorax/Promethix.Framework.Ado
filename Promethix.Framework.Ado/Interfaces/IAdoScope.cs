using System;

namespace Promethix.Framework.Ado.Interfaces
{
    public interface IAdoScope : IDisposable
    {
        void Complete();
    }
}
