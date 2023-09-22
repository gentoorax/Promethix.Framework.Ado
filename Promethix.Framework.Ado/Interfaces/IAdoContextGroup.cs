using Promethix.Framework.Ado.Implementation;
using System;

namespace Promethix.Framework.Ado.Interfaces
{
    public interface IAdoContextGroup : IDisposable
    {
        TAdoContext GetContext<TAdoContext>() where TAdoContext : AdoContext;
    }
}
