//------------------------------------------------------------------------------
// <copyright file="XboxHttpProxyHostConfigurationSetting.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Configuration
{
    using System;

    /// <summary>
    /// An explicit override of the XboxConfigurationSetting class meant to represent the use of the string
    /// of the HttpProxyHost configuration setting (see xbconfig command line utility).
    /// </summary>
    internal class XboxHttpProxyHostConfigurationSetting : XboxConfigurationSetting<string>
    {
        /// <summary>
        /// Initializes a new instance of the XboxHttpProxyHostConfigurationSetting class.
        /// </summary>
        /// <param name="key">The setting key.</param>
        internal XboxHttpProxyHostConfigurationSetting(string key)
            : base(key)
        {
        }

        /// <summary>
        /// Converts a strongly-typed value into a string value.
        /// </summary>
        /// <param name="httpProxyHost">The value to be converted.</param>
        /// <returns>The string value that corresponds to the specified value.</returns>
        protected override string GetStringValueFromValue(string httpProxyHost)
        {
            // Enforce allowed values.
            if (string.IsNullOrEmpty(httpProxyHost) || System.Text.RegularExpressions.Regex.IsMatch(httpProxyHost, @"^(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}):(\d{1,5})$"))
            {
                return httpProxyHost;
            }
            else
            {
                throw new ArgumentException("HttpProxyHost must be a valid IP address with port number separated by colon.", "httpProxyHost");
            }
        }
    }
}
