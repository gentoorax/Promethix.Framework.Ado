﻿using Promethix.Framework.Ado.Implementation;

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
