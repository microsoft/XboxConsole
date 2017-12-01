//------------------------------------------------------------------------------
// <copyright file="XboxConsoleAdapter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Adapter.April2014
{
    using System.Net;

    /// <summary>
    /// This class represents an implemenation of the XboxConsoleAdapter
    /// that is supported by the April 2014 version of the XDK.
    /// </summary>
    internal partial class XboxConsoleAdapter : XboxConsoleAdapterBase
    {
        /// <summary>
        /// Initializes a new instance of the XboxConsoleAdapter class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "This is taken care of in this.Dispose(bool)")]
        public XboxConsoleAdapter()
            : base(new XboxXdk())
        {
        }

        /// <summary>
        /// Initializes a new instance of the XboxConsoleAdapter class.
        /// </summary>
        /// <param name="xboxXdk">The XboxXdk functional facade implementation.</param>
        public XboxConsoleAdapter(XboxXdkBase xboxXdk)
            : base(xboxXdk)
        {
        }

        /// <summary>
        /// Gets or sets the default console address.
        /// </summary>
        protected override string DefaultConsoleImpl
        {
            get
            {
                return this.XboxXdk.DefaultConsole;
            }

            set
            {
                this.XboxXdk.DefaultConsole = value;
            }
        }

        /// <summary>
        /// Queries for and returns a value of an Xbox configuration property (see xbconnect command line utility).
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="key">The configuration property name.</param>
        /// <returns>The configuration property value.</returns>
        protected override string GetConfigValueImpl(string systemIpAddress, string key)
        {
            return this.XboxXdk.GetConfigValue(systemIpAddress, key);
        }

        /// <summary>
        /// Sets an Xbox configuration property to the specified value (see xbconnect command line utility).
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="key">The configuration property name.</param>
        /// <param name="value">The configuration property value.</param>
        protected override void SetConfigValueImpl(string systemIpAddress, string key, string value)
        {
            this.XboxXdk.SetConfigValue(systemIpAddress, key, value);
        }

        /// <summary>
        /// Disposes of resources used by this class.
        /// </summary>
        /// <param name="disposing">The indicator whether Dispose() was called explicitly or from the finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                if (disposing)
                {
                    if (this.systemXboxDebugMonitorClient != null)
                    {
                        this.systemXboxDebugMonitorClient.Dispose();
                    }

                    if (this.titleXboxDebugMonitorClient != null)
                    {
                        this.titleXboxDebugMonitorClient.Dispose();
                    }
                }
            }

            base.Dispose(disposing);
        }
    }
}
