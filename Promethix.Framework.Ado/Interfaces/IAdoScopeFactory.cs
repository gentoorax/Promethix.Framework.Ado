using Promethix.Framework.Ado.Enums;
using System.Data;

namespace Promethix.Framework.Ado.Interfaces
{
    public interface IAdoScopeFactory
    {
        IAdoScope Create(AdoScopeOption adoScopeOption = AdoScopeOption.JoinExisting);

        IAdoScope CreateWithTransaction(IsolationLevel isolationLevel);
    }
}
