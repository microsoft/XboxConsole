//------------------------------------------------------------------------------
// <copyright file="ColorSpaceType.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Configuration
{
    /// <summary>
    /// Range of valid values for ColorSpace configuration setting.
    /// </summary>
    public enum ColorSpaceType
    {
        /// <summary>
        /// Limited color space.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Rgb", Justification = "Rgb is spelled correctly.")]
        RgbLimited,

        /// <summary>
        /// Full color space.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Rgb", Justification = "Rgb is spelled correctly.")]
        RgbFull
    }
}
