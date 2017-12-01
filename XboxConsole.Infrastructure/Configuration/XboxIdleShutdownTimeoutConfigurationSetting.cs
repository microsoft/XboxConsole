//------------------------------------------------------------------------------
// <copyright file="XboxIdleShutdownTimeoutConfigurationSetting.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Configuration
{
    using System;
    using System.Globalization;

    /// <summary>
    /// An explicit override of the XboxConfigurationSetting class meant to represent the use of the string
    /// of the IdleShutdownTimeout configuration setting (see xbconfig command line utility).
    /// </summary>
    internal class XboxIdleShutdownTimeoutConfigurationSetting : XboxConfigurationSetting<IdleShutdownTimeoutType>
    {
        /// <summary>
        /// Initializes a new instance of the XboxIdleShutdownTimeoutConfigurationSetting class.
        /// </summary>
        /// <param name="key">The setting key.</param>
        internal XboxIdleShutdownTimeoutConfigurationSetting(string key)
            : base(key)
        {
        }

        /// <summary>
        /// Converts a strongly-typed value into a string value.
        /// </summary>
        /// <param name="idleShutdownTimeout">The value to be converted.</param>
        /// <returns>The string value that corresponds to the specified value.</returns>
        protected override string GetStringValueFromValue(IdleShutdownTimeoutType idleShutdownTimeout)
        {
            // Enforce allowed values.
            switch (idleShutdownTimeout)
            {
                default:
                case IdleShutdownTimeoutType.NoTimeout:
                    return "0";
                case IdleShutdownTimeoutType.TimeoutOneHour:
                    return "360";
                case IdleShutdownTimeoutType.TimeoutOneMinute:
                    return "60";
            }
        }

        /// <summary>
        /// Converts a string value into a stringly-typed value.
        /// </summary>
        /// <param name="stringVal">The string value to be converted.</param>
        /// <returns>The strongly typed value corresponding to the specified string value.</returns>
        protected override IdleShutdownTimeoutType GetValueFromStringValue(string stringVal)
        {
            if (string.IsNullOrEmpty(stringVal))
            {
                return IdleShutdownTimeoutType.NoTimeout;
            }

            switch (stringVal)
            {
                default:
                case "0":
                    return IdleShutdownTimeoutType.NoTimeout;
                case "60":
                    return IdleShutdownTimeoutType.TimeoutOneMinute;
                case "360":
                    return IdleShutdownTimeoutType.TimeoutOneHour;
            }
        }
    }
}
