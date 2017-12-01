//------------------------------------------------------------------------------
// <copyright file="XboxDeploymentMetric.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Deployment
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents the metrics for a progress update.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class XboxDeploymentMetric
    {
        /// <summary>
        /// Initializes a new instance of the XboxDeploymentMetric class.
        /// </summary>
        /// <param name="filePath">The path of the file being currently transferred.</param>
        /// <param name="fileBytesTransferred">The number of bytes that have been transferred for the current file.</param>
        /// <param name="fileSizeInBytes">The file size of the file currently being transferred.</param>
        /// <param name="percentageFileBytesTransferred">The percentage, between 0.0 and 1.0, of the file that has been transferred.</param>
        /// <param name="totalBytes">The total size in bytes of the entire deployment.</param>
        /// <param name="totalBytesTransferred">The total number of bytes that have been transferred so far.</param>
        /// <param name="percentageTotalBytesTransferred">The percentage, between 0.0 and 1.0, of the deployment that has been transferred.</param>
        /// <param name="totalFiles">The total number of files in the deployment.</param>
        /// <param name="totalFilesTransferred">The number of files that have been completely transferred.</param>
        public XboxDeploymentMetric(
            string filePath,
            double fileBytesTransferred,
            double fileSizeInBytes,
            double percentageFileBytesTransferred,
            double totalBytes,
            double totalBytesTransferred,
            double percentageTotalBytesTransferred,
            double totalFiles,
            double totalFilesTransferred)
        {
            this.FilePath = filePath;
            this.FileBytesTransferred = fileBytesTransferred;
            this.FileSizeInBytes = fileSizeInBytes;
            this.PercentageFileBytesTransferred = percentageFileBytesTransferred;

            this.TotalBytes = totalBytes;
            this.TotalBytesTransferred = totalBytesTransferred;
            this.PercentageTotalBytesTransferred = percentageTotalBytesTransferred;

            this.TotalFiles = totalFiles;
            this.TotalFilesTransferred = totalFilesTransferred;
        }

        /// <summary>
        /// Gets the path of the file being currently transferred.
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// Gets the number of bytes that have been transferred for the current file.
        /// </summary>
        public double FileBytesTransferred { get; private set; }

        /// <summary>
        /// Gets the file size of the file currently being transferred.
        /// </summary>
        public double FileSizeInBytes { get; private set; }

        /// <summary>
        /// Gets the percentage, between 0.0 and 1.0, of the file that has been transferred.
        /// </summary>
        public double PercentageFileBytesTransferred { get; private set; }

        /// <summary>
        /// Gets the total size in bytes of the entire deployment.
        /// </summary>
        public double TotalBytes { get; private set; }

        /// <summary>
        /// Gets the total number of bytes that have been transferred so far.
        /// </summary>
        public double TotalBytesTransferred { get; private set; }

        /// <summary>
        /// Gets the percentage, between 0.0 and 1.0, of the deployment that has been transferred.
        /// </summary>
        public double PercentageTotalBytesTransferred { get; private set; }

        /// <summary>
        /// Gets the total number of files in the deployment.
        /// </summary>
        public double TotalFiles { get; private set; }

        /// <summary>
        /// Gets the number of files that have been completely transferred.
        /// </summary>
        public double TotalFilesTransferred { get; private set; }
    }
}
