/*
 * MIT License
 * Copyright (c) 2023 Christopher Law
 * https://chrislaw.me
 */
using Promethix.Framework.Ado.Enums;
using Promethix.Framework.Ado.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
#if NET7_0_OR_GREATER
using System.Runtime.InteropServices;
#endif
using System.Transactions;

namespace Promethix.Framework.Ado.Implementation
{
    public class AdoContextGroup : IAdoContextGroup
    {
        private bool disposed;

        private readonly Dictionary<Type, AdoContext> initialisedAdoContexts;

        private readonly IAdoContextOptionsRegistry adoContextOptionsRegistry;

        private readonly AdoScopeOptions adoScopeOptions;

        private readonly TransactionScope transactionScope;

        private bool completed;

        private ExceptionDispatchInfo lastError;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0016:Use 'throw' expression", Justification = "Multi-targeted not supported in .NET4.8!")]
        public AdoContextGroup(IAdoContextOptionsRegistry adoContextOptionsRegistry, AdoScopeOptions adoScopeOptions)
        {
#pragma warning disable CA1510 // Use ArgumentNullException throw helper. Multi-targeted not supported in .NET4.8!
            if (adoScopeOptions == null)
            {
                throw new ArgumentNullException(nameof(adoScopeOptions));
            }
#pragma warning restore CA1510 // Use ArgumentNullException throw helper

            disposed = false;
            completed = false;
            initialisedAdoContexts = [];
            this.adoContextOptionsRegistry = adoContextOptionsRegistry;
            this.adoScopeOptions = adoScopeOptions;

            // Prepare distributed transaction if requested.
            // Gets a bit messy here, because .NET 5 and .NET 6 don't support distributed transactions.
            // .NET 7 or better support distributed transactions, but only on Windows!
#if NET7_0_OR_GREATER
            // For .NET 7 and greater, we can use the new TransactionManager.ImplicitDistributedTransactions property.
            // Note this is only support on Windows AFAIK.
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                TransactionManager.ImplicitDistributedTransactions = true;
            }
#endif
#if NET5 || NET6
            if (adoScopeOptions.ScopeExecutionOption == AdoContextGroupExecutionOption.Distributed)
            {
                throw new NotImplementedException("Cannot use distributed transactions with .NET 5 or .NET 6. Please use .NET 7 or greater. Windows is also likely required with MSDTC enabled.");
            }
#endif
            if (adoScopeOptions.ScopeExecutionOption == AdoContextGroupExecutionOption.Distributed)
            {
                TransactionOptions transactionOptions = adoScopeOptions.IsolationLevel.HasValue
                    ? new TransactionOptions { IsolationLevel = (System.Transactions.IsolationLevel)adoScopeOptions.IsolationLevel.Value }
                    : new TransactionOptions();

                transactionScope = new TransactionScope(TransactionScopeOption.Required, transactionOptions);
            }
        }

        public TAdoContext GetContext<TAdoContext>() where TAdoContext : AdoContext
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(AdoContextGroup));
            }

            // Determine whether we need to create a new context or use an existing one from our cache.
            if (!initialisedAdoContexts.ContainsKey(typeof(TAdoContext)))
            {
                // First time we have been asked for this type of context, so create it.
                // Create one, cache it and start its database transaction if needed.
                TAdoContext adoContext = Activator.CreateInstance<TAdoContext>();

                // If we have some configuration options for this context type, apply them.
                if ((adoContextOptionsRegistry?.TryGetContextOptions<TAdoContext>(out AdoContextOptionsBuilder options) != null) && options != null)
                {
                    adoContext.ConfigureContext(options);
                }

                initialisedAdoContexts.Add(typeof(TAdoContext), adoContext);

                // TODO: Implement ReadOnly

                // Start transaction if requested.
                if (adoScopeOptions.IsolationLevel.HasValue)
                {
                    // Explicit transaction requested. (Could also be distributed if requested. E.g. AdoContextGroupExecutionOption.Distributed)
                    adoContext.BeginTransaction(adoScopeOptions.IsolationLevel.Value);
                }
                else if (adoContext.AdoContextExecution == AdoContextExecutionOption.Transactional
                    || adoScopeOptions.ScopeExecutionOption == AdoContextGroupExecutionOption.Distributed)
                {
                    // Implicit transaction requested via ADO Context configuration.
                    adoContext.BeginTransaction();
                }
            }

            return initialisedAdoContexts[typeof(TAdoContext)] as TAdoContext;
        }

        public void Commit()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(AdoContextGroup));
            }

            if (completed)
            {
                throw new InvalidOperationException("This AdoContextCollection has already been completed. You can't call Commit() or Rollback() more than once on an AdoContextCollection.");
            }

            CommitInternal();

            completed = true;

            lastError?.Throw();
        }

        private void CommitInternal()
        {
            foreach (AdoContext adoContext in initialisedAdoContexts.Values)
            {
                try
                {
                    // TODO: Implement ReadOnly

                    // If we started a transaction, commit it.
                    if (adoContext.IsInTransaction)
                    {
                        adoContext.CommitTransaction();
                    }
                }
                catch (Exception ex) when (ex is not ObjectDisposedException)
                {
                    lastError = ExceptionDispatchInfo.Capture(ex);
                }
            }

            if (adoScopeOptions.ScopeExecutionOption == AdoContextGroupExecutionOption.Distributed)
            {
                transactionScope.Complete();
                transactionScope.Dispose();
            }
        }

        public void Rollback()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(AdoContextGroup));
            }

            if (completed)
            {
                throw new InvalidOperationException("This AdoContextCollection has already been completed. You can't call Commit() or Rollback() more than once on an AdoContextCollection.");
            }

            foreach (AdoContext adoContext in initialisedAdoContexts.Values)
            {
                try
                {
                    if (adoContext.IsInTransaction)
                    {
                        adoContext.RollbackTransaction();
                    }
                }
                catch (Exception ex) when (ex is not ObjectDisposedException)
                {
                    lastError = ExceptionDispatchInfo.Capture(ex);
                }
            }

            completed = true;

            lastError?.Throw();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Dispose managed state (managed objects)
                    if (!completed)
                    {
                        try
                        {
                            Rollback();
                        }
                        catch (Exception ex) when (ex is not ObjectDisposedException)
                        {
                            lastError = ExceptionDispatchInfo.Capture(ex);
                        }
                    }

                    foreach (AdoContext adoContext in initialisedAdoContexts.Values)
                    {
                        try
                        {
                            adoContext.Dispose();
                        }
                        catch (Exception ex) when (ex is not ObjectDisposedException)
                        {
                            lastError = ExceptionDispatchInfo.Capture(ex);
                        }
                    }

                    transactionScope?.Dispose();
                    initialisedAdoContexts.Clear();
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
