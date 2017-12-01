//------------------------------------------------------------------------------
// <copyright file="GamepadExtensions.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Xbox.Input
{
    using Microsoft.Xbox.XTF;

    /// <summary>
    /// This contains extra functionality added to the XtfGamepad class by the GTO CTS team.
    /// </summary>
    public sealed partial class XtfGamepad
    {
        /// <summary>
        /// Gets the Id of the gamepad.
        /// </summary>
        public ulong Id
        {
            get
            {
                return this._id;
            }
        }

        /// <summary>
        /// Sets the state of the gamepad.
        /// </summary>
        /// <param name="report">The state to set the gamepad.</param>
        public void SetGamepadState(GAMEPAD_REPORT report)
        {
            this._currentState = report;
            this.SendReport();
        }
    }
}
