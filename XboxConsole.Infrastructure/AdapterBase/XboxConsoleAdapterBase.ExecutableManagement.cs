//------------------------------------------------------------------------------
// <copyright file="XboxConsoleAdapterBase.ExecutableManagement.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox
{
    using System;

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
        /// Runs an executable on the console.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="fileName">The path to an executable to start.</param>
        /// <param name="arguments">The command-line arguments to pass into the executable.</param>
        /// <param name="operatingSystem">The <see cref="Microsoft.Internal.GamesTest.Xbox.XboxOperatingSystem"/> to run the executable on.</param>
        /// <param name="outputReceivedCallback">A callback method that will be called when there is output from the process.</param>
        public void RunExecutable(string systemIpAddress, string fileName, string arguments, XboxOperatingSystem operatingSystem, Action<string> outputReceivedCallback)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            this.PerformXdkAction(
                systemIpAddress,
                () => this.RunExecutableImpl(systemIpAddress, fileName, arguments, operatingSystem, outputReceivedCallback),
                "Failed to run executable.");
        }

        /// <summary>
        /// Provides the adapter-specific implementation of the "RunExecutable" method.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="fileName">The path to an executable to start.</param>
        /// <param name="arguments">The command-line arguments to pass into the executable.</param>
        /// <param name="operatingSystem">The <see cref="Microsoft.Internal.GamesTest.Xbox.XboxOperatingSystem"/> to run the executable on.</param>
        /// <param name="outputReceivedCallback">A callback method that will be called when there is output from the process.</param>
        protected virtual void RunExecutableImpl(string systemIpAddress, string fileName, string arguments, XboxOperatingSystem operatingSystem, Action<string> outputReceivedCallback)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }
    }
}
