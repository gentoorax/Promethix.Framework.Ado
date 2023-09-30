/*
 * MIT License
 * Copyright (c) 2023 Christopher Law
 * https://chrislaw.me
 */
using Promethix.Framework.Ado.Enums;
using Promethix.Framework.Ado.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.ExceptionServices;

namespace Promethix.Framework.Ado.Implementation
{
    public class AdoContextGroup : IAdoContextGroup
    {
        private bool disposed;

        private readonly Dictionary<Type, AdoContext> initialisedAdoContexts;

        private readonly IAdoContextOptionsRegistry adoContextOptionsRegistry;

        private readonly IsolationLevel? isolationLevel;

        private bool completed;

        private ExceptionDispatchInfo lastError;

        public AdoContextGroup(IAdoContextOptionsRegistry adoContextOptionsRegistry, IsolationLevel? isolationLevel = null)
        {
            disposed = false;
            completed = false;
            initialisedAdoContexts = new Dictionary<Type, AdoContext>();
            this.adoContextOptionsRegistry = adoContextOptionsRegistry;
            this.isolationLevel = isolationLevel;
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
                if (isolationLevel.HasValue)
                {
                    // Explicit transaction requested.
                    adoContext.BeginTransaction(isolationLevel.Value);
                }
                else if (adoContext.AdoContextExecution == AdoContextExecutionOption.Transactional)
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

            completed = true;

            lastError?.Throw();
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
