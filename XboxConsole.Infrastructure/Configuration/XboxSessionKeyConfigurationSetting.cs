//------------------------------------------------------------------------------
// <copyright file="XboxSessionKeyConfigurationSetting.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Configuration
{
    using System;

    /// <summary>
    /// An explicit override of the XboxConfigurationSetting class meant to represent the use of the string
    /// of the SessionKey configuration setting (see xbconfig command line utility).
    /// </summary>
    internal class XboxSessionKeyConfigurationSetting : XboxConfigurationSetting<string>
    {
        /// <summary>
        /// Initializes a new instance of the XboxSessionKeyConfigurationSetting class.
        /// </summary>
        /// <param name="key">The setting key.</param>
        internal XboxSessionKeyConfigurationSetting(string key)
            : base(key)
        {
        }

        /// <summary>
        /// Converts a strongly-typed value into a string value.
        /// </summary>
        /// <param name="sessionKey">The value to be converted.</param>
        /// <returns>The string value that corresponds to the specified value.</returns>
        protected override string GetStringValueFromValue(string sessionKey)
        {
            // Enforce allowed values.
            if (string.IsNullOrEmpty(sessionKey) || System.Text.RegularExpressions.Regex.IsMatch(sessionKey, "^[a-zA-Z0-9]{0,31}$"))
            {
                return sessionKey;
            }
            else
            {
                throw new ArgumentException("sessionKey must be either null or an alphanumeric string of 31 characters or less.", "sessionKey");
            }
        }
    }
}
