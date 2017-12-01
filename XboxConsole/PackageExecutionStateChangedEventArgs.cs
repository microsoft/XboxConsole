//------------------------------------------------------------------------------
// <copyright file="PackageExecutionStateChangedEventArgs.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;

namespace Microsoft.Internal.GamesTest.Xbox
{
    /// <summary>
    /// Represents event data for the ExecutionStateChanged event.
    /// </summary>
    public class PackageExecutionStateChangedEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PackageExecutionStateChangedEventArgs"/> class.
        /// </summary>
        /// <param name="package">The package that fired the event.</param>
        /// <param name="oldState">The old execution state of the package.</param>
        /// <param name="newState">The new execution state of the package.</param>
        public PackageExecutionStateChangedEventArgs(XboxPackage package, PackageExecutionState oldState, PackageExecutionState newState)
            : this(package, oldState, newState, null)
        {
            // Nothing needed here.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageExecutionStateChangedEventArgs"/> class.
        /// </summary>
        /// <param name="package">The package that fired the event.</param>
        /// <param name="oldState">The old execution state of the package.</param>
        /// <param name="newState">The new execution state of the package.</param>
        /// <param name="exception">The exception that occured during discovery of the state change.</param>
        public PackageExecutionStateChangedEventArgs(XboxPackage package, PackageExecutionState oldState, PackageExecutionState newState, Exception exception)
        {
            this.Package = package;
            this.OldState = oldState;
            this.NewState = newState;
            this.Exception = exception;
        }

        /// <summary>
        /// Gets the package that fired the event.
        /// </summary>
        public XboxPackage Package { get; private set; }

        /// <summary>
        /// Gets the old execution state of the package.
        /// </summary>
        public PackageExecutionState OldState { get; private set; }

        /// <summary>
        /// Gets the new execution state of the package.
        /// </summary>
        public PackageExecutionState NewState { get; private set; }

        /// <summary>
        /// Gets the exception that may have occured upon package state changes.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This property will only be non-<c>null</c> when something bad may have happened while discovering the new state of the package.
        ///     </para>
        ///     <para>
        ///         If this property is non-<c>null</c> than something bad has happened to the communication with the package; in this case, events
        ///         have most likely ceased. As such, to continue receiving events it will be better to get a new instance of the XboxPackage from
        ///         the XboxConsole class.
        ///     </para>
        /// </remarks>
        public Exception Exception { get; private set; }
    }
}
