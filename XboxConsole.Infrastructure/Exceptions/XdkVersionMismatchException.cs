//------------------------------------------------------------------------------
// <copyright file="XdkVersionMismatchException.cs" company="Microsoft">
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
    /// An exception that is thrown if we detect that XDK versions don't match when performing certain actions.
    /// For example, loading a configuration file saved on another XDK version.
    /// </summary>
    [Serializable]
    public class XdkVersionMismatchException : XboxConsoleException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XdkVersionMismatchException"/> class.
        /// </summary>
        public XdkVersionMismatchException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XdkVersionMismatchException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public XdkVersionMismatchException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XdkVersionMismatchException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public XdkVersionMismatchException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XdkVersionMismatchException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        protected XdkVersionMismatchException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}
