//------------------------------------------------------------------------------
// <copyright file="XboxGamepadReading.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Input
{
    /// <summary>
    /// Stores the data returned by the GetGamepadState RPC call.
    /// </summary>
    internal class XboxGamepadReading
    {
        /// <summary>
        /// Gets or sets the XboxGamepad index.
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the unique ID used by Xbox.
        /// </summary>
        public long InternalId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the XboxGamepadState.
        /// </summary>
        public XboxGamepadState State
        {
            get;
            set;
        }

        /// <summary>
        /// Returns whether the given buttons are pressed.
        /// </summary>
        /// <param name="buttons">The buttons to check.</param>
        /// <returns>True if all the given buttons are pressed.</returns>
        public bool AreButtonsPressed(XboxGamepadButtons buttons)
        {
            return (this.State.Buttons & buttons) == buttons;
        }
    }
}
