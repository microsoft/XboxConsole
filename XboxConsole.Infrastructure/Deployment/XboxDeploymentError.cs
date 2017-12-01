//------------------------------------------------------------------------------
// <copyright file="XboxDeploymentError.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Deployment
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents an error that has occured during deployment.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class XboxDeploymentError
    {
        /// <summary>
        /// Initializes a new instance of the XboxDeploymentError class.
        /// </summary>
        /// <param name="errorCode">The error code of the error.</param>
        public XboxDeploymentError(long errorCode)
        {
            this.ErrorCode = errorCode;
        }

        /// <summary>
        /// Gets the error code of the error that has occured.
        /// </summary>
        public long ErrorCode { get; private set; }
    }
}
