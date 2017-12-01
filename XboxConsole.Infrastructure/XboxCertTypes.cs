//------------------------------------------------------------------------------
// <copyright file="XboxCertTypes.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides information about a consoles cert type.
    /// </summary>
    [Flags]
    public enum XboxCertTypes
    {
        /// <summary>
        /// No cert type specified.
        /// </summary>
        None = 0x0,

        /// <summary>
        /// ERA development specified on the console.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "DevKit", Justification = "Matches xbconfig setting.")]
        EraDevKit = 0x1,

        /// <summary>
        /// SRA development specified on the console.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "DevKit", Justification = "Matches xbconfig setting.")]
        SraDevKit = 0x2,

        /// <summary>
        /// ERA test specified on the console.
        /// </summary>
        EraTestKit = 0x4,

        /// <summary>
        /// Some other cert is specified.
        /// </summary>
        Other = 0x8,
    }
}
