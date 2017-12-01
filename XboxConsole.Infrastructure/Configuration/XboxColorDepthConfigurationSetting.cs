//------------------------------------------------------------------------------
// <copyright file="XboxColorDepthConfigurationSetting.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Configuration
{
    using System;
    using System.Globalization;

    /// <summary>
    /// An explicit override of the XboxConfigurationSetting class meant to represent the use of the string
    /// of the ColorDepth configuration setting (see xbconfig command line utility).
    /// </summary>
    internal class XboxColorDepthConfigurationSetting : XboxConfigurationSetting<ColorDepthType>
    {
        /// <summary>
        /// Initializes a new instance of the XboxColorDepthConfigurationSetting class.
        /// </summary>
        /// <param name="key">The setting key.</param>
        internal XboxColorDepthConfigurationSetting(string key)
            : base(key)
        {
        }

        /// <summary>
        /// Converts a strongly-typed value into a string value.
        /// </summary>
        /// <param name="displayResolution">The value to be converted.</param>
        /// <returns>The string value that corresponds to the specified value.</returns>
        protected override string GetStringValueFromValue(ColorDepthType displayResolution)
        {
            // Enforce allowed values.
            switch (displayResolution)
            {
                default:
                case ColorDepthType.TwentyFourBit:
                    return "24";
                case ColorDepthType.ThirtyBit:
                    return "30";
                case ColorDepthType.ThirtySixBit:
                    return "36";
            }
        }

        /// <summary>
        /// Converts a string value into a stringly-typed value.
        /// </summary>
        /// <param name="stringVal">The string value to be converted.</param>
        /// <returns>The strongly typed value corresponding to the specified string value.</returns>
        protected override ColorDepthType GetValueFromStringValue(string stringVal)
        {
            if (string.IsNullOrEmpty(stringVal))
            {
                return ColorDepthType.TwentyFourBit;
            }

            switch (stringVal.ToUpperInvariant())
            {
                default:
                case "24":
                    return ColorDepthType.TwentyFourBit;
                case "30":
                    return ColorDepthType.ThirtyBit;
                case "36":
                    return ColorDepthType.ThirtySixBit;
            }
        }
    }
}
