//------------------------------------------------------------------------------
// <copyright file="XboxConsoleException.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox
{
    using System;
    using System.Runtime.Serialization;
    using System.Security;

    /// <summary>
    /// Represents an XboxException where the cause of the exception was a specific Xbox.
    /// This exception is in the Microsoft.Internal.GamesTest.Xbox namespace and not the
    /// Microsoft.Internal.GamesTest.Xbox.Exceptions namespace in order to maintain consistency
    /// with older XboxConsole built for the Xbox 360.
    /// </summary>
    [Serializable]
    public class XboxConsoleException : XboxException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XboxConsoleException"/> class.
        /// </summary>
        public XboxConsoleException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxConsoleException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public XboxConsoleException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxConsoleException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public XboxConsoleException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxConsoleException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="xboxName">The Xbox name.</param>
        public XboxConsoleException(string message, string xboxName)
            : base(message)
        {
            this.XboxName = xboxName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxConsoleException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="xboxName">The Xbox name.</param>
        public XboxConsoleException(string message, Exception innerException, string xboxName)
            : base(message, innerException)
        {
            this.XboxName = xboxName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxConsoleException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        protected XboxConsoleException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.XboxName = info.GetString("XboxName");
        }

        /// <summary>
        /// Gets the Xbox that threw the original COMException.
        /// </summary>
        public string XboxName { get; private set; }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is a null reference (Nothing in Visual Basic).
        /// </exception>
        [SecurityCritical]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            base.GetObjectData(info, context);
            info.AddValue("XboxName", this.XboxName, typeof(string));
        }
    }
}
