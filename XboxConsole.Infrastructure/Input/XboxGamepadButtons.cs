//------------------------------------------------------------------------------
// <copyright file="XboxGamepadButtons.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Input
{
    using System;

    /// <summary>
    /// Represents the buttons on an XboxGamepad.
    /// </summary>
    [Flags]
    public enum XboxGamepadButtons
    {
        /// <summary>
        /// Right thumbstick. 
        /// </summary>
        RightThumbstick = 0x8000,

        /// <summary>
        /// Left thumbstick.
        /// </summary>
        LeftThumbstick = 0x4000,

        /// <summary>
        /// Right shoulder button.
        /// </summary>
        RightShoulder = 0x2000,

        /// <summary>
        /// Left shoulder button.
        /// </summary>
        LeftShoulder = 0x1000,

        /// <summary>
        /// Right dpad button.
        /// </summary>
        DpadRight = 0x800,

        /// <summary>
        /// Left dpad button.
        /// </summary>
        DpadLeft = 0x400,

        /// <summary>
        /// Down dpad button.
        /// </summary>
        DpadDown = 0x200,

        /// <summary>
        /// Up dpad button.
        /// </summary>
        DpadUp = 0x100,

        /// <summary>
        /// Y dpad button.
        /// </summary>
        Y = 0x80,

        /// <summary>
        /// X dpad button.
        /// </summary>
        X = 0x40,

        /// <summary>
        /// B dpad button.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "B", Justification = "Intended identifier")]
        B = 0x20,

        /// <summary>
        /// A dpad button.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "A", Justification = "Intended identifier")]
        A = 0x10,

        /// <summary>
        /// View button.
        /// </summary>
        View = 0x8,

        /// <summary>
        /// Menu button.
        /// </summary>
        Menu = 0x4,

        /// <summary>
        /// Nexus button (Xbox button).
        /// </summary>
        Nexus = 0x2,

        /// <summary>
        /// Enroll button (sync button).
        /// </summary>
        Enroll = 0x1,
    }
}
