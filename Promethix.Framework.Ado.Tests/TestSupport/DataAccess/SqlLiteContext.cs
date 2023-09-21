using Promethix.Framework.Ado.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethix.Framework.Ado.Tests.TestSupport.DataAccess
{
    public class SqlLiteContext : AdoContext
    {
        public SqlLiteContext()
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
