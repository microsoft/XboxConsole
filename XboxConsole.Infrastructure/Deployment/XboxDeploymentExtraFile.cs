//------------------------------------------------------------------------------
// <copyright file="XboxDeploymentExtraFile.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Deployment
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents a progress update about extra file on the console.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class XboxDeploymentExtraFile
    {
        /// <summary>
        /// Initializes a new instance of the XboxDeploymentExtraFile class.
        /// </summary>
        /// <param name="filePath">The path on the console of the extra file.</param>
        /// <param name="extraFileDetected">A boolean that specifies whether an extra file has been detected.</param>
        /// <param name="extraFileRemoved">A boolean that specifies whether an extra file has been removed.</param>
        public XboxDeploymentExtraFile(string filePath, bool extraFileDetected, bool extraFileRemoved)
        {
            this.FilePath = filePath;
            this.ExtraFileDetected = extraFileDetected;
            this.ExtraFileRemoved = extraFileRemoved;
        }

        /// <summary>
        /// Gets the path, on the console, of the extra file.
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not an extra file has been detected.
        /// </summary>
        public bool ExtraFileDetected { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not an extra file has been removed.
        /// </summary>
        public bool ExtraFileRemoved { get; private set; }
    }
}
