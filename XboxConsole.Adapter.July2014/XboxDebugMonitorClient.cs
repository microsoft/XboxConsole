//------------------------------------------------------------------------------
// <copyright file="XboxDebugMonitorClient.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox
{
    using System;
    using Microsoft.Xbox.XTF.Diagnostics;

    /// <summary>
    /// XDK-agnostic wrapper of Xtf's DebugMonitorClient.
    /// </summary>
    internal class XboxDebugMonitorClient : DisposableObject, IXboxDebugMonitorClient
    {
        /// <summary>
        /// Initializes a new instance of the XboxDebugMonitorClient class.
        /// </summary>
        /// <param name="address">The address used to instantiate the underlying DebugMonitorClient.</param>
        internal XboxDebugMonitorClient(string address)
        {
            this.Client = new DebugMonitorClient(address);
        }

        private DebugMonitorClient Client { get; set; }

        /// <summary>
        /// Starts debug monitoring.
        /// </summary>
        /// <param name="processId">The Xbox Console process Id.</param>
        /// <param name="handler">The text event handler.</param>
        public void Start(uint processId, EventHandler<TextEventArgs> handler)
        {
            this.ThrowIfDisposed();
            this.Client.Start(
                processId, 
                StartFlags.None,
                (sender, eventArgs) =>
                {
                    if (handler != null)
                    {
                        handler(sender, new TextEventArgs(eventArgs.DebugString));
                    }
                });
        }

        /// <summary>
        /// Stops debug monitoring.
        /// </summary>
        /// <param name="processId">The Xbox Console process Id.</param>
        public void Stop(uint processId)
        {
            this.ThrowIfDisposed();
            this.Client.Stop(processId, StopFlags.None);
        }

        /// <summary>
        /// Disposes of managed resources used by this class.
        /// </summary>
        protected override void DisposeManagedResources()
        {
            if (this.Client != null)
            {
                this.Client.Dispose();
            }

            base.DisposeManagedResources();
        }
    }
}
