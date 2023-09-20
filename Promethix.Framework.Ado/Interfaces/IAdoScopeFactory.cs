using Promethix.Framework.Ado.Enums;

namespace Promethix.Framework.Ado.Interfaces
{
    public interface IAdoScopeFactory
    {
        IAdoScope Create(AdoScopeOption adoScopeOption);
    }
}
