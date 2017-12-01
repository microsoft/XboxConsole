//------------------------------------------------------------------------------
// <copyright file="XboxGeographicRegionConfigurationSetting.cs" company="Microsoft">
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
    /// An explicit override of the XboxConfigurationSetting class meant to represent the GeographicRegion
    /// configuration setting (see xbconfig command line utility).
    /// </summary>
    internal class XboxGeographicRegionConfigurationSetting : XboxConfigurationSetting<RegionInfo>
    {
        private Regex
            regexCultureInfoRegion = new Regex(@"(?<=-)\w{2}$");

        /// <summary>
        /// Initializes a new instance of the XboxGeographicRegionConfigurationSetting class.
        /// </summary>
        /// <param name="key">The setting key.</param>
        internal XboxGeographicRegionConfigurationSetting(string key)
            : base(key)
        {
        }

        /// <summary>
        /// Converts a string value into a RegionInfo class.
        /// </summary>
        /// <param name="stringVal">The string value.</param>
        /// <returns>The RegionInfo class that corresponds to the specified string value.</returns>
        protected override System.Globalization.RegionInfo GetValueFromStringValue(string stringVal)
        {
            if (stringVal == null)
            {
                return null;
            }

            RegionInfo region;
            try
            {
                region = new RegionInfo(stringVal);
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException("The supplied region string does not correspond to a valid .NET RegionInfo class.", "stringVal", e);
            }

            // we don't support custom RegionInfo definitions
            // unfortunately, these are a little harder to get, since the user can create a custom CultureInfo definition that uses a preexisting RegionInfo definition
            // so we're comparing against a list of .SpecificCultures except .UserCustomCultures, to ensure we're only examining a list of valid, non-custom RegionInfo defintions
            if (!CultureInfo.GetCultures(CultureTypes.SpecificCultures).Except(CultureInfo.GetCultures(CultureTypes.UserCustomCulture)).Any(culture => this.ConvertCultureInfoToRegionInfo(culture).Equals(region)))
            {
                throw new ArgumentException("The Xbox One console does not support custom .NET RegionInfo definitions.", "stringVal");
            }

            return region;
        }
        
        /// <summary>
        /// Returns a new RegionInfo object created from the region identifier of a CultureInfo object. 
        /// If no region identifier is present, the full CultureInfo description is used
        /// to create the RegionInfo object.
        /// </summary>
        /// <param name="cultureInfo">The CultureInfo object to be converted.</param>
        /// <returns>The RegionInfo object created from the CultureInfo definition.</returns>
        private RegionInfo ConvertCultureInfoToRegionInfo(CultureInfo cultureInfo)
        {
            string regionIdentifier = cultureInfo.ToString();
            if (this.regexCultureInfoRegion.IsMatch(regionIdentifier))
            {
                regionIdentifier = this.regexCultureInfoRegion.Match(regionIdentifier).Value;
            }

            return new RegionInfo(regionIdentifier);
        }
    }
}
