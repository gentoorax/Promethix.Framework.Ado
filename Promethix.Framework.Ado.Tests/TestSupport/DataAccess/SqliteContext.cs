using Promethix.Framework.Ado.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethix.Framework.Ado.Tests.TestSupport.DataAccess
{
    public class SqliteContext : AdoContext
    {
        public SqliteContext()
                : base(
                      "SqlLiteIntegration",
                      "Microsoft.Data.Sqlite",
                      "Data Source=mydatabase.db"
                      )
        {
            // No Implementation
        }
    }
}
