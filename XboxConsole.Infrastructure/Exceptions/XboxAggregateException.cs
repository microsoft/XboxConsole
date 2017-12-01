//------------------------------------------------------------------------------
// <copyright file="XboxAggregateException.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Security;

    /// <summary>
    /// Represents an XboxException that aggregates other exceptions.
    /// </summary>
    [Serializable]
    public class XboxAggregateException : XboxConsoleException
    {
        /// <summary>
        /// A collection of inner exceptions.
        /// </summary>
        /// <remarks>
        /// This class requires a concrete collection type so that it can be serialized.
        /// </remarks>
        private List<Exception> innerExceptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxAggregateException"/> class.
        /// </summary>
        public XboxAggregateException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxAggregateException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public XboxAggregateException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxAggregateException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public XboxAggregateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxAggregateException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerExceptions">The inner exceptions.</param>
        public XboxAggregateException(string message, params Exception[] innerExceptions)
            : base(message, innerExceptions.FirstOrDefault())
        {
            this.InnerExceptions = innerExceptions;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxAggregateException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerExceptions">The inner exceptions.</param>
        public XboxAggregateException(string message, IEnumerable<Exception> innerExceptions)
            : base(message, innerExceptions.FirstOrDefault())
        {
            this.InnerExceptions = innerExceptions;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxAggregateException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="xboxName">The Xbox name.</param>
        public XboxAggregateException(string message, string xboxName)
            : base(message, xboxName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxAggregateException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="xboxName">The Xbox name.</param>
        public XboxAggregateException(string message, Exception innerException, string xboxName)
            : base(message, innerException, xboxName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxAggregateException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="xboxName">The Xbox name.</param>
        /// <param name="innerExceptions">The inner exceptions.</param>
        public XboxAggregateException(string message, string xboxName, params Exception[] innerExceptions)
            : base(message, innerExceptions.FirstOrDefault(), xboxName)
        {
            this.InnerExceptions = innerExceptions;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxAggregateException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="xboxName">The Xbox name.</param>
        /// <param name="innerExceptions">The inner exceptions.</param>
        public XboxAggregateException(string message, string xboxName, IEnumerable<Exception> innerExceptions)
            : base(message, innerExceptions.FirstOrDefault(), xboxName)
        {
            this.InnerExceptions = innerExceptions;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxAggregateException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        protected XboxAggregateException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets a collection of the Exception instances that caused the current exception.
        /// </summary>
        public IEnumerable<Exception> InnerExceptions
        {
            get { return this.innerExceptions; }

            private set { this.innerExceptions = value.ToList(); }
        }

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
            info.AddValue("InnerExceptions", this.innerExceptions, typeof(List<Exception>));
        }
    }
}
