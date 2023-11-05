/*
 * MIT License
 * Copyright (c) 2023 Christopher Law
 * https://chrislaw.me
 */
using Microsoft.Extensions.Configuration;
using Promethix.Framework.Ado.Enums;
using System;
using System.Data;
using System.Data.Common;

namespace Promethix.Framework.Ado.Implementation
{
    public abstract class AdoContext : IDisposable
    {
        private bool disposedValue;

        private IDbConnection connection;

        private AdoContextOptionsBuilder adoContextOptions;

        public bool IsConfigured { get; private set; }

        private IDbTransaction transaction;

        public string Name => adoContextOptions?.Name;

        public bool IsInTransaction => transaction != null;

        public AdoContextExecutionOption AdoContextExecution => adoContextOptions.ExecutionOption;

        public IDbConnection Connection
        {
            get
            {
                OpenConnection();
                return connection;
            }
        }

        protected AdoContext()
        {
            // No Implementation
        }

        protected AdoContext(AdoContextOptionsBuilder adoContextOptions)
        {
            ConfigureContext(adoContextOptions);
        }

        protected AdoContext(
            string name,
            string providerName,
            string connectionString,
            AdoContextExecutionOption adoContextExecutionOption,
            IsolationLevel? overrideDefaultIsolationLevel = null)
        {
            adoContextOptions = new AdoContextOptionsBuilder()
                .WithNamedContext(name)
                .WithProviderName(providerName)
                .WithConnectionString(connectionString)
                .WithExecutionOption(adoContextExecutionOption)
                .WithDefaultIsolationLevel(overrideDefaultIsolationLevel);

            ConfigureContext(adoContextOptions);
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

            transaction = adoContextOptions.OverrideDefaultIsolationLevel.HasValue ? connection.BeginTransaction(adoContextOptions.OverrideDefaultIsolationLevel.Value) : connection.BeginTransaction();
        }

        public void CommitTransaction()
        {
            transaction.Commit();
            transaction.Dispose();
        }

        protected internal void ConfigureContext(AdoContextOptionsBuilder adoContextOptions)
        {
            if (adoContextOptions == null)
            {
                throw new ArgumentNullException(nameof(adoContextOptions));
            }

            if (adoContextOptions.ExecutionOption == AdoContextExecutionOption.NonTransactional && adoContextOptions.OverrideDefaultIsolationLevel.HasValue)
            {
                throw new ArgumentException("Cannot set an override default isolation level for a non-transactional context.");
            }

            this.adoContextOptions = adoContextOptions;
            IsConfigured = true;

            DbProviderFactory factory = DbProviderFactories.GetFactory(adoContextOptions.ProviderName);

            connection = factory.CreateConnection();
            connection.ConnectionString = adoContextOptions.ConnectionString;
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
