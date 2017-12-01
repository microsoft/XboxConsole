//------------------------------------------------------------------------------
// <copyright file="XboxConsoleAdapterBase.Debug.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The base class for all XboxConsole adapters.  This class provides a default implementation
    /// for all parts of the Xbox Console API, even if they are not supported by one particular
    /// version of the XDK (in which case an exception is thrown).  It is assumed that the adapter
    /// for each version of the XDK will override the pieces of functionality that are available or
    /// different in that particular build.
    /// </summary>
    internal abstract partial class XboxConsoleAdapterBase : DisposableObject
    {
        /// <summary>
        /// Gets the list of processes running on a console.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="operatingSystem">The Xbox operating system.</param>
        /// <returns>The enumeration of XboxProcessDefinition instances.</returns>
        public IEnumerable<XboxProcessDefinition> GetRunningProcesses(string systemIpAddress, XboxOperatingSystem operatingSystem)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            return this.PerformXdkFunc(
                systemIpAddress,
                () => this.GetRunningProcessesImpl(systemIpAddress, operatingSystem),
                "Failed to get running processes.");
        }

        /// <summary>
        /// Starts debug monitoring of an Xbox process.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="operatingSystem">The Xbox operating system.</param>
        /// <param name="processId">The process Id.</param>
        /// <param name="handler">The handler called when a TextReceived event occurs.</param>
        public void StartDebug(string systemIpAddress, XboxOperatingSystem operatingSystem, uint processId, EventHandler<TextEventArgs> handler)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            this.PerformXdkAction(
                systemIpAddress,
                () => this.StartDebugImpl(systemIpAddress, operatingSystem, processId, handler),
                "Failed to start debug output monitoring.");
        }

        /// <summary>
        /// Stops debug monitoring of an Xbox process.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="operatingSystem">The Xbox operating system.</param>
        /// <param name="processId">The process Id.</param>
        public void StopDebug(string systemIpAddress, XboxOperatingSystem operatingSystem, uint processId)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            this.PerformXdkAction(
                systemIpAddress,
                () => this.StopDebugImpl(systemIpAddress, operatingSystem, processId),
                "Failed to stop debug output monitoring.");
        }

        /// <summary>
        /// Provides the adapter-specific implementation of the "GetRunningProcesses" method.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="operatingSystem">The Xbox operating system.</param>
        /// <returns>The enumeration of XboxProcessDefinition instances.</returns>
        protected virtual IEnumerable<XboxProcessDefinition> GetRunningProcessesImpl(string systemIpAddress, XboxOperatingSystem operatingSystem)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Provides the adapter-specific implementation of the "StartDebug" method.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="operatingSystem">The Xbox operating system.</param>
        /// <param name="processId">The process Id.</param>
        /// <param name="handler">The handler called when a TextReceived event occurs.</param>
        protected virtual void StartDebugImpl(string systemIpAddress, XboxOperatingSystem operatingSystem, uint processId, EventHandler<TextEventArgs> handler)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Provides the adapter-specific implementation of the "StopDebug" method.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="operatingSystem">The Xbox operating system.</param>
        /// <param name="processId">The process Id.</param>
        protected virtual void StopDebugImpl(string systemIpAddress, XboxOperatingSystem operatingSystem, uint processId)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }
    }
}
