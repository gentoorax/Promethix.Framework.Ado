/*
 * MIT License
 * Copyright (c) 2023 Christopher Law
 * https://chrislaw.me
 */
using Promethix.Framework.Ado.Enums;
using Promethix.Framework.Ado.Interfaces;
using System;
using System.Data;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Promethix.Framework.Ado.Implementation
{
    public class AdoScope : IAdoScope
    {
        private bool disposed;

        private bool completed;

#pragma warning disable IDE0079 // Remove unnecessary suppression - Unnecessary suppression for .NET 4.8 only - we are multi-targeting.
#pragma warning disable CA2213 // Disposable fields should be disposed - This is disposed of through other means.
        private readonly AdoScope parentScope;
#pragma warning restore CA2213 // Disposable fields should be disposed
#pragma warning restore IDE0079 // Remove unnecessary suppression

        private readonly AdoContextGroup adoContexts;

        private readonly InstanceIdentifier instanceIdentifier = new();

        private static readonly AsyncLocal<InstanceIdentifier> ambientDbContextScopeKey = new();

        private readonly bool nested;

        private static readonly ConditionalWeakTable<InstanceIdentifier, AdoScope> adoScopeInstances = new();

        public IAdoContextGroup AdoContexts => adoContexts;

        public AdoScope(IsolationLevel isolationLevel)
            : this(AdoScopeOption.JoinExisting, isolationLevel)
        {
            // No Implementation
        }

        public AdoScope(AdoScopeOption joiningOption)
            : this(joiningOption, null)
        {
            // No Implementation
        }

        public AdoScope()
            : this(AdoScopeOption.JoinExisting, null)
        {
            // No Implementation
        }

        public AdoScope(AdoScopeOption joiningOption, IsolationLevel? isolationLevel)
        {
            if (isolationLevel.HasValue && joiningOption == AdoScopeOption.JoinExisting)
            {
                throw new ArgumentException("Cannot join an ambient AdoScope when an explicit database transaction is required. When requiring explicit database transactions to be used (i.e.when the 'isolationLevel' parameter is set), you must not also ask to join the ambient context (i.e.the 'joinAmbient' parameter must be set to false).");
            }

            disposed = false;
            completed = false;

            parentScope = GetAmbientScope();

            if (parentScope != null && joiningOption == AdoScopeOption.JoinExisting)
            {
                nested = true;
                adoContexts = parentScope.adoContexts;
            }
            else
            {
                nested = false;
                adoContexts = new AdoContextGroup(isolationLevel);
            }

            SetAmbientScope(this);
        }

        public void Complete()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(AdoScope));
            }

            if (completed)
            {
                throw new InvalidOperationException("You cannot call Complete() more than once on an AdoScope. Are you missing a 'using' statement?");
            }

            if (!nested)
            {
                CommitInternal();
            }

            completed = true;
        }

        private void CommitInternal()
        {
            adoContexts.Commit();
        }

        internal static void SetAmbientScope(AdoScope newAmbientScope)
        {
            if (newAmbientScope == null)
            {
                throw new ArgumentNullException(nameof(newAmbientScope));
            }

            InstanceIdentifier current = ambientDbContextScopeKey.Value;

            if (current == newAmbientScope.instanceIdentifier)
            {
                return;
            }

            ambientDbContextScopeKey.Value = newAmbientScope.instanceIdentifier;

            adoScopeInstances.GetValue(newAmbientScope.instanceIdentifier, key => newAmbientScope);
        }

        internal static AdoScope GetAmbientScope()
        {
            InstanceIdentifier current = ambientDbContextScopeKey.Value;

            if (current == null)
            {
                return null;
            }

            if (adoScopeInstances.TryGetValue(current, out AdoScope ambientScope))
            {
                return ambientScope;
            }

            System.Diagnostics.Debug.WriteLine("Programming error detected. Found a reference to an ambient AdoScope in the AsyncLocal (CallContext) but didn't have an instance for it in our adoScopeInstances table. This most likely means that this AdoScope instance wasn't disposed of properly. AdoScope instance must always be disposed. Review the code for any AdoScope instance used outside of a 'using' block and fix it so that all AdoScope instances are disposed of.");
            return null;
        }

        internal static void RemoveAmbientScope()
        {
            InstanceIdentifier identifier = ambientDbContextScopeKey.Value;

            if (identifier == null)
            {
                return;
            }

            if (adoScopeInstances.TryGetValue(identifier, out _))
            {
                adoScopeInstances.Remove(identifier);
            }

            ambientDbContextScopeKey.Value = null;
        }

        /// <summary>
        /// Surpresses the ambient scope for the current async control flow.
        /// </summary>
        internal static void HideAmbientScope()
        {
            ambientDbContextScopeKey.Value = null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (!nested)
                    {
                        if (!completed)
                        {
                            try
                            {
                                RollbackInternal();
                            }
                            catch (Exception ex) when (ex is not ObjectDisposedException)
                            {
                                System.Diagnostics.Debug.WriteLine(ex);
                            }

                            completed = true;
                        }

                        adoContexts.Dispose();
                    }

                    AdoScope currentAmbientScope = GetAmbientScope();

                    if (currentAmbientScope != this)
                    {
                        System.Diagnostics.Debug.WriteLine("Programming error detected. The AdoScope instance being disposed is not the current ambient scope. This most likely means that this AdoScope instance wasn't disposed of properly. AdoScope instance must always be disposed. Review the code for any AdoScope instance used outside of a 'using' block and fix it so that all AdoScope instances are disposed of.");
                    }

                    RemoveAmbientScope();

                    if (parentScope != null)
                    {
                        if (parentScope.disposed)
                        {
                            System.Diagnostics.Debug.WriteLine("Programming error detected. The parent AdoScope instance is already disposed. This most likely means that this AdoScope instance wasn't disposed of properly. AdoScope instance must always be disposed. Review the code for any AdoScope instance used outside of a 'using' block and fix it so that all AdoScope instances are disposed of.");
                        }   
                        else
                        {
                            SetAmbientScope(parentScope);
                        }
                    }
                }

                // Unmanaged resources are disposed of here. If we have any

                disposed = true;
            }
        }

        private void RollbackInternal()
        {
            adoContexts.Rollback();
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
