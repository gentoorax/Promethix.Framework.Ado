using Promethix.Framework.Ado.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace Promethix.Framework.Ado.Implementation
{
    public class AdoContextGroup : IAdoContextGroup
    {
        private bool disposed;

        private readonly Dictionary<Type, AdoContext> initialisedAdoContexts;

        private readonly IsolationLevel? isolationLevel;

        private bool completed;

        private ExceptionDispatchInfo lastError;

        public AdoContextGroup(IsolationLevel? isolationLevel = null)
        {
            disposed = false;
            completed = false;
            initialisedAdoContexts = new Dictionary<Type, AdoContext>();
            this.isolationLevel = isolationLevel;
        }

        public TAdoContext GetContext<TAdoContext>() where TAdoContext : AdoContext
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(AdoContextGroup));
            }

            if (!initialisedAdoContexts.ContainsKey(typeof(TAdoContext)))
            {
                // First time we have been asked for this type of context, so create it.
                // Create one, cache it and start its database transaction if needed.
                TAdoContext adoContext = Activator.CreateInstance<TAdoContext>();

                initialisedAdoContexts.Add(typeof(TAdoContext), adoContext);

                // TODO: Implement ReadOnly

                if (isolationLevel.HasValue)
                {
                    adoContext.BeginTransaction(isolationLevel.Value);
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
                    // TODO: dispose managed state (managed objects)
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
