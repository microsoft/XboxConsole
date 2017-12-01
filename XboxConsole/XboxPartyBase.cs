//------------------------------------------------------------------------------
// <copyright file="XboxPartyBase.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox
{
    using System;
    using System.Linq;
    using Microsoft.Internal.GamesTest.Xbox.Input;
    using Microsoft.Internal.GamesTest.Xbox.Telemetry;

    /// <summary>
    /// Virtual party base class.
    /// </summary>
    public abstract class XboxPartyBase : XboxItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XboxPartyBase"/> class.
        /// </summary>
        /// <param name="console">The XboxConsole object that the party object is associated with.</param>
        internal XboxPartyBase(XboxConsole console)
            : base(console)
        {
        }

        /// <summary>
        /// Gets the unique party identifier.
        /// </summary>
        public abstract string PartyId
        {
            get;
        }
    }
}