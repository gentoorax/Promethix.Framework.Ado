using Promethix.Framework.Ado.Interfaces;

namespace Promethix.Framework.Ado.Implementation
{
    public class AmbientAdoContextLocator : IAmbientAdoContextLocator
    {
        public TAdoContext GetContext<TAdoContext>() where TAdoContext : AdoContext
        {
            var ambientAdoScope = AdoScope.GetAmbientScope();
            return ambientAdoScope?.AdoContexts.GetContext<TAdoContext>();
        }
    }
}
