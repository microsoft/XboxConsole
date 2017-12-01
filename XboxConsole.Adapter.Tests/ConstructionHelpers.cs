//------------------------------------------------------------------------------
// <copyright file="ConstructionHelpers.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdapterVersion = Microsoft.Internal.GamesTest.Xbox.Adapter.May2014;

namespace Microsoft.Internal.GamesTest.Xbox.Adapter.Tests
{
    /// <summary>
    /// Helpers to construct new instances of the adapter and xdk that is being tested.
    /// To change the adapter version being tested, update the using statement above.
    /// </summary>
    internal static class ConstructionHelpers
    {
        /// <summary>
        /// Creates the correct version of the XboxConsoleAdapter.
        /// </summary>
        /// <param name="xboxXdk">The fake XboxXdk to use.</param>
        /// <returns>The appropriate XboxConsoleAdapter object.</returns>
        public static XboxConsoleAdapterBase NewXboxConsoleAdapter(XboxXdkBase xboxXdk)
        {
            return new AdapterVersion.XboxConsoleAdapter(xboxXdk);
        }
    }
}
