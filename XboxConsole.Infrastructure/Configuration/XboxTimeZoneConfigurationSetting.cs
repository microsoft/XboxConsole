//------------------------------------------------------------------------------
// <copyright file="XboxTimeZoneConfigurationSetting.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// An explicit override of the XboxConfigurationSetting class meant to represent the TimeZone
    /// configuration setting (see xbconfig command line utility).
    /// </summary>
    internal class XboxTimeZoneConfigurationSetting : XboxConfigurationSetting<TimeZoneInfo>
    {
        /// <summary>
        /// Initializes a new instance of the XboxTimeZoneConfigurationSetting class.
        /// </summary>
        /// <param name="key">The setting key.</param>
        internal XboxTimeZoneConfigurationSetting(string key)
            : base(key)
        {
        }

        /// <summary>
        /// Converts a TimeZoneInfo class into a string as expected by the TimeZone
        /// configuration setting (e.g. "en-US;en-GB;en-AU").
        /// </summary>
        /// <param name="timeZone">The TimeZoneInfo class to be parsed.</param>
        /// <returns>The string value that corresponds to the specified TimeZoneInfo class.</returns>
        protected override string GetStringValueFromValue(TimeZoneInfo timeZone)
        {
            if (timeZone == null)
            {
                return null;
            }

            try
            {
                // Calling FindSystemTimeZoneById will tell us if this is a valid time zone for this computer.
                TimeZoneInfo.FindSystemTimeZoneById(timeZone.Id);
            }
            catch (TimeZoneNotFoundException e)
            {
                throw new ArgumentException("The supplied time zone string does not correspond to a .NET TimeZoneInfo class.", "timeZone", e);
            }

            // TimeZoneInfo.ToString() includes hour deviation, which is not a component of the expected string
            return timeZone.Id;
        }

        /// <summary>
        /// Converts a string value into a TimeZoneInfo class.
        /// </summary>
        /// <param name="stringVal">The string value.</param>
        /// <returns>The TimeZoneInfo class that corresponds to the specified string value.</returns>
        protected override TimeZoneInfo GetValueFromStringValue(string stringVal)
        {
            if (stringVal == null)
            {
                return null;
            }

            TimeZoneInfo timeZone;
            try
            {
                timeZone = TimeZoneInfo.FindSystemTimeZoneById(stringVal);
            }
            catch (TimeZoneNotFoundException e)
            {
                throw new ArgumentException("The supplied time zone string does not correspond to a .NET TimeZoneInfo class.", "stringVal", e);
            }

            return timeZone;
        }
    }
}
