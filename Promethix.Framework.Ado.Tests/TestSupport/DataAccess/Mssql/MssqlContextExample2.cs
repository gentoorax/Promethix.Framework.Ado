using Promethix.Framework.Ado.Enums;
using Promethix.Framework.Ado.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethix.Framework.Ado.Tests.TestSupport.DataAccess.Mssql
{
    public class MssqlContextExample2 : AdoContext
    {
        public MssqlContextExample2()
            : base(
                "MssqlContextExample2",
                "Microsoft.Data.SqlClient",
                "Server=(local);Database=AdoScopeTest2;Integrated Security=True;TrustServerCertificate=True",
                AdoContextExecutionOption.NonTransactional)
        {
            // No Implementation
        }
    }
}
