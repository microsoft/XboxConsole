//------------------------------------------------------------------------------
// <copyright file="XboxConsoleAdapter.ExecutableManagement.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Adapter.August2014
{
    using System;

    /// <summary>
    /// This class represents an implemenation of the XboxConsoleAdapter
    /// that is supported by the August 2014 version of the XDK.
    /// </summary>
    internal partial class XboxConsoleAdapter : XboxConsoleAdapterBase
    {
        /// <summary>
        /// Runs an executable on the console.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="fileName">The path to an executable to start.</param>
        /// <param name="arguments">The command-line arguments to pass into the executable.</param>
        /// <param name="operatingSystem">The <see cref="Microsoft.Internal.GamesTest.Xbox.XboxOperatingSystem"/> to run the executable on.</param>
        /// <param name="outputRecievedCallback">A callback method that will be called when there is output from the process.</param>
        protected override void RunExecutableImpl(string systemIpAddress, string fileName, string arguments, XboxOperatingSystem operatingSystem, Action<string> outputRecievedCallback)
        {
            this.XboxXdk.RunExecutable(systemIpAddress, fileName, arguments, operatingSystem, outputRecievedCallback);
        }
    }
}
