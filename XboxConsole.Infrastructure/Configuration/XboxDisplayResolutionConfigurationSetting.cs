//------------------------------------------------------------------------------
// <copyright file="XboxDisplayResolutionConfigurationSetting.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Configuration
{
    using System;
    using System.Globalization;

    /// <summary>
    /// An explicit override of the XboxConfigurationSetting class meant to represent the use of the string
    /// of the DisplayResolution configuration setting (see xbconfig command line utility).
    /// </summary>
    internal class XboxDisplayResolutionConfigurationSetting : XboxConfigurationSetting<DisplayResolutionType>
    {
        /// <summary>
        /// Initializes a new instance of the XboxDisplayResolutionConfigurationSetting class.
        /// </summary>
        /// <param name="key">The setting key.</param>
        internal XboxDisplayResolutionConfigurationSetting(string key)
            : base(key)
        {
        }

        /// <summary>
        /// Converts a strongly-typed value into a string value.
        /// </summary>
        /// <param name="displayResolution">The value to be converted.</param>
        /// <returns>The string value that corresponds to the specified value.</returns>
        protected override string GetStringValueFromValue(DisplayResolutionType displayResolution)
        {
            // Enforce allowed values.
            switch (displayResolution)
            {
                default:
                case DisplayResolutionType.HD:
                    return "720p";
                case DisplayResolutionType.FullHD:
                    return "1080p";
                case DisplayResolutionType.Minimal:
                    return "640x480";
            }
        }

        /// <summary>
        /// Converts a string value into a stringly-typed value.
        /// </summary>
        /// <param name="stringVal">The string value to be converted.</param>
        /// <returns>The strongly typed value corresponding to the specified string value.</returns>
        protected override DisplayResolutionType GetValueFromStringValue(string stringVal)
        {
            if (string.IsNullOrEmpty(stringVal))
            {
                return DisplayResolutionType.HD;
            }

            switch (stringVal.ToUpperInvariant())
            {
                default:
                case "720P":
                    return DisplayResolutionType.HD;
                case "1080P":
                    return DisplayResolutionType.FullHD;
                case "640X480":
                    return DisplayResolutionType.Minimal;
            }
        }
    }
}
