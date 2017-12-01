//------------------------------------------------------------------------------
// <copyright file="XboxRemoteParty.cs" company="Microsoft">
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
    /// Represents a party started on another console to which an invitation can be accepted or declined.
    /// </summary>
    public class XboxRemoteParty : XboxPartyBase
    {
        /// <summary>
        /// Backing variable for the PartyID property.
        /// </summary>
        private string partyId;

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxRemoteParty"/> class.
        /// </summary>
        /// <param name="console">The XboxConsole object that this object will be associated with.</param>
        /// <param name="initPartyId">Remote party ID to initialize with.</param>
        internal XboxRemoteParty(XboxConsole console, string initPartyId)
            : base(console)
        {
            XboxConsoleEventSource.Logger.ObjectCreated(XboxConsoleEventSource.GetCurrentConstructor());

            this.partyId = initPartyId;
        }

        /// <summary>
        /// Gets the Party ID useful for interoperation with external tools.
        /// </summary>
        public override string PartyId
        {
            get
            {
                return this.partyId;
            }
        }

        /// <summary>
        /// Gets the remote party associated with a specified Party ID.
        /// </summary>
        /// <param name="console">The XboxConsole object that this object will be associated with.</param>
        /// <param name="partyId">Unique identifier of a party created on another console.</param>
        /// <returns>The remote party object which can be passed into other functions.</returns>
        public static XboxRemoteParty FromPartyId(XboxConsole console, string partyId)
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            return new XboxRemoteParty(console, partyId);
        }
    }
}
