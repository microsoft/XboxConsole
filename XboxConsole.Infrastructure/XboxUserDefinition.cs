//------------------------------------------------------------------------------
// <copyright file="XboxUserDefinition.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox
{
    /// <summary>
    /// Represents the data of a user on an Xbox console.
    /// </summary>
    internal class XboxUserDefinition
    {
        /// <summary>
        /// Initializes a new instance of the XboxUserDefinition class.
        /// </summary>
        /// <param name="userId">A unique identifier for the user.</param>
        /// <param name="emailAddress">The email address used to sign the user in.</param>
        /// <param name="gamertag">The user's Gamertag.</param>
        /// <param name="signedIn">Is the user is signed in.</param>
        /// <param name="autoSignIn">Whether the user is signed in automatically.</param>
        /// <param name="xuid">User unique Live identifier.</param>
        public XboxUserDefinition(uint userId, string emailAddress, string gamertag, bool signedIn, bool autoSignIn, string xuid)
        {
            this.UserId = userId;
            this.EmailAddress = emailAddress;
            this.Gamertag = gamertag;
            this.IsSignedIn = signedIn;
            this.AutoSignIn = autoSignIn;
            this.Xuid = xuid;
        }

        /// <summary>
        /// Initializes a new instance of the XboxUserDefinition class.
        /// </summary>
        /// <param name="userId">A unique identifier for the user.</param>
        /// <param name="emailAddress">The email address used to sign the user in.</param>
        /// <param name="gamertag">The user's Gamertag.</param>
        /// <param name="signedIn">Is the user is signed in.</param>
        public XboxUserDefinition(uint userId, string emailAddress, string gamertag, bool signedIn)
        {
            this.UserId = userId;
            this.EmailAddress = emailAddress;
            this.Gamertag = gamertag;
            this.IsSignedIn = signedIn;
            this.AutoSignIn = false;
            this.Xuid = string.Empty;
        }

        /// <summary>
        /// Gets a unique identifier for the user.
        /// </summary>
        public uint UserId { get; private set; }

        /// <summary>
        /// Gets the email address used to sign the user in.
        /// </summary>
        public string EmailAddress { get; private set; }

        /// <summary>
        /// Gets the user's Gamertag.
        /// </summary>
        public string Gamertag { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the user is signed in.
        /// </summary>
        public bool IsSignedIn { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the user is setup for automatic sign in.
        /// </summary>
        public bool AutoSignIn { get; private set; }

        /// <summary>
        /// Gets the unique Xbox Live identifier for the user.
        /// </summary>
        public string Xuid { get; private set; }
    }
}
