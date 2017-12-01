//------------------------------------------------------------------------------
// <copyright file="XboxFileTransferMetric.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.IO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents the metrics for a progress update.
    /// </summary>
    public class XboxFileTransferMetric
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XboxFileTransferMetric"/> class.
        /// </summary>
        /// <param name="sourceFilePath">The path of the source file.</param>
        /// <param name="targetFilePath">The path to the target file.</param>
        /// <param name="fileSize">The file size of the file being transferred.</param>
        /// <param name="fileBytesTransferred">The number of bytes of the current file that have been transferred.</param>
        /// <param name="totalSize">The total number of bytes being transferred.</param>
        /// <param name="totalBytesTransferred">The total number of of bytes that have been transferred.</param>
        public XboxFileTransferMetric(
            string sourceFilePath,
            string targetFilePath,
            double fileSize,
            double fileBytesTransferred,
            double totalSize,
            double totalBytesTransferred)
        {
            this.SourceFilePath = sourceFilePath;
            this.TargetFilePath = targetFilePath;

            this.FileSizeInBytes = fileSize;
            this.FileBytesTransferred = fileBytesTransferred;
            this.PercentageFileBytesTransferred = (fileSize == 0.0) ? 0.0 : (double)fileBytesTransferred / (double)fileSize;

            this.TotalSizeInBytes = totalSize;
            this.TotalBytesTransferred = totalBytesTransferred;
            this.PercentageTotalBytesTransferred = (totalSize == 0.0) ? 0.0 : (double)totalBytesTransferred / (double)totalSize;
        }

        /// <summary>
        /// Gets the path of the source file.
        /// </summary>
        public string SourceFilePath { get; private set; }

        /// <summary>
        /// Gets the path of the target file.
        /// </summary>
        public string TargetFilePath { get; private set; }

        /// <summary>
        /// Gets the number of bytes that have been transferred for the current file.
        /// </summary>
        public double FileBytesTransferred { get; private set; }

        /// <summary>
        /// Gets the file size of the file being transferred.
        /// </summary>
        public double FileSizeInBytes { get; private set; }

        /// <summary>
        /// Gets the percentage, between 0.0 and 1.0, of the file that has been transferred.
        /// </summary>
        public double PercentageFileBytesTransferred { get; private set; }

        /// <summary>
        /// Gets the total number of bytes that have been transferred.
        /// </summary>
        public double TotalBytesTransferred { get; private set; }

        /// <summary>
        /// Gets the total number of bytes being transferred.
        /// </summary>
        public double TotalSizeInBytes { get; private set; }

        /// <summary>
        /// Gets the percentage, between 0.0 and 1.0, of the total number of bytes that have been transferred.
        /// </summary>
        public double PercentageTotalBytesTransferred { get; private set; }
    }
}
