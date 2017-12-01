//------------------------------------------------------------------------------
// <copyright file="XboxMacAddressConfigurationSetting.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Configuration
{
    using System;

    /// <summary>
    /// An explicit override of the XboxConfigurationSetting class meant to represent the use of the string
    /// of the MACAddress configuration setting (see xbconfig command line utility).
    /// </summary>
    internal class XboxMacAddressConfigurationSetting : XboxConfigurationSetting<string>
    {
        /// <summary>
        /// Initializes a new instance of the XboxMacAddressConfigurationSetting class.
        /// </summary>
        /// <param name="key">The setting key.</param>
        internal XboxMacAddressConfigurationSetting(string key)
            : base(key)
        {
        }

        /// <summary>
        /// Converts a strongly-typed value into a string value.
        /// </summary>
        /// <param name="macAddress">The value to be converted.</param>
        /// <returns>The string value that corresponds to the specified value.</returns>
        protected override string GetStringValueFromValue(string macAddress)
        {
            // Enforce allowed values.
            if (string.IsNullOrEmpty(macAddress) || System.Text.RegularExpressions.Regex.IsMatch(macAddress, @"^([0-9A-F]{2}[:-]){5}([0-9A-F]{2})$"))
            {
                return macAddress;
            }
            else
            {
                throw new ArgumentException("MACAddress is expected to be formatted as six 2-digit hex components separated by dashes.", "macAddress");
            }
        }
    }
}
