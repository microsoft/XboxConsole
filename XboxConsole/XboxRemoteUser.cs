//------------------------------------------------------------------------------
// <copyright file="XboxRemoteUser.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox
{
    using System;
    using Microsoft.Internal.GamesTest.Xbox.Input;

    /// <summary>
    /// A virtual user object referring to a user on another console.
    /// </summary>
    public class XboxRemoteUser : XboxUserBase
    {
        /// <summary>
        /// Backing variable to store user's XUID.
        /// </summary>
        private string userXuid;

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxRemoteUser"/> class.
        /// </summary>
        /// <param name="console">The XboxConsole object that this object will be associated with.</param>
        /// <param name="xuid">The unique Xbox Live identifier to initialize with.</param>
        internal XboxRemoteUser(XboxConsole console, string xuid)
            : base(console)
        {
            this.userXuid = xuid;
        }

        /// <summary>
        /// Gets the user's unique Xbox Live identifier.
        /// </summary>
        public override string Xuid
        {
            get
            {
                return this.userXuid;
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="XboxRemoteUser"/> class.
        /// </summary>
        /// <param name="console">The XboxConsole object that this object will be associated with.</param>
        /// <param name="xuid">A unique Xbox Live identifier for the user.</param>
        /// <returns>A new instance of XboxRemoteUser.</returns>
        public static XboxRemoteUser FromXuid(XboxConsole console, string xuid)
        {
            return new XboxRemoteUser(console, xuid);
        }
    }
}
