//------------------------------------------------------------------------------
// <copyright file="XboxConfigurationSetting.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Configuration
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    /// <summary>
    /// Represents a setting in Xbox configuration (see xbconfig command line utility).
    /// </summary>
    /// <typeparam name="T">The setting type.</typeparam>
    internal class XboxConfigurationSetting<T>
    {
        private string stringValue;
        private T stronglyTypedValue;

        /// <summary>
        /// Initializes a new instance of the XboxConfigurationSetting class.
        /// </summary>
        /// <param name="key">The setting key.</param>
        internal XboxConfigurationSetting(string key)
        {
            this.Key = key;
        }

        /// <summary>
        /// Gets the setting key.
        /// </summary>
        internal string Key { get; private set; }

        /// <summary>
        /// Gets or sets the setting string value.
        /// </summary>
        internal string StringValue
        {
            get
            {
                return this.stringValue;
            }

            set
            {
                this.stringValue = value;
                this.stronglyTypedValue = this.GetValueFromStringValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the setting strongly-typed value.
        /// </summary>
        internal T Value
        {
            get
            {
                return this.stronglyTypedValue;
            }

            set
            {
                this.stronglyTypedValue = value;
                this.stringValue = this.GetStringValueFromValue(value);
            }
        }

        /// <summary>
        /// Compares the XboxConfigurationSetting with a string, which is assumed
        /// to be the output from xbConfig for the associated configuration property.
        /// </summary>
        /// <param name="stringVal">The string to be compared against for equality.</param>
        /// <remarks>This function is used for testing purposes only.</remarks>
        /// <returns>A boolean value indicating equality.</returns>
        internal virtual bool EqualsConfigString(string stringVal)
        {
            return this.Value.Equals(this.GetValueFromStringValue(stringVal));
        }

        /// <summary>
        /// Converts a strongly-typed value into a string value.
        /// </summary>
        /// <param name="stronglyTypedVal">The strongly-typed value.</param>
        /// <returns>The string value that corresponds to the specified strongly-typed value.</returns>
        protected virtual string GetStringValueFromValue(T stronglyTypedVal)
        {
            if (stronglyTypedVal == null)
            {
                return null;
            }

            return stronglyTypedVal.ToString();
        }

        /// <summary>
        /// Converts a string value into a strongly-typed value.
        /// </summary>
        /// <param name="stringVal">The string value.</param>
        /// <returns>The strongly-typed value that corresponds to the specified string value.</returns>
        protected virtual T GetValueFromStringValue(string stringVal)
        {
            if (stringVal == null)
            {
                return default(T);
            }

            // a TypeConverter allows us to convert strings to Nullable<> objects
            TypeConverter conv = TypeDescriptor.GetConverter(typeof(T));
            return (T)conv.ConvertFrom(stringVal);
        }
    }
}
