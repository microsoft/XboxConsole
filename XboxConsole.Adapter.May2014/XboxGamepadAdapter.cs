//------------------------------------------------------------------------------
// <copyright file="XboxGamepadAdapter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Adapter.May2014
{
    using System;
    using System.Globalization;
    using Microsoft.Internal.GamesTest.Xbox.Input;
    using Microsoft.Xbox.Input;
    using Microsoft.Xbox.XTF;
    using Microsoft.Xbox.XTF.Input;

    /// <summary>
    /// Adapter specific logic for the XboxGamepad class.
    /// </summary>
    internal class XboxGamepadAdapter : IVirtualGamepad
    {
        private const string ConnectErrorMessage = "Could not connect gamepad.{0}**Hint** Max allowed connections is 16, controllers may still be connected if previous instances have crashed or debugging stopped. This may require a reboot of the kit.";
        private const string UseErrorMessage = "Could not access the gamepad because it was disconnected.";

        // Constant used by Xtf to map a trigger value in the range 0.0 to 1.0 to the range 0.0 to 255.0. (Equivalent to 1.0 / 255.0.)
        private const float TriggerConversion = 0.003922f;

        private XtfGamepad xtfGamepad;
        private string originalIpAddress;

        /// <summary>
        /// Initializes a new instance of the XboxGamepadAdapter class.
        /// </summary>
        /// <param name="systemIpAddress">Original IP address the XTF gamepad was connected to (used for error reporting).</param>
        /// <param name="xtfGamepad">An XtfGamepad.</param>
        public XboxGamepadAdapter(string systemIpAddress, XtfGamepad xtfGamepad)
        {
            this.originalIpAddress = systemIpAddress;
            this.xtfGamepad = xtfGamepad;
        }

        /// <summary>
        /// Gets the id of the connected gamepad.
        /// </summary>
        public ulong Id
        {
            get
            {
                return this.xtfGamepad.Id;
            }
        }

        /// <summary>
        /// Connects an XtfGamepad.
        /// </summary>
        /// <returns>The Id of the gamepad.</returns>
        public ulong Connect()
        {
            try
            {
                this.xtfGamepad.Connect();
                return this.xtfGamepad.Id;
            }
            catch (XtfInputNoConnectionException ex)
            {
                throw new XboxInputException(
                    string.Format(CultureInfo.InvariantCulture, ConnectErrorMessage, Environment.NewLine),
                    ex,
                    this.originalIpAddress,
                    null);
            }
            catch (XtfInputException ex)
            {
                throw new XboxInputException(
                    string.Format(CultureInfo.InvariantCulture, ConnectErrorMessage, Environment.NewLine),
                    ex,
                    this.originalIpAddress,
                    null);
            }
        }

        /// <summary>
        /// Disconnects an XtfGamepad.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                this.xtfGamepad.Disconnect();
            }
            catch (XtfInputNoConnectionException ex)
            {
                throw new XboxInputException(
                    string.Format(CultureInfo.InvariantCulture, UseErrorMessage),
                    ex,
                    this.originalIpAddress,
                    null);
            }
        }

        /// <summary>
        /// Sets the gamepad state.
        /// </summary>
        /// <param name="state">The new state for the gamepad.</param>
        public void SetGamepadState(XboxGamepadState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException("state");
            }

            GAMEPAD_REPORT report;
            report.Buttons = (ushort)state.Buttons;
            report.LeftThumbstickX = this.ConvertGamepadStateThumbstickValue(state.LeftThumbstickX);
            report.LeftThumbstickY = this.ConvertGamepadStateThumbstickValue(state.LeftThumbstickY);
            report.RightThumbstickX = this.ConvertGamepadStateThumbstickValue(state.RightThumbstickX);
            report.RightThumbstickY = this.ConvertGamepadStateThumbstickValue(state.RightThumbstickY);
            report.LeftTrigger = this.ConvertGamepadStateTriggerValue(state.LeftTrigger);
            report.RightTrigger = this.ConvertGamepadStateTriggerValue(state.RightTrigger);

            try
            {
                this.xtfGamepad.SetGamepadState(report);
            }
            catch (XtfInputNoConnectionException ex)
            {
                throw new XboxInputException(
                    string.Format(CultureInfo.InvariantCulture, UseErrorMessage),
                    ex,
                    this.originalIpAddress,
                    null);
            }
        }

        /// <summary>
        /// Converts a trigger value from a GamepadState format to a GAMEPAD_REPORT format.
        /// </summary>
        /// <param name="val">Trigger value as a float.</param>
        /// <returns>Trigger value as a ushort.</returns>
        /// <remarks>
        /// This conversion is the same one used by Xtf to map a floating point value in the range 0.0 to 1.0
        /// to an integral value in the range 0 to 1023.
        /// </remarks>
        private ushort ConvertGamepadStateTriggerValue(float val)
        {
            return (ushort)((short)(val / TriggerConversion) * 4);
        }

        /// <summary>
        /// Converts a thumbstick value from a GamepadState format to a GAMEPAD_REPORT format.
        /// </summary>
        /// <param name="val">Thumbstick value as a float.</param>
        /// <returns>Thumbstick value as a ushort.</returns>
        /// <remarks>
        /// This conversion is the same one used by Xtf to map a floating point value in the range -1.0 to 1.0
        /// to an integral value in the range -32768 to 32767.
        /// </remarks>
        private short ConvertGamepadStateThumbstickValue(float val)
        {
            return (short)(val * short.MaxValue);
        }
    }
}
