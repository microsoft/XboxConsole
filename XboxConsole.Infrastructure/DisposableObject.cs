//------------------------------------------------------------------------------
// <copyright file="DisposableObject.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Represents a disposable object.
    /// </summary>
    public class DisposableObject : IDisposable
    {
        /// <summary>
        /// Finalizes an instance of the <see cref="DisposableObject" /> class.
        /// </summary>
        ~DisposableObject()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets a value indicating whether the object has been disposed once, protects against double disposal.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// Do not make this method virtual.  A derived class should not be able to override this method.
        /// <summary>
        /// Disposes the current object then suppresses further finalization.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to take this object off the finalization queue 
            // and prevent finalization code for this object from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Throws an ObjectDisposedException if this object has been disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }

        /// <summary>
        /// Standard virtual overload for <see cref="IDisposable"/> pattern.
        /// </summary>
        /// <param name="disposing">
        /// <c>True</c> means this is a call to <see cref="Dispose()"/>.
        /// <c>False</c> means it has been called from the finalizer.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.IsDisposed)
            {
                // Dispose of managed resources (this should be done only when Dispose() has been called explicitly) 
                if (disposing)
                {
                    this.DisposeManagedResources();
                }

                // Dispose the native resources (this should be done when Dispose() is called or when the finalizer is called)
                this.DisposeNativeResources();
            }

            this.IsDisposed = true;
        }

        /// <summary>
        /// Allows derived classes to provide custom dispose handling for managed resources.
        /// </summary>
        /// <remarks>
        /// Derived classes are expected to override this method to dispose their managed resources, then call the base class.
        /// </remarks>
        protected virtual void DisposeManagedResources()
        {
        }

        /// <summary>
        /// Allows derived classes to provide custom dispose handling for native resources.
        /// </summary>
        /// <remarks>
        /// Derived classes are expected to override this method to dispose their native resources, then call the base class.
        /// </remarks>
        protected virtual void DisposeNativeResources()
        {
        }
    }
}
