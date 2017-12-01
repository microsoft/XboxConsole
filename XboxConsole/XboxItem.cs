//------------------------------------------------------------------------------
// <copyright file="XboxItem.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// A class that represents an item that is associated with an XboxConsole.
    /// Some examples include packages and files.
    /// </summary>
    public abstract class XboxItem
    {
        /// <summary>
        /// Initializes a new instance of the XboxItem class.
        /// </summary>
        /// <param name="console">The console that this item is associated with.</param>
        internal XboxItem(XboxConsole console)
        {
            if (console == null)
            {
                throw new ArgumentNullException("console");
            }

            this.Console = console;
        }

        /// <summary>
        /// Gets the XboxConsole that this item is associated with.
        /// </summary>
        public XboxConsole Console { get; private set; }
    }
}
