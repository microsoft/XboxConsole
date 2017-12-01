//------------------------------------------------------------------------------
// <copyright file="CannotConnectException.cs" company="Microsoft">
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
    /// Represents an exception that is thrown when the library is unable to connect to
    /// an Xbox.
    /// </summary>
    [Serializable]
    public class CannotConnectException : XboxConsoleException
    {
        /// <summary>
        /// Initializes a new instance of the CannotConnectException class.
        /// </summary>
        public CannotConnectException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the CannotConnectException class.
        /// </summary>
        /// <param name="message">The message describing the purpose for throwing this exception.</param>
        public CannotConnectException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CannotConnectException class.
        /// </summary>
        /// <param name="message">The message describing the purpose for throwing this exception.</param>
        /// <param name="xboxName">The name of the Xbox for which the connection attempt failed.</param>
        public CannotConnectException(string message, string xboxName)
            : base(message, null, xboxName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CannotConnectException class.
        /// </summary>
        /// <param name="message">The message describing the purpose for throwing this exception.</param>
        /// <param name="innerException">Any applicable inner exception.</param>
        /// <param name="xboxName">The name of the Xbox for which the connection attempt failed.</param>
        public CannotConnectException(string message, Exception innerException, string xboxName)
            : base(message, innerException, xboxName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CannotConnectException class.
        /// </summary>
        /// <param name="message">The message describing the purpose for throwing this exception.</param>
        /// <param name="inner">Any applicable inner exception.</param>
        public CannotConnectException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CannotConnectException class.
        /// </summary>
        /// <param name="info">The SerializationInfo for this exception.</param>
        /// <param name="context">The StreamingContext for this exception.</param>
        protected CannotConnectException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}
