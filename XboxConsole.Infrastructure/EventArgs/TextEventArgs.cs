//------------------------------------------------------------------------------
// <copyright file="TextEventArgs.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox
{
    using System;

    /// <summary>
    /// Represents event data for the TextReceived event.
    /// </summary>
    public class TextEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public TextEventArgs(string message)
            : this(null, message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextEventArgs"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="message">The message.</param>
        public TextEventArgs(string source, string message)
        {
            this.Source = source;
            this.Message = message;
        }

        /// <summary>
        /// Gets the message that was received.
        /// </summary>
        public string Source { get; private set; }

        /// <summary>
        /// Gets the message that was received.
        /// </summary>
        public string Message { get; private set; }
    }
}