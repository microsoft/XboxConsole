//------------------------------------------------------------------------------
// <copyright file="XboxDnsServerConfigurationSetting.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Configuration
{
    using System;

    /// <summary>
    /// An explicit override of the XboxConfigurationSetting class meant to represent the use of the string
    /// of the DnsServer configuration setting (see xbconfig command line utility).
    /// </summary>
    internal class XboxDnsServerConfigurationSetting : XboxConfigurationSetting<string>
    {
        /// <summary>
        /// Initializes a new instance of the XboxDnsServerConfigurationSetting class.
        /// </summary>
        /// <param name="key">The setting key.</param>
        internal XboxDnsServerConfigurationSetting(string key)
            : base(key)
        {
        }

        /// <summary>
        /// Converts a strongly-typed value into a string value.
        /// </summary>
        /// <param name="dnsServer">The value to be converted.</param>
        /// <returns>The string value that corresponds to the specified value.</returns>
        protected override string GetStringValueFromValue(string dnsServer)
        {
            if (string.IsNullOrEmpty(dnsServer))
            {
                return string.Empty;
            }

            // Enforce allowed values.
            const string IpAddressRegex = @"([0-9]{1,3}\.){3}[0-9]{1,3}";

            string[] values = dnsServer.Split(',');
            bool firstBlank = string.IsNullOrEmpty(values[0]);            
            bool firstValid = System.Text.RegularExpressions.Regex.IsMatch(values[0], IpAddressRegex);
            bool secondBlank = values.Length < 2 || string.IsNullOrEmpty(values[1]);
            bool secondValid = false;

            if (!secondBlank)
            {
                secondValid = System.Text.RegularExpressions.Regex.IsMatch(values[1], IpAddressRegex);
            }

            if (values.Length <= 2 && values.Length > 0 && (firstBlank || firstValid) && (secondBlank || secondValid))
            {
                return dnsServer;
            }
            else
            {
                throw new ArgumentException("DnsServer must be one or two valid IP addresses separated by a comma.", "dnsServer");
            }
        }
    }
}
