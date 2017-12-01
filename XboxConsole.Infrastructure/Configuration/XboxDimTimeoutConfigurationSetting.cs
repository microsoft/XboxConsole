//------------------------------------------------------------------------------
// <copyright file="XboxDimTimeoutConfigurationSetting.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Configuration
{
    using System;
    using System.Globalization;

    /// <summary>
    /// An explicit override of the XboxConfigurationSetting class meant to represent the use of the string
    /// of the DimTimeout configuration setting (see xbconfig command line utility).
    /// </summary>
    internal class XboxDimTimeoutConfigurationSetting : XboxConfigurationSetting<int>
    {
        /// <summary>
        /// Initializes a new instance of the XboxDimTimeoutConfigurationSetting class.
        /// </summary>
        /// <param name="key">The setting key.</param>
        internal XboxDimTimeoutConfigurationSetting(string key)
            : base(key)
        {
        }

        /// <summary>
        /// Converts a strongly-typed value into a string value.
        /// </summary>
        /// <param name="dimTimeout">The value to be converted.</param>
        /// <returns>The string value that corresponds to the specified value.</returns>
        protected override string GetStringValueFromValue(int dimTimeout)
        {
            // Enforce allowed values.
            if (dimTimeout >= 0 && dimTimeout <= 255 && (dimTimeout == 0 || (dimTimeout % 5) == 0))
            {
                return dimTimeout.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                throw new ArgumentException("DimTimeout must be 0, or a value between 1 and 255 in increments of 5", "dimTimeout");
            }
        }
    }
}
