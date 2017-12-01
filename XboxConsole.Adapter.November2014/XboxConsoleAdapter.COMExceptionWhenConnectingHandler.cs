//------------------------------------------------------------------------------
// <copyright file="XboxConsoleAdapter.COMExceptionWhenConnectingHandler.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Adapter.November2014
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using Microsoft.Internal.GamesTest.Xbox.Deployment;

    /// <summary>
    /// This class represents an implemenation of the XboxConsoleAdapter
    /// that is supported by the November 2014 version of the XDK.
    /// </summary>
    internal partial class XboxConsoleAdapter : XboxConsoleAdapterBase
    {
        /// <summary>
        /// A helper class designed to execute a function within the Adapter and check for a specific sequence of COMExceptions
        /// that indicates that the console against which the user is making requests is currently rebooting. These COMExceptions
        /// are transformed into CannotConnectExceptions for easier consumption.
        /// </summary>
        private class COMExceptionWhenConnectingHandler
        {
            // The COMException with HResult -1945042173 is ALWAYS the first COMException thrown when trying to poll the
            // execution state of a package while the console is rebooting.
            private const int COMExceptionHResult = -1945042173;

            // There are a handful of COMExceptions that are thrown when trying to poll the package state after the console has
            // been rebooted, all with different HResults, but they all share one commonality: The message contains
            // the text "Class not registered".
            private const string COMExceptionMessage = "Class not registered";

            private bool rebootingBasedOnCOMExceptionStep1 = false;
            private bool rebootingBasedOnCOMExceptionStep2 = false;

            /// <summary>
            /// Execute a function that returns an HResult and checks for a specific sequence of COMExceptions
            /// that indicates that the console against which the user is making requests is currently rebooting. These COMExceptions
            /// are transformed into CannotConnectExceptions for easier consumption.
            /// </summary>
            /// <param name="func">A function that makes a request of a console and returns an HResult.</param>
            /// <param name="systemIpAddress">The ip address of the console being used.</param>
            /// <returns>The HResult of the executed function.</returns>
            public uint CheckForCOMExceptionWhenConnecting(Func<uint> func, string systemIpAddress)
            {
                uint funcReturn;
                try
                {
                    funcReturn = func();

                    // if we've made it this far after both stages of rebooting, we know that we've successfully completed rebooting
                    if (this.rebootingBasedOnCOMExceptionStep1 && this.rebootingBasedOnCOMExceptionStep2)
                    {
                        this.rebootingBasedOnCOMExceptionStep1 = false;
                        this.rebootingBasedOnCOMExceptionStep2 = false;
                    }
                }
                catch (COMException ex)
                {
                    // The inability to connect to a rebooting console will manifest as several COMExceptions.
                    // At first a COMException with HResult -1945042173 is thrown, followed by one of several
                    // COMExceptions with varying HResults, all containing the message: "Class not registered".

                    if (ex.HResult == COMExceptionHResult)
                    {
                        // if we've encountered the first COMException, we know that we're rebooting
                        if (!this.rebootingBasedOnCOMExceptionStep1)
                        {
                            this.rebootingBasedOnCOMExceptionStep1 = true;
                        }

                        this.ThrowCannotConnectException(systemIpAddress, ex);
                    }
                    else if (ex.Message.Contains(COMExceptionMessage) && this.rebootingBasedOnCOMExceptionStep1)
                    {
                        // if we've encountered the second COMException AFTER the first, we know that we're rebooting
                        if (!this.rebootingBasedOnCOMExceptionStep2)
                        {
                            this.rebootingBasedOnCOMExceptionStep2 = true;
                        }

                        this.ThrowCannotConnectException(systemIpAddress, ex);
                    }

                    // if none of this is true, something bad has happened and we need to keep throwing the actual COMException
                    throw;
                }

                return funcReturn;
            }

            /// <summary>
            /// Encapsulates a COMException within a CannotConnectException and throws the latter.
            /// </summary>
            /// <param name="systemIpAddress">The ip address of the unresponsive console.</param>
            /// <param name="ex">The COMException to be encapsulated.</param>
            private void ThrowCannotConnectException(string systemIpAddress, COMException ex)
            {
                // make sure to throw a CannotConnectException instead of the COMException
                throw new CannotConnectException(string.Format(CultureInfo.CurrentCulture, "Cannot connect to console with IP {0}; console is likely rebooting.", systemIpAddress), ex);
            }
        }
    }
}
