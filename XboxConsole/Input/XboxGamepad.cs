//------------------------------------------------------------------------------
// <copyright file="XboxGamepad.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Input
{
    using System;
    using System.Threading;
    using Microsoft.Internal.GamesTest.Xbox.Telemetry;

    /// <summary>
    /// A virtual gamepad.
    /// </summary>
    public class XboxGamepad : XboxItem
    {
        private ulong? id;

        /// <summary>
        /// Initializes a new instance of the XboxGamepad class.
        /// </summary>
        /// <param name="console">The console the XboxGamepad will connect to.</param>
        internal XboxGamepad(XboxConsole console)
            : base(console)
        {
            this.Id = null;
        }

        /// <summary>
        /// Gets the id of the XboxGamepad.
        /// </summary>
        /// <remarks>A null Id indicates the XboxGamepad is not connected.</remarks>
        public ulong? Id
        {
            get
            {
                return this.id;
            }

            private set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the XboxGamepad is connected.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return this.Id.HasValue;
            }
        }

        /// <summary>
        /// Connects the XboxGamepad to the console.
        /// </summary>
        public void Connect()
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            this.ThrowIfConnected();

            this.Id = this.Console.Adapter.ConnectXboxGamepad(this.Console.SystemIpAddressAndSessionKeyCombined);
        }

        /// <summary>
        /// Disconnects the XboxGamepad from the console.
        /// </summary>
        public void Disconnect()
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            this.ThrowIfNotConnected();

            this.Console.Adapter.DisconnectXboxGamepad(this.Console.SystemIpAddressAndSessionKeyCombined, this.Id.Value);

            this.Id = null;
        }

        /// <summary>
        /// Sets the state of the XboxGamepad.
        /// </summary>
        /// <param name="state">The state to set the XboxGamepad to.</param>
        public void SetXboxGamepadState(XboxGamepadState state)
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            if (state == null)
            {
                throw new ArgumentNullException("state");
            }

            this.ThrowIfNotConnected();

            this.Console.Adapter.SendGamepadReport(this.Console.SystemIpAddressAndSessionKeyCombined, this.Id.Value, state);
        }

        private void ThrowIfConnected()
        {
            if (this.IsConnected)
            {
                throw new XboxInputException("XboxGamepad is already connected.", this.Console.SystemIpAddressString, this.Id.Value);
            }
        }

        private void ThrowIfNotConnected()
        {
            if (!this.IsConnected)
            {
                throw new XboxInputException("XboxGamepad is not connected.", this.Console.SystemIpAddressString, this.Id);
            }
        }
    }
}
