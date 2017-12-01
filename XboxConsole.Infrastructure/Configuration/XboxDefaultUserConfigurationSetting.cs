//------------------------------------------------------------------------------
// <copyright file="XboxDefaultUserConfigurationSetting.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Configuration
{
    using System;

    /// <summary>
    /// An explicit override of the XboxConfigurationSetting class meant to represent the use of the string
    /// of the DefaultUser configuration setting (see xbconfig command line utility).
    /// </summary>
    internal class XboxDefaultUserConfigurationSetting : XboxConfigurationSetting<string>
    {
        /// <summary>
        /// Initializes a new instance of the XboxDefaultUserConfigurationSetting class.
        /// </summary>
        /// <param name="key">The setting key.</param>
        internal XboxDefaultUserConfigurationSetting(string key)
            : base(key)
        {
        }

        /// <summary>
        /// Converts a strongly-typed value into a string value.
        /// </summary>
        /// <param name="defaultUser">The value to be converted.</param>
        /// <returns>The string value that corresponds to the specified value.</returns>
        protected override string GetStringValueFromValue(string defaultUser)
        {
            // Enforce allowed values.
            if (string.IsNullOrEmpty(defaultUser) || System.Text.RegularExpressions.Regex.IsMatch(defaultUser, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
            {
                return defaultUser;
            }
            else
            {
                throw new ArgumentException("DefaultUser must be a valid email addres in the form myemailaddress@xboxtest.com", "defaultUser");
            }
        }
    }
}
