using Promethix.Framework.Ado.Enums;
using Promethix.Framework.Ado.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethix.Framework.Ado.Implementation
{
    public class AdoScopeFactory : IAdoScopeFactory
    {
        public IAdoScope Create(AdoScopeOption adoScopeOption = AdoScopeOption.JoinExisting)
        {
            return new AdoScope(adoScopeOption);
        }

        public IAdoScope CreateWithTransaction(IsolationLevel isolationLevel)
        {
            return new AdoScope(AdoScopeOption.ForceCreateNew, isolationLevel);
        }
    }
}
