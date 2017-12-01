//------------------------------------------------------------------------------
// <copyright file="UserPairingType.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Configuration
{
    /// <summary>
    /// Range of valid values for UserPairing configuration setting(s).
    /// </summary>
    public enum UserPairingType
    {
        /// <summary>
        /// User will be paired to physical gamepad.
        /// </summary>
        Gamepad,

        /// <summary>
        /// User will be paired to virtual gamepad.
        /// </summary>
        Virtual,

        /// <summary>
        /// User will be paired to first physical controller discovered.
        /// </summary>
        AnyPhysical,

        /// <summary>
        /// User will not be paired.
        /// </summary>
        None
    }
}
