using Promethix.Framework.Ado.Enums;
using Promethix.Framework.Ado.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethix.Framework.Ado.Tests.TestSupport.DataAccess
{
    public class SqliteContextExample5 : AdoContext
    {
        public SqliteContextExample5()
            : base(
                  "SqliteContextExample4",
                  "Microsoft.Data.Sqlite",
                  "Data Source=mydatabase5.db",
                  AdoContextExecutionOption.NonTransactional
                  )
        {
            // No Implementation
        }
    }
}
