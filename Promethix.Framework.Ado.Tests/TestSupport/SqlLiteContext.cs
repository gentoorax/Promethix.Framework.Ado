using Promethix.Framework.Ado.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethix.Framework.Ado.Tests.TestSupport
{
    public class SqlLiteContext : AdoContext
    {
        public SqlLiteContext()
                : base(
                      "SqlLiteIntegration",
                      "System.Data.SQLite",
                      "Data Source=mydatabase.db;Version=3;"
                      )
        {
            // No Implementation
        }
    }
}
