//------------------------------------------------------------------------------
// <copyright file="ColorDepthType.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Configuration
{
    /// <summary>
    /// Range of valid values for ColorDepthType configuration setting.
    /// </summary>
    public enum ColorDepthType
    {
        /// <summary>
        /// 24 bit color depth.
        /// </summary>
        TwentyFourBit,

        /// <summary>
        /// 30 bit color depth.
        /// </summary>
        ThirtyBit,

        /// <summary>
        /// 36 bit color depth.
        /// </summary>
        ThirtySixBit,
    }
}