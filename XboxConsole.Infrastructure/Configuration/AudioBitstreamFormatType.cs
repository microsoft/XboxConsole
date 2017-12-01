//------------------------------------------------------------------------------
// <copyright file="AudioBitstreamFormatType.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Configuration
{
    /// <summary>
    /// Range of valid values for the AudioBitstreamFormat configuration setting.
    /// </summary>
    public enum AudioBitstreamFormatType
    {
        /// <summary>
        /// Output is in Digital Theater Systems format.
        /// </summary>
        Dts,

        /// <summary>
        /// Output is in Dolby Digital format.
        /// </summary>
        DolbyDigital
    }
}
