//------------------------------------------------------------------------------
// <copyright file="ProfilingModeType.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Configuration
{
    /// <summary>
    /// Range of valid values for ProfilingMode configuration setting.
    /// </summary>
    public enum ProfilingModeType
    {
        /// <summary>
        /// Reserve memory to aid with profiling.
        /// </summary>
        On,

        /// <summary>
        /// Restore retail memory profile.
        /// </summary>
        Off,

        /// <summary>
        /// Legacy: match the "on" behavior of March XDK and previous.
        /// </summary>
        Legacy
    }
}
