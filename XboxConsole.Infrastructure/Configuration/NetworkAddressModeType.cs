//------------------------------------------------------------------------------
// <copyright file="NetworkAddressModeType.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Configuration
{
    /// <summary>
    /// Range of valid values for NetworkAddressMode configuration setting.
    /// </summary>
    public enum NetworkAddressModeType
    {
        /// <summary>
        /// Automatic address mode, use DHCP.
        /// </summary>
        Automatic,

        /// <summary>
        /// Manual address mode, use static IPs.
        /// </summary>
        Manual
    }
}
