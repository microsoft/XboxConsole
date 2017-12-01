//------------------------------------------------------------------------------
// <copyright file="IVirtualGamepad.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Input
{
    /// <summary>
    /// Represents an interface for communicating with XTF Virtual gamepads.
    /// </summary>
    internal interface IVirtualGamepad
    {
        /// <summary>
        /// Gets the id of the controller.
        /// </summary>
        ulong Id { get; }

        /// <summary>
        /// Connects a gamepad.
        /// </summary>
        /// <returns>The id of the gamepad after connecting.</returns>
        ulong Connect();

        /// <summary>
        /// Disconnects a gamepad.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Sets the state of a gamepad.
        /// </summary>
        /// <param name="state">The state to set the gamepad to.</param>
        void SetGamepadState(XboxGamepadState state);
    }
}
