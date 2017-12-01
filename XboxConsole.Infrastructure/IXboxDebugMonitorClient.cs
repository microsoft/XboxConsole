//------------------------------------------------------------------------------
// <copyright file="IXboxDebugMonitorClient.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox
{
    using System;

    /// <summary>
    /// XDK-agnostic wrapper of Xtf's DebugMonitorClient.
    /// </summary>
    internal interface IXboxDebugMonitorClient : IDisposable
    {
        /// <summary>
        /// Starts debug monitoring.
        /// </summary>
        /// <param name="processId">The Xbox Console process Id.</param>
        /// <param name="handler">The text event handler.</param>
        void Start(uint processId, EventHandler<TextEventArgs> handler);

        /// <summary>
        /// Stops debug monitoring.
        /// </summary>
        /// <param name="processId">The Xbox Console process Id.</param>
        void Stop(uint processId);
    }
}
