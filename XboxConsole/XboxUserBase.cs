//------------------------------------------------------------------------------
// <copyright file="XboxUserBase.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox
{
    using System;
    using System.Linq;
    using Microsoft.Internal.GamesTest.Xbox.Input;

    /// <summary>
    /// The base class of virtual users.
    /// </summary>
    public abstract class XboxUserBase : XboxItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XboxUserBase"/> class.
        /// </summary>
        /// <param name="console">The XboxConsole object that this object will be associated with.</param>
        internal XboxUserBase(XboxConsole console) : base(console)
        {
        }

        /// <summary>
        /// Gets a unique Xbox Live identifier for the user.
        /// </summary>
        public abstract string Xuid
        {
            get;
        }
    }
}