//------------------------------------------------------------------------------
// <copyright file="XboxNonEmptyStringConfigurationSetting.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// An explicit override of the XboxConfigurationSetting class meant to represent configuration settings
    /// that, although strings, cannot be empty.
    /// </summary>
    internal class XboxNonEmptyStringConfigurationSetting : XboxConfigurationSetting<string>
    {
        /// <summary>
        /// Initializes a new instance of the XboxNonEmptyStringConfigurationSetting class.
        /// </summary>
        /// <param name="key">The setting key.</param>
        internal XboxNonEmptyStringConfigurationSetting(string key)
            : base(key)
        {
        }

        /// <summary>
        /// Ensures that the provided string is not empty.
        /// </summary>
        /// <param name="value">The string to be checked.</param>
        /// <returns>The string value as it was passed in, provided that it was not empty.</returns>
        protected override string GetStringValueFromValue(string value)
        {
            this.EnsureStringIsNonEmpty(value);
            return value;
        }

        /// <summary>
        /// Ensures that the provided string is not empty.
        /// </summary>
        /// <param name="stringVal">The string to be checked.</param>
        /// <returns>The string value as it was passed in, provided that it was not empty.</returns>
        protected override string GetValueFromStringValue(string stringVal)
        {
            this.EnsureStringIsNonEmpty(stringVal);
            return stringVal;
        }

        /// <summary>
        /// Throws an ArgumentException if the provided string is empty.
        /// </summary>
        /// <param name="value">The string to check.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1820:TestForEmptyStringsUsingStringLength", Justification = "Check for null happens in AdapterBase.SetConfigValue")]
        private void EnsureStringIsNonEmpty(string value)
        {
            if (value != null && value.Trim() == string.Empty)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The {0} configuration setting does not support empty strings.", this.Key), "value");
            }
        }
    }
}
