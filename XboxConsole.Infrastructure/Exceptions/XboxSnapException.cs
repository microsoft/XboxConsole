//------------------------------------------------------------------------------
// <copyright file="XboxSnapException.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox
{
    using System;
    using System.Runtime.Serialization;
    using System.Security;

    /// <summary>
    /// Represents an XboxException where the cause of the exception concerns snapping applications.
    /// </summary>
    [Serializable]
    public class XboxSnapException : XboxConsoleException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XboxSnapException"/> class.
        /// </summary>
        public XboxSnapException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxSnapException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public XboxSnapException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxSnapException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public XboxSnapException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxSnapException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="xboxName">The Xbox name.</param>
        public XboxSnapException(string message, string xboxName)
            : base(message, xboxName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxSnapException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="xboxName">The Xbox name.</param>
        public XboxSnapException(string message, Exception innerException, string xboxName)
            : base(message, innerException, xboxName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxSnapException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        protected XboxSnapException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
