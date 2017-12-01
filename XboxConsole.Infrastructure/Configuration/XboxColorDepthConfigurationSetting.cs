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
    internal class XboxColorDepthConfigurationSetting : XboxConfigurationSetting<int>
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
        /// <param name="colorDepth">The value to be converted.</param>
        /// <returns>The string value that corresponds to the specified value.</returns>
        protected override string GetStringValueFromValue(int colorDepth)
        {
            if (colorDepth == 0)
            {
                return null;
            }

            // Enforce allowed values.
            if (colorDepth == 24 || colorDepth == 30 || colorDepth == 36)
            {
                return colorDepth.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "ColorDepth must be 24, 30, or 36. '{0}' passed.", colorDepth.ToString(CultureInfo.InvariantCulture)), "colorDepth");
            }
        }
    }
}
