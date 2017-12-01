//------------------------------------------------------------------------------
// <copyright file="XboxPreferredLanguagesConfigurationSetting.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// An explicit override of the XboxConfigurationSetting class meant to represent the list of strings
    /// used to update the PreferredLanguages configuration setting (see xbconfig command line utility).
    /// </summary>
    internal class XboxPreferredLanguagesConfigurationSetting : XboxConfigurationSetting<IEnumerable<CultureInfo>>
    {
        private Regex
            regexCultureInfoScript = new Regex(@"[-]\w{4}");

        /// <summary>
        /// Initializes a new instance of the XboxPreferredLanguagesConfigurationSetting class.
        /// </summary>
        /// <param name="key">The setting key.</param>
        internal XboxPreferredLanguagesConfigurationSetting(string key)
            : base(key)
        {
        }
        
        /// <summary>
        /// Compares an XboxPreferredLanguagesConfigurationSetting object with a string, which is assumed to be a
        /// semicolon-delimited list of language-and-region information (see xbconfig output). This output
        /// may not coincide perfectly with .NET CultureInfo definitions; correspondingly, the four-letter script identifier
        /// will be removed, and the CultureInfo definitions may be compared as neutral cultures.
        /// </summary>
        /// <param name="stringVal">The string to be compared against for equality.</param>
        /// <remarks>This function is used for testing purposes only.</remarks>
        /// <returns>A boolean value indicating equality.</returns>
        internal override bool EqualsConfigString(string stringVal)
        {
            IEnumerable<CultureInfo> cultures = this.ConvertConfigurationStringsToCultureInfo(this.SplitConfigurationString(stringVal));
            if (this.Value.SequenceEqual(cultures))
            {
                return true;
            }

            // assume that the the stringVal is a list of values returned from the Xbox One, 
            // which has a much greater chance of including
            // a neutral culture that doesn't correspond to one of our specific cultures
            // because of this, we'll convert the cultures from the XboxPreferredLanguagesConfigurationSetting
            // into neutral cultures and see if they exist within the list derived from stringVal
            foreach (CultureInfo settingCulture in this.Value)
            {
                if (!cultures.Contains(settingCulture))
                {
                    string neutralCultureString = this.ConvertConfigurationStringToNeutralCulture(settingCulture.ToString());
                    CultureInfo neutralCulture = new CultureInfo(neutralCultureString);
                    if (!cultures.Contains(neutralCulture))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Converts a list of CultureInfo classes into the string as expected by the PreferredLanguages
        /// configuration setting (e.g. "en-US;en-GB;en-AU").
        /// </summary>
        /// <param name="cultures">The list of languages to be parsed.</param>
        /// <returns>The string value that corresponds to the specified list of CultureInfo classes.</returns>
        protected override string GetStringValueFromValue(IEnumerable<CultureInfo> cultures)
        {
            if (cultures == null)
            {
                return null;
            }

            // we don't support custom CultureInfo definitions
            if (CultureInfo.GetCultures(CultureTypes.UserCustomCulture).Any(culture => cultures.Contains(culture)))
            {
                throw new ArgumentException("The Xbox One console does not support custom .NET CultureInfo definitions.", "cultures");
            }

            return cultures.Select(culture => culture.ToString()).Aggregate((culture1, culture2) => string.Format(CultureInfo.InvariantCulture, "{0};{1}", culture1, culture2));
        }

        /// <summary>
        /// Converts a string value into a list of CultureInfo classes.
        /// </summary>
        /// <param name="stringVal">The string value.</param>
        /// <returns>The list of CultureInfo classes that corresponds to the specified string value.</returns>
        protected override IEnumerable<CultureInfo> GetValueFromStringValue(string stringVal)
        {
            if (stringVal == null)
            {
                return null;
            }

            IEnumerable<CultureInfo> cultures = this.ConvertConfigurationStringsToCultureInfo(this.SplitConfigurationString(stringVal));

            // we don't support custom CultureInfo definitions
            if (CultureInfo.GetCultures(CultureTypes.UserCustomCulture).Any(culture => cultures.Contains(culture)))
            {
                throw new ArgumentException("The Xbox One console does not support custom .NET CultureInfo definitions.", "stringVal");
            }

            return cultures;
        }

        /// <summary>
        /// Splits a semicolon-delimited string.
        /// </summary>
        /// <param name="configString">The semicolon-delimited string to be split.</param>
        /// <returns>The individual strings as split from the original.</returns>
        private IEnumerable<string> SplitConfigurationString(string configString)
        {
            return configString.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Takes a string representation of a .NET CultureInfo class and removes the script identifiers, but leaves
        /// the region identifiers in place.
        /// </summary>
        /// <param name="configString">The culture to be converted.</param>
        /// <returns>The converted culture.</returns>
        private string ConvertConfigurationStringRemoveScript(string configString)
        {
            if (this.regexCultureInfoScript.IsMatch(configString))
            {
                configString = configString.Replace(this.regexCultureInfoScript.Match(configString).Value, string.Empty);
            }

            return configString;
        }

        /// <summary>
        /// Takes a string representation of a .NET CultureInfo class and removes all identifiers in order
        /// to convert the representation into a neutral culture.
        /// </summary>
        /// <param name="configString">The culture to be converted.</param>
        /// <returns>The neutral culture.</returns>
        private string ConvertConfigurationStringToNeutralCulture(string configString)
        {
            return configString.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries).First();
        }

        /// <summary>
        /// Converts an IEnumerable of strings into an IEnumerable of .NET CultureInfo objects.
        /// </summary>
        /// <param name="configStrings">The IEnumerable of strings to be converted.</param>
        /// <returns>The IEnumerable of .NET CultureInfo objects.</returns>
        private IEnumerable<CultureInfo> ConvertConfigurationStringsToCultureInfo(IEnumerable<string> configStrings)
        {
            IEnumerable<CultureInfo> cultures;
            try
            {
                cultures = configStrings.Select(culture => new CultureInfo(this.ConvertConfigurationStringRemoveScript(culture)));
            }
            catch (CultureNotFoundException e)
            {
                throw new ArgumentException("One or more of the supplied configuration strings does not correspond to a valid .NET CultureInfo class.", "configStrings", e);
            }

            return cultures;
        }
    }
}
