/*
 * MIT License
 * Copyright (c) 2023 Christopher Law
 * https://chrislaw.me
 */
using Promethix.Framework.Ado.Enums;
using System;
using System.Data;
using System.Data.Common;

namespace Promethix.Framework.Ado.Implementation
{
    public abstract class AdoContext : IDisposable
    {
        private bool disposedValue;

        private readonly string name;

        private readonly AdoContextExecutionOption adoContextExecutionOption;

        private readonly IsolationLevel? overrideDefaultIsolationLevel;

        private readonly IDbConnection connection;

        private IDbTransaction transaction;

        public string Name => name;

        public bool IsInTransaction => transaction != null;

        public AdoContextExecutionOption AdoContextExecution => adoContextExecutionOption;

        public IDbConnection Connection
        {
            get
            {
                OpenConnection();
                return connection;
            }
        }

        protected AdoContext(
            string name,
            string providerName,
            string connectionString,
            AdoContextExecutionOption adoContextExecutionOption,
            IsolationLevel? overrideDefaultIsolationLevel = null)
        {
            if (adoContextExecutionOption == AdoContextExecutionOption.NonTransactional && overrideDefaultIsolationLevel.HasValue)
            {
                throw new ArgumentException("Cannot set an override default isolation level for a non-transactional context.", nameof(overrideDefaultIsolationLevel));
            }

            this.name = name;
            this.adoContextExecutionOption = adoContextExecutionOption;
            this.overrideDefaultIsolationLevel = overrideDefaultIsolationLevel;

            DbProviderFactory factory = DbProviderFactories.GetFactory(providerName);

            connection = factory.CreateConnection();
            connection.ConnectionString = connectionString;
        }

        private void OpenConnection()
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
        }

        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            OpenConnection();

            transaction = connection.BeginTransaction(isolationLevel);
        }

        public void BeginTransaction()
        {
            OpenConnection();

            transaction = overrideDefaultIsolationLevel.HasValue ? connection.BeginTransaction(overrideDefaultIsolationLevel.Value) : connection.BeginTransaction();
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
                    // Dispose managed state (managed objects) here
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
            transaction.Rollback();
            transaction.Dispose();
        }
    }
}
