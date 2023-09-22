using Promethix.Framework.Ado.Implementation;

namespace Promethix.Framework.Ado.Interfaces
{
    public interface IAmbientAdoContextLocator
    {
        TAdoContext GetContext<TAdoContext>() where TAdoContext : AdoContext;
    }
}
