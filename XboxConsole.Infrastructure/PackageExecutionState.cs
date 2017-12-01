//------------------------------------------------------------------------------
// <copyright file="PackageExecutionState.cs" company="Microsoft">
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
    /// An enumeration representing each possible state of an XboxPackage.
    /// </summary>
    public enum PackageExecutionState
    {
        /// <summary>
        /// Unable to determine the state of the package.
        /// </summary>
        Unknown,

        /// <summary>
        /// The package is currently running.
        /// </summary>
        Running,

        /// <summary>
        /// The package is being suspended.
        /// </summary>
        Suspending,

        /// <summary>
        /// The package is supsended.
        /// </summary>
        Suspended,

        /// <summary>
        /// The package has been terminated.
        /// </summary>
        Terminated,

        /// <summary>
        /// The package has been constrained.
        /// </summary>
        Constrained
    }
}
