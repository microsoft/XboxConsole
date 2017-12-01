//------------------------------------------------------------------------------
// <copyright file="XboxConsoleAdapterBase.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using Microsoft.Internal.GamesTest.Xbox.Input;

    /// <summary>
    /// The base class for all XboxConsole adapters.  This class provides default a implementation
    /// for all parts of the Xbox Console API, even if they are not supported by one particular
    /// version of the XDK (in which case an exception is thrown).  It is assumed that the adapter
    /// for each version of the XDK will override the pieces of functionality that are available or
    /// different in that particular build.
    /// </summary>
    internal abstract partial class XboxConsoleAdapterBase : DisposableObject
    {
        private const string NotSupportedMessage = "The feature that you are trying to use is not supported by the version of the XDK installed on your machine.";

        /// <summary>
        /// Initializes a new instance of the XboxConsoleAdapterBase class.
        /// </summary>
        /// <param name="xboxXdk">The XboxXdk functional facade implementation.</param>
        /// <exception cref="ArgumentNullException">Thrown if either argument is null or empty.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Required by design.")]
        protected XboxConsoleAdapterBase(XboxXdkBase xboxXdk)
        {
            if (xboxXdk == null)
            {
                throw new ArgumentNullException("xboxXdk");
            }

            this.XboxXdk = xboxXdk;
            this.ThrowIfXdkNotInstalled();
        }

        /// <summary>
        /// Gets or sets the default console address.
        /// </summary>
        public string DefaultConsole
        {
            get
            {
                this.ThrowIfDisposed();

                return this.PerformXdkFunc(
                    null,
                    () => this.DefaultConsoleImpl,
                    "Failed to get the default console address.");
            }

            set
            {
                this.ThrowIfDisposed();

                this.PerformXdkAction(
                    null,
                    () => this.DefaultConsoleImpl = value,
                    "Failed to set the default console address.");
            }
        }

        /// <summary>
        /// Gets the XboxXdk functional facade implementation.
        /// </summary>
        protected XboxXdkBase XboxXdk { get; private set; }

        /// <summary>
        /// Gets the path to the root folder of XDK.
        /// </summary>
        protected virtual string PathToXdk
        {
            get 
            {
                return Environment.GetEnvironmentVariable("DurangoXdk");
            }
        }

        /// <summary>
        /// Gets the path to the "bin" folder of XDK.
        /// </summary>
        protected virtual string PathToXdkBin
        {
            get
            {
                return Path.Combine(this.PathToXdk, "bin");
            }
        }

        /// <summary>
        /// Gets or sets the default console address.
        /// </summary>
        protected virtual string DefaultConsoleImpl
        {
            get
            {
                throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
            }

            set
            {
                throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
            }
        }

        /// <summary>
        /// Queries for and returns a value of an Xbox configuration property (see xbconnect command line utility).
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="key">The configuration property name.</param>
        /// <returns>The configuration property value.</returns>
        public string GetConfigValue(string systemIpAddress, string key)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key");
            }

            return this.PerformXdkFunc(
                systemIpAddress,
                () => this.GetConfigValueImpl(systemIpAddress, key),
                "Failed to get the configuration property.");
        }

        /// <summary>
        /// Sets an Xbox configuration property to the specified value (see xbconnect command line utility).
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="key">The configuration property name.</param>
        /// <param name="value">The configuration property value.</param>
        public void SetConfigValue(string systemIpAddress, string key, string value)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress); 
            
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            
            this.PerformXdkAction(
                systemIpAddress,
                () => this.SetConfigValueImpl(systemIpAddress, key, value),
                "Failed to set the configuration property.");
        }

        /// <summary>
        /// Gets information about the console.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <returns>A XboxConsoleInfo containing information about the console.</returns>
        public XboxConsoleInfo GetConsoleInfo(string systemIpAddress)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            return this.PerformXdkFunc(
                systemIpAddress,
                () => this.GetConsoleInfoImpl(systemIpAddress),
                "Failed to get the console information.");
        }

        /// <summary>
        /// Queries for and returns a value of an Xbox configuration property (see xbconnect command line utility).
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="key">The configuration property name.</param>
        /// <returns>The configuration property value.</returns>
        protected virtual string GetConfigValueImpl(string systemIpAddress, string key)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Sets an Xbox configuration property to the specified value (see xbconnect command line utility).
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="key">The configuration property name.</param>
        /// <param name="value">The configuration property value.</param>
        protected virtual void SetConfigValueImpl(string systemIpAddress, string key, string value)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Gets information about the console.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <returns>A XboxConsoleInfo containing information about the console.</returns>
        protected virtual XboxConsoleInfo GetConsoleInfoImpl(string systemIpAddress)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Throws XdkNotFoundException if XDK is not properly installed.
        /// </summary>
        protected virtual void ThrowIfXdkNotInstalled()
        {
            if (string.IsNullOrWhiteSpace(this.PathToXdk))
            {
                throw new XdkNotFoundException("Failed to find a value for the \"DurangoXdk\" environment variable. Ensure that the XDK is installed.");
            }

            string xdkBinDir = this.PathToXdkBin;
            if (!Directory.Exists(xdkBinDir))
            {
                throw new XdkNotFoundException(string.Format(CultureInfo.InvariantCulture, "Failed to find the expected directory containing XDK binary files. Ensure this directory exists: '{0}'", xdkBinDir));
            }
        }

        private void ThrowIfInvalidSystemIpAddress(string systemIpAddress)
        {
            if (string.IsNullOrWhiteSpace(systemIpAddress))
            {
                throw new ArgumentNullException("systemIpAddress");
            }
        }

        private void ThrowIfInvalidGamepadState(XboxGamepadState report)
        {
            if (report == null)
            {
                throw new ArgumentNullException("report");
            }
        }

        private void PerformXdkAction(string systemIpAddress, Action action, string errorMessage)
        {
            try
            {
                action();
            }
            catch (COMException ex)
            {
                throw ExceptionFactory.Create(errorMessage, ex, systemIpAddress);
            }
        }

        private T PerformXdkFunc<T>(string systemIpAddress, Func<T> func, string errorMessage)
        {
            try
            {
                return func();
            }
            catch (COMException ex)
            {
                throw ExceptionFactory.Create(errorMessage, ex, systemIpAddress);
            }
        }
    }
}
