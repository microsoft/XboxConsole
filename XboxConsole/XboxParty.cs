//------------------------------------------------------------------------------
// <copyright file="XboxParty.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Internal.GamesTest.Xbox.Input;

    /// <summary>
    /// Represents a party started by a local user.
    /// </summary>
    public class XboxParty : XboxPartyBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XboxParty"/> class.
        /// </summary>
        /// <param name="console">The console on which the party is to be created.</param>
        /// <param name="titleId">The Title ID of the title the party is for.</param>
        /// <returns>The XboxParty object which can be used with XboxUser party management functions or to get party members.</returns>
        internal XboxParty(XboxConsole console, uint titleId)
            : base(console)
        {
            this.TitleId = titleId;
        }

        /// <summary>
        /// Gets the TitleID of the title the party has been created for.
        /// </summary>
        public uint TitleId
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the local party ID for the title the party was created for.
        /// </summary>
        public override string PartyId
        {
            get
            {
                return Console.Adapter.GetPartyId(Console.SystemIpAddressAndSessionKeyCombined, this.TitleId);
            }
        }

        /// <summary>
        /// Gets the party member XUIDs.
        /// </summary>
        public IEnumerable<XboxUserBase> Members
        {
            get
            {
                string[] members = this.Console.Adapter.GetPartyMembers(this.Console.ConnectionString, this.TitleId);

                XboxUser[] localUsers = this.Console.Users.ToArray();

                XboxUserBase[] memberObjects = new XboxUserBase[members.Length];

                for (int n = 0; n < memberObjects.Length; n++)
                {
                    XboxUser localUser = (from xu in localUsers where xu.IsSignedIn && xu.Xuid == members[n] select xu).SingleOrDefault();

                    if (localUser == null)
                    {
                        memberObjects[n] = new XboxRemoteUser(this.Console, members[n]);
                    }
                    else
                    {
                        memberObjects[n] = localUser;
                    }
                }

                return memberObjects;
            }
        }

        /// <summary>
        /// Gets the local party associated with a specified Title ID.
        /// Until the party has been created, this object will throw an XboxConsoleException upon use.
        /// </summary>
        /// <param name="console">The console object on which the title's party has been created.</param>
        /// <param name="titleId">Title ID the party is for.</param>
        /// <returns>The party object which can be passed into other functions or for getting party members.</returns>
        public static XboxParty FromTitleId(XboxConsole console, uint titleId)
        {
            return new XboxParty(console, titleId);
        }
    }
}
