//------------------------------------------------------------------------------
// <copyright file="XboxDevkitAllowAcgConfigurationSetting.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Configuration
{
    using System;

    /// <summary>
    /// An explicit override of the XboxConfigurationSetting class meant to represent the use of the string
    /// of the HdmiAudio configuration setting (see xbconfig command line utility).
    /// </summary>
    internal class XboxDevkitAllowAcgConfigurationSetting : XboxConfigurationSetting<bool?>
    {
        /// <summary>
        /// Initializes a new instance of the XboxDevkitAllowAcgConfigurationSetting class.
        /// </summary>
        /// <param name="key">The setting key.</param>
        internal XboxDevkitAllowAcgConfigurationSetting(string key)
            : base(key)
        {
        }

        /// <summary>
        /// Converts a strongly-typed value into a string value.
        /// </summary>
        /// <param name="devkitAllowAcgSetting">The value to be converted.</param>
        /// <returns>The string value that corresponds to the specified value.</returns>
        protected override string GetStringValueFromValue(bool? devkitAllowAcgSetting)
        {
            return devkitAllowAcgSetting.HasValue ? (devkitAllowAcgSetting.Value ? "On" : "Off") : "Off";
        }

        /// <summary>
        /// Converts a string value into a stringly-typed value.
        /// </summary>
        /// <param name="stringVal">The string value to be converted.</param>
        /// <returns>The strongly typed value corresponding to the specified string value.</returns>
        protected override bool? GetValueFromStringValue(string stringVal)
        {
            if (string.IsNullOrEmpty(stringVal))
            {
                return false;
            }

            switch (stringVal.ToUpperInvariant())
            {
                default:
                case "OFF":
                    return false;
                case "ON":
                    return true;
            }
        }
    }
}
