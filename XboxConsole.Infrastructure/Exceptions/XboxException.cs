//------------------------------------------------------------------------------
// <copyright file="XboxException.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    /// <summary>
    /// Represents the base Exception type for the XboxConsole library.
    /// This exception is in the Microsoft.Internal.GamesTest.Xbox namespace and not the
    /// Microsoft.Internal.GamesTest.Xbox.Exceptions namespace in order to maintain consistency
    /// with older XboxConsole built for the Xbox 360.
    /// </summary>
    [Serializable]
    public class XboxException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XboxException"/> class.
        /// </summary>
        public XboxException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public XboxException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public XboxException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        protected XboxException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
