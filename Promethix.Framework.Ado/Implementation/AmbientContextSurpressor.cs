using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethix.Framework.Ado.Implementation
{
    public class AmbientContextSurpressor : IDisposable
    {
#pragma warning disable CA2213 // Disposable fields should be disposed - intentionally not disposed only surpressing it.
        private AdoScope savedScope;
#pragma warning restore CA2213 // Disposable fields should be disposed

        private bool disposedValue;

        public AmbientContextSurpressor()
        {
            savedScope = AdoScope.GetAmbientScope();

            // Same as DbContextScope
            // We're hiding the ambient scope but not removing its instance
            // altogether. This is to be tolerant to some programming errors. 
            // 
            // Suppose we removed the ambient scope instance here. If someone
            // was to start a parallel task without suppressing
            // the ambient context and then tried to suppress the ambient
            // context within the parallel task while the original flow
            // of execution was still ongoing (a strange thing to do, I know,
            // but I'm sure this is going to happen), we would end up 
            // removing the ambient context instance of the original flow 
            // of execution from within the parallel flow of execution!
            // 
            // As a result, any code in the original flow of execution
            // that would attempt to access the ambient scope would end up 
            // with a null value since we removed the instance.
            //
            // It would be a fairly nasty bug to track down. So don't let
            // that happen. Hiding the ambient scope (i.e. clearing the CallContext
            // in our execution flow but leaving the ambient scope instance untouched)
            // is safe.
            AdoScope.HideAmbientScope();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (savedScope != null)
                    {
                        AdoScope.SetAmbientScope(savedScope);
                        // savedScope.Dispose(); -- itentionally not disposing - we are only surpressing it.
                        // it's still tracked so will be disposed of when the parent scope is disposed.
                        savedScope = null;
                    }
                }

                disposedValue = true;
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
