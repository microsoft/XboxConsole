//------------------------------------------------------------------------------
// <copyright file="WirelessRadioSettingsType.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Configuration
{
    /// <summary>
    /// Range of valid values for WirelessRadioSettings configuration setting.
    /// </summary>
    public enum WirelessRadioSettingsType
    {
        /// <summary>
        /// Turn all radios on.
        /// </summary>
        On,

        /// <summary>
        /// Turn all radios off.
        /// </summary>
        Off,

        /// <summary>
        /// Turn WLAN radio off.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Lan", Justification = "Lan is spelled correctly.")]
        WLanOff,

        /// <summary>
        /// Turn Accessories radio off.
        /// </summary>
        AccessoriesOff
    }
}
