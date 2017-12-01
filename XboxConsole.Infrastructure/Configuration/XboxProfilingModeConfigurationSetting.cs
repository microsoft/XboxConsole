//------------------------------------------------------------------------------
// <copyright file="XboxProfilingModeConfigurationSetting.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Configuration
{
    using System;

    /// <summary>
    /// An explicit override of the XboxConfigurationSetting class meant to represent the use of the strings
    /// "On" and "Off" to toggle the ProfilingMode configuration setting (see xbconfig command line utility).
    /// </summary>
    internal class XboxProfilingModeConfigurationSetting : XboxConfigurationSetting<bool?>
    {
        /// <summary>
        /// Initializes a new instance of the XboxProfilingModeConfigurationSetting class.
        /// </summary>
        /// <param name="key">The setting key.</param>
        internal XboxProfilingModeConfigurationSetting(string key)
            : base(key)
        {
        }

        /// <summary>
        /// Converts a boolean value to either "On" or "Off".
        /// </summary>
        /// <param name="profilingModeOn">The boolean value to be parsed.</param>
        /// <returns>The string value that corresponds to the specified boolean value.</returns>
        protected override string GetStringValueFromValue(bool? profilingModeOn)
        {
            if (!profilingModeOn.HasValue)
            {
                return null;
            }

            return profilingModeOn.Value ? "On" : "Off";
        }

        /// <summary>
        /// Converts a string value into a boolean.
        /// </summary>
        /// <param name="stringVal">The string value.</param>
        /// <returns>The list of CultureInfo classes that corresponds to the specified string value.</returns>
        protected override bool? GetValueFromStringValue(string stringVal)
        {
            if (stringVal == null)
            {
                return null;
            }

            // just in case, we'll accept "on"/"off", "true"/"false", or integer representations of boolean values
            if (stringVal.Equals("1", StringComparison.OrdinalIgnoreCase) ||
                stringVal.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                stringVal.Equals("on", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (stringVal.Equals("0", StringComparison.OrdinalIgnoreCase) ||
                stringVal.Equals("false", StringComparison.OrdinalIgnoreCase) ||
                stringVal.Equals("off", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            throw new ArgumentException("The provided string cannot be converted into a boolean value.", "stringVal");
        }
    }
}
