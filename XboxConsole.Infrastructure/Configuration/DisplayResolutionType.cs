//------------------------------------------------------------------------------
// <copyright file="DisplayResolutionType.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Configuration
{
    /// <summary>
    /// Range of valid values for DisplayResolution configuration setting.
    /// </summary>
    public enum DisplayResolutionType
    {
        /// <summary>
        /// 720p resolution.
        /// </summary>
        HD,

        /// <summary>
        /// 1080p resolution.
        /// </summary>
        FullHD,

        /// <summary>
        /// 640x480 resolution.
        /// </summary>
        Minimal
    }
}
