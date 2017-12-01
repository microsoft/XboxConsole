//------------------------------------------------------------------------------
// <copyright file="IdleShutdownTimeoutType.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Configuration
{
    /// <summary>
    /// Range of valid values for IdleShutdownTimeout configuration setting.
    /// </summary>
    public enum IdleShutdownTimeoutType
    {
        /// <summary>
        /// No timeout (value of "0").
        /// </summary>
        NoTimeout,

        /// <summary>
        /// One minute (value of "60").
        /// </summary>
        TimeoutOneMinute,

        /// <summary>
        /// One hour (value of "360").
        /// </summary>
        TimeoutOneHour
    }
}
