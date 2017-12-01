//------------------------------------------------------------------------------
// <copyright file="XboxConsoleAdapter.Input.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Adapter.October2014
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Internal.GamesTest.Xbox.Input;

    /// <summary>
    /// This class represents an implemenation of the XboxConsoleAdapter
    /// that is supported by the October 2014 version of the XDK.
    /// </summary>
    internal partial class XboxConsoleAdapter : XboxConsoleAdapterBase
    {
        private List<IVirtualGamepad> gamepads = new List<IVirtualGamepad>();

        /// <summary>
        /// Connects the XboxGamepad to the console.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <returns>The id of the XboxGamepad.</returns>
        protected override ulong ConnectXboxGamepadImpl(string systemIpAddress)
        {
            IVirtualGamepad gamepad = this.XboxXdk.CreateXboxGamepad(systemIpAddress);
            this.gamepads.Add(gamepad);

            return gamepad.Connect();
        }

        /// <summary>
        /// Disconnects an XboxGamepad from the console.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="xboxGamepadId">The Id of the XboxGamepad to disconnect.</param>
        protected override void DisconnectXboxGamepadImpl(string systemIpAddress, ulong xboxGamepadId)
        {
            var gamepad = this.GetXboxGamepad(xboxGamepadId);
            gamepad.Disconnect();
            this.gamepads.Remove(gamepad);
        }

        /// <summary>
        /// Disconnects all XboxGamepads from the console.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        protected override void DisconnectAllXboxGamepadsImpl(string systemIpAddress)
        {
            this.XboxXdk.DisconnectAllXboxGamepads(systemIpAddress);
            this.gamepads.Clear();
        }

        /// <summary>
        /// Sets the state of the XboxGamepad.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="xboxGamepadId">The Id of the XboxGamepad to set the state of.</param>
        /// <param name="report">The state to set.</param>
        protected override void SendXboxGamepadReportImpl(string systemIpAddress, ulong xboxGamepadId, Input.XboxGamepadState report)
        {
            var gamepad = this.GetXboxGamepad(xboxGamepadId);
            gamepad.SetGamepadState(report);
        }

        private IVirtualGamepad GetXboxGamepad(ulong id)
        {
            var gamepad = this.gamepads.FirstOrDefault(g => g.Id == id);

            if (gamepad == null)
            {
                throw new XboxInputException(string.Format(CultureInfo.InvariantCulture, "Did not find a XboxGamepad with Id '{0}' in adapter.", id));
            }

            return gamepad;
        }
    }
}
