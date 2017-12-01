//------------------------------------------------------------------------------
// <copyright file="XboxConsoleAdapterBase.ConsoleControl.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Adapter
{
    using System;
    using Microsoft.Internal.GamesTest.Utilities;

    /// <summary>
    /// A thin layer over the XTF provided managed API.
    /// </summary>
    public abstract partial class XboxConsoleAdapterBase : DisposableObject
    {
        /// <summary>Reboot the Xbox console and wait no more than the specified amount of time for the console to become responsive again.</summary>
        /// <param name="timeout">The maximum amount of time to wait for the Xbox console to become
        /// responsive again after initiating the reboot sequence.</param>
        /// <exception cref="System.TimeoutException">Thrown if the reboot operation does not complete within the given timeout period.</exception>
        public virtual void Reboot(TimeSpan timeout)
        {
            throw new NotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Shutdown the Xbox console and wait no more than the specified amount of time for the operation to complete.
        /// </summary>
        /// <param name="timeout">The maximum amount of time to wait for the shutdown process to complete and for the Xbox console to become unresponsive.</param>
        /// <exception cref="System.TimeoutException">Thrown if the shutdown operation does not complete within the given timeout period.</exception>
        public virtual void Shutdown(TimeSpan timeout)
        {
            throw new NotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Immediately after initiating a Shutdown or Reboot request, the console remains responsive
        /// for a short period of time.  This method is intended to block until the kit is verified
        /// to no longer be responsive.
        /// </summary>
        /// <param name="timeout">The maximum amount of time to wait for the console to become unresponsive.</param>
        /// <exception cref="System.OperationCanceledException">Thrown when the caller cancels this waiting operation.</exception>
        protected virtual void WaitUntilConsoleIsUnresponsive(TimeSpan timeout)
        {
            throw new NotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// The XTF APIs do not offer a way to synchronously reboot a console therefore, we need to implement
        /// our own logic to wait for a console to finish rebooting.  This method should block until it has
        /// confirmed that the console has completed rebooting or the timeout has been reached or exceeded.
        /// </summary>
        /// <param name="timeout">The maximum amount of time to wait for the console to finish rebooting.</param>
        /// <exception cref="System.OperationCanceledException">Thrown when the caller cancels this waiting operation.</exception>
        protected virtual void WaitUntilRebootIsComplete(TimeSpan timeout)
        {
            throw new NotSupportedException(NotSupportedMessage);
        }
    }
}
