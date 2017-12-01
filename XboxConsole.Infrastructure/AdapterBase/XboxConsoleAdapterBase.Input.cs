//------------------------------------------------------------------------------
// <copyright file="XboxConsoleAdapterBase.Input.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox
{
    using System;
    using Microsoft.Internal.GamesTest.Xbox.Input;

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
        /// Connects the XboxGamepad to the console.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <returns>The id of the XboxGamepad.</returns>
        public ulong ConnectXboxGamepad(string systemIpAddress)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            return this.PerformXdkFunc(
                systemIpAddress,
                () => this.ConnectXboxGamepadImpl(systemIpAddress),
                "Failed to connect a gamepad.");
        }

        /// <summary>
        /// Disconnects an XboxGamepad from the console.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="gamepadId">The Id of XboxGamepad to disconnect.</param>
        public void DisconnectXboxGamepad(string systemIpAddress, ulong gamepadId)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            this.PerformXdkAction(
                systemIpAddress,
                () => this.DisconnectXboxGamepadImpl(systemIpAddress, gamepadId),
                "Failed to disconnect a gamepad.");
        }

        /// <summary>
        /// Sets the state of the XboxGamepad.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="gamepadId">The id of the XboxGamepad to set the state of.</param>
        /// <param name="report">The state to set.</param>
        public void SendGamepadReport(string systemIpAddress, ulong gamepadId, XboxGamepadState report)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);
            this.ThrowIfInvalidGamepadState(report);

            this.PerformXdkAction(
                systemIpAddress,
                () => this.SendXboxGamepadReportImpl(systemIpAddress, gamepadId, report),
                "Failed to send a gamepad report.");
        }

        /// <summary>
        /// Connects the XboxGamepad to the console.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <returns>The id of the XboxGamepad.</returns>
        protected virtual ulong ConnectXboxGamepadImpl(string systemIpAddress)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Disconnects an XboxGamepad from the console.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="gamepadId">The id of the XboxGamepad to disconnect.</param>
        protected virtual void DisconnectXboxGamepadImpl(string systemIpAddress, ulong gamepadId)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Sets the state of the XboxGamepad.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="gamepadId">The Id of the XboxGamepad to set the state of.</param>
        /// <param name="report">The state to set.</param>
        protected virtual void SendXboxGamepadReportImpl(string systemIpAddress, ulong gamepadId, XboxGamepadState report)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }
    }
}
