using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethix.Framework.Ado.Implementation
{
    public abstract class AdoContext : IDisposable
    {
        private bool disposedValue;

        private readonly string name;

        private readonly IDbConnection connection;

        private IDbTransaction transaction;

        public string Name => name;

        public bool IsInTransaction => transaction != null;

        protected AdoContext(string name, string providerName, string connectionString)
        {
            this.name = name;

            DbProviderFactory factory = DbProviderFactories.GetFactory(providerName);

            connection = factory.CreateConnection();
            connection.ConnectionString = connectionString;
        }

        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            transaction = connection.BeginTransaction(isolationLevel);
        }

        public void CommitTransaction()
        {
            transaction.Commit();
            transaction.Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                transaction?.Dispose();
                connection?.Close();
                connection?.Dispose();

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        internal void RollbackTransaction()
        {
            throw new NotImplementedException();
        }
    }
}
