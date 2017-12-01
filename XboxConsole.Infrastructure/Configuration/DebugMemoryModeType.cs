//------------------------------------------------------------------------------
// <copyright file="DebugMemoryModeType.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Configuration
{
    /// <summary>
    /// Range of valid values for the DebugMemoryMode configuration setting.
    /// </summary>
    public enum DebugMemoryModeType
    {
        /// <summary>
        /// Use Pix debug memory mode.
        /// </summary>
        Pix,

        /// <summary>
        /// Use Pix_Tool debug memory mode.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Matches xbconfig setting name")]
        Pix_Tool,

        /// <summary>
        /// Use Pix_Title debug memory mode.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Matches xbconfig setting name")]
        Pix_Title,

        /// <summary>
        /// Use PGI debug memory mode.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "PGI", Justification = "Matches xbconfig setting name")]
        PGI,

        /// <summary>
        /// Use PGI_Title debug memory mode.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "PGI", Justification = "Matches xbconfig setting name")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Matches xbconfig setting name")]
        PGI_Title,

        /// <summary>
        /// Use PGI_Tool debug memory mode.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "PGI", Justification = "Matches xbconfig setting name")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Matches xbconfig setting name")]
        PGI_Tool
    }
}
