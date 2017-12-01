//------------------------------------------------------------------------------
// <copyright file="XboxConsoleFeatureNotSupportedException.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox
{
    using System;

    /// <summary>
    /// An exception that is thrown if a feature (method, property, API, etc) is not supported, typically in a particular version of XDK.
    /// </summary>
    [Serializable]
    public class XboxConsoleFeatureNotSupportedException : XboxConsoleException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XboxConsoleFeatureNotSupportedException"/> class.
        /// </summary>
        public XboxConsoleFeatureNotSupportedException()
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxConsoleFeatureNotSupportedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public XboxConsoleFeatureNotSupportedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxConsoleFeatureNotSupportedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public XboxConsoleFeatureNotSupportedException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxConsoleFeatureNotSupportedException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        protected XboxConsoleFeatureNotSupportedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}
