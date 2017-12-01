//------------------------------------------------------------------------------
// <copyright file="XboxDeploymentInfo.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Deployment
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents an information progress update that has occured during deployment.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class XboxDeploymentInfo
    {
        /// <summary>
        /// Initializes a new instance of the XboxDeploymentInfo class.
        /// </summary>
        /// <param name="info">The information string of the deployment.</param>
        public XboxDeploymentInfo(string info)
        {
            this.Info = info;
        }

        /// <summary>
        /// Gets the information string of the deployment.
        /// </summary>
        public string Info { get; private set; }
    }
}
