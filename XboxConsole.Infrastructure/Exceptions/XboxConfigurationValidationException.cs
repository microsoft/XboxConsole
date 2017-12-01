//------------------------------------------------------------------------------
// <copyright file="XboxConfigurationValidationException.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security;
using System.Xml.Schema;

namespace Microsoft.Internal.GamesTest.Xbox.Exceptions
{
    /// <summary>
    /// An exception that is thrown if the xml file loaded into a XboxConfiguration fails validation.
    /// </summary>
    [Serializable]
    public class XboxConfigurationValidationException : XboxException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XboxConfigurationValidationException"/> class.
        /// </summary>
        public XboxConfigurationValidationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxConfigurationValidationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public XboxConfigurationValidationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxConfigurationValidationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public XboxConfigurationValidationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxConfigurationValidationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="validationErrors">The list of validation errors.</param>
        public XboxConfigurationValidationException(string message, IEnumerable<ValidationEventArgs> validationErrors)
            : base(message)
        {
            this.ValidationErrors = validationErrors;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxConfigurationValidationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        /// <param name="validationErrors">The list of validation errors.</param>
        public XboxConfigurationValidationException(string message, Exception inner, IEnumerable<ValidationEventArgs> validationErrors)
            : base(message, inner)
        {
            this.ValidationErrors = validationErrors;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxConfigurationValidationException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        protected XboxConfigurationValidationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets the list of validation errors.
        /// </summary>
        public IEnumerable<ValidationEventArgs> ValidationErrors { get; private set; }

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
        }
    }
}
