using Promethix.Framework.Ado.Enums;
using Promethix.Framework.Ado.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethix.Framework.Ado.Tests.TestSupport.DataAccess.Mssql
{
    public class MssqlContextExample1 : AdoContext
    {
        public MssqlContextExample1()
            : base(
                  "MssqlContextExample1",
                  "Microsoft.Data.SqlClient",
                  "Server=(local);Database=AdoScopeTest1;Integrated Security=True;TrustServerCertificate=True",
                  AdoContextExecutionOption.NonTransactional
                  )
        {
            // No Implementation
        }
    }
}
