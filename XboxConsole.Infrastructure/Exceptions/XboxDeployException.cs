//------------------------------------------------------------------------------
// <copyright file="XboxDeployException.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox
{
    using System;
    using System.Runtime.Serialization;
    using System.Security;

    /// <summary>
    /// Represents an XboxException where the cause of the exception concerns build deployment.
    /// </summary>
    [Serializable]
    public class XboxDeployException : XboxConsoleException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XboxDeployException"/> class.
        /// </summary>
        public XboxDeployException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxDeployException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public XboxDeployException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxDeployException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public XboxDeployException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxDeployException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="xboxName">The Xbox name.</param>
        public XboxDeployException(string message, string xboxName)
            : base(message, xboxName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxDeployException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="xboxName">The Xbox name.</param>
        public XboxDeployException(string message, Exception innerException, string xboxName)
            : base(message, innerException, xboxName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxDeployException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        protected XboxDeployException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
