//------------------------------------------------------------------------------
// <copyright file="XboxConsoleAdapter.Debug.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Adapter.May2014
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This class represents an implemenation of the XboxConsoleAdapter
    /// that is supported by the May 2014 version of the XDK.
    /// </summary>
    internal partial class XboxConsoleAdapter : XboxConsoleAdapterBase
    {
        private IXboxDebugMonitorClient systemXboxDebugMonitorClient;
        private IXboxDebugMonitorClient titleXboxDebugMonitorClient;

        /// <summary>
        /// Provides the adapter-specific implementation of the "GetRunningProcesses" method.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="operatingSystem">The Xbox operating system.</param>
        /// <returns>The enumeration of XboxProcessDefinition instances.</returns>
        protected override IEnumerable<XboxProcessDefinition> GetRunningProcessesImpl(string systemIpAddress, XboxOperatingSystem operatingSystem)
        {
            return this.XboxXdk.GetRunningProcesses(systemIpAddress, operatingSystem);
        }

        /// <summary>
        /// Provides the adapter-specific implementation of the "StartDebug" method.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="operatingSystem">The Xbox operating system.</param>
        /// <param name="processId">The process Id.</param>
        /// <param name="handler">The handler called when a TextReceived event occurs.</param>
        protected override void StartDebugImpl(string systemIpAddress, XboxOperatingSystem operatingSystem, uint processId, EventHandler<TextEventArgs> handler)
        {
            switch (operatingSystem)
            {
                case XboxOperatingSystem.System:
                    if (this.systemXboxDebugMonitorClient == null)
                    {
                        this.systemXboxDebugMonitorClient = this.XboxXdk.CreateDebugMonitorClient(systemIpAddress, XboxOperatingSystem.System);
                    }

                    this.systemXboxDebugMonitorClient.Start(processId, handler);
                    break;

                case XboxOperatingSystem.Title:
                    if (this.titleXboxDebugMonitorClient == null)
                    {
                        this.titleXboxDebugMonitorClient = this.XboxXdk.CreateDebugMonitorClient(systemIpAddress, XboxOperatingSystem.Title);
                    }

                    this.titleXboxDebugMonitorClient.Start(processId, handler);
                    break;

                default:
                    throw new ArgumentException("This operating system is not supported.");
            }
        }

        /// <summary>
        /// Provides the adapter-specific implementation of the "StopDebug" method.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="operatingSystem">The Xbox operating system.</param>
        /// <param name="processId">The process Id.</param>
        protected override void StopDebugImpl(string systemIpAddress, XboxOperatingSystem operatingSystem, uint processId)
        {
            switch (operatingSystem)
            {
                case XboxOperatingSystem.System:
                    if (this.systemXboxDebugMonitorClient == null)
                    {
                        this.systemXboxDebugMonitorClient = this.XboxXdk.CreateDebugMonitorClient(systemIpAddress, XboxOperatingSystem.System);
                    }

                    this.systemXboxDebugMonitorClient.Stop(processId);
                    break;

                case XboxOperatingSystem.Title:
                    if (this.titleXboxDebugMonitorClient == null)
                    {
                        this.titleXboxDebugMonitorClient = this.XboxXdk.CreateDebugMonitorClient(systemIpAddress, XboxOperatingSystem.Title);
                    }

                    this.titleXboxDebugMonitorClient.Stop(processId);
                    break;

                default:
                    throw new ArgumentException("This operating system is not supported.");
            }
        }
    }
}
