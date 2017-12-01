//------------------------------------------------------------------------------
// <copyright file="XboxHostNameConfigurationSetting.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Configuration
{
    using System;

    /// <summary>
    /// An explicit override of the XboxConfigurationSetting class meant to represent the use of the string
    /// of the HostName configuration setting (see xbconfig command line utility).
    /// </summary>
    internal class XboxHostNameConfigurationSetting : XboxConfigurationSetting<string>
    {
        /// <summary>
        /// Initializes a new instance of the XboxHostNameConfigurationSetting class.
        /// </summary>
        /// <param name="key">The setting key.</param>
        internal XboxHostNameConfigurationSetting(string key)
            : base(key)
        {
        }

        /// <summary>
        /// Converts a strongly-typed value into a string value.
        /// </summary>
        /// <param name="hostName">The value to be converted.</param>
        /// <returns>The string value that corresponds to the specified value.</returns>
        protected override string GetStringValueFromValue(string hostName)
        {
            // Enforce allowed values.
            if (string.IsNullOrEmpty(hostName) || (System.Text.RegularExpressions.Regex.IsMatch(hostName, "^[a-zA-Z0-9]{0,15}$") && !char.IsDigit(hostName[0])))
            {
                return hostName;
            }
            else
            {
                throw new ArgumentException("hostName must be either null or an alphanumeric string of 15 characters or less that doesn't start with a digit.", "hostName");
            }
        }
    }
}
