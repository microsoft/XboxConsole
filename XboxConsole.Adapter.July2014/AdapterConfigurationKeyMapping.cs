//------------------------------------------------------------------------------
// <copyright file="AdapterConfigurationKeyMapping.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Adapter.July2014
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This class holds mappings for configuration keys for Global XboxConsole keys to keys for the specific XDK.
    /// </summary>
    internal static class AdapterConfigurationKeyMapping
    {
        private static readonly Dictionary<string, string> KeyMappings = new Dictionary<string, string>()
            {
                { "SessionKey", "AccessKey" },
            };

        /// <summary>
        /// Maps the global XboxConsole configuration key to the XDK specific key.
        /// </summary>
        /// <param name="key">The key to map.</param>
        /// <returns>The XDK specific key.</returns>
        public static string MapKey(string key)
        {
            if (KeyMappings.ContainsKey(key))
            {
                return KeyMappings[key];
            }
            else
            {
                return key;
            }
        }
    }
}
