//------------------------------------------------------------------------------
// <copyright file="XboxProfilingModeConfigurationSetting.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Configuration
{
    using System;
    using System.Globalization;

    /// <summary>
    /// An explicit override of the XboxConfigurationSetting class meant to represent the use of the strings
    /// "On", "Off", and "Legacy" to set the ProfilingMode configuration setting (see xbconfig command line utility).
    /// </summary>
    internal class XboxProfilingModeConfigurationSetting : XboxConfigurationSetting<ProfilingModeType>
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
        /// Converts the enumeration value to either "On", "Off", or "Legacy".
        /// </summary>
        /// <param name="profilingMode">The enumeration value to be parsed.</param>
        /// <returns>The string value that corresponds to the specified boolean value.</returns>
        protected override string GetStringValueFromValue(ProfilingModeType profilingMode)
        {
            return profilingMode.ToString();
        }

        /// <summary>
        /// Converts a string value into the ProfilingModeType enumeration.
        /// </summary>
        /// <param name="stringVal">The string value.</param>
        /// <returns>The enumeration constant that corresponds to the passed string value.</returns>
        protected override ProfilingModeType GetValueFromStringValue(string stringVal)
        {
            if (stringVal == null)
            {
                return ProfilingModeType.Off;
            }

            switch (stringVal.ToUpper(CultureInfo.CurrentCulture))
            {
                case "ON":
                    return ProfilingModeType.On;
                case "OFF":
                    return ProfilingModeType.Off;
                case "LEGACY":
                    return ProfilingModeType.Legacy;
            }

            throw new ArgumentException("The provided string cannot be converted into a ProfilingModeType value.", "stringVal");
        }
    }
}
