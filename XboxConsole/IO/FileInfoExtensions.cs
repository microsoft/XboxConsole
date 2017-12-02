//------------------------------------------------------------------------------
// <copyright file="FileInfoExtensions.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.IO
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents extension methods for the FileInfo type.
    /// </summary>
    public static class FileInfoExtensions
    {
        /// <summary>
        /// Copies a file from a PC to an Xbox.
        /// </summary>
        /// <param name="fileInfo">The file info object pointing to the PC file.</param>
        /// <param name="xboxPath">The path on the Xbox that will represent the copied file.</param>
        /// <param name="console">The Xbox console the file will be copied to.</param>
        /// <returns>A file info object pointing to the copied file on the Xbox.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Maintains consistency with the original XboxConsole.")]
        public static XboxFileInfo CopyTo(this FileInfo fileInfo, XboxPath xboxPath, XboxConsole console)
        {
            return CopyTo(fileInfo, xboxPath, console, null);
        }

        /// <summary>
        /// Copies a file from a PC to an Xbox.
        /// </summary>
        /// <param name="fileInfo">The file info object pointing to the PC file.</param>
        /// <param name="xboxPath">The path on the Xbox that will represent the copied file.</param>
        /// <param name="console">The Xbox console the file will be copied to.</param>
        /// <param name="metrics">The progress handler that the calling app uses to receive progress updates about metrics. This may be null.</param>
        /// <returns>A file info object pointing to the copied file on the Xbox.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Maintains consistency with the original XboxConsole.")]
        public static XboxFileInfo CopyTo(this FileInfo fileInfo, XboxPath xboxPath, XboxConsole console, IProgress<XboxFileTransferMetric> metrics)
        {
            if (fileInfo == null)
            {
                throw new ArgumentNullException("fileInfo");
            }

            if (xboxPath == null)
            {
                throw new ArgumentNullException("xboxPath");
            }

            if (console == null)
            {
                throw new ArgumentNullException("console");
            }

            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException("Cannot copy a file that does not exist", fileInfo.FullName);
            }

            console.Adapter.SendFile(console.SystemIpAddressAndSessionKeyCombined, fileInfo.FullName, xboxPath, metrics);

            return new XboxFileInfo(xboxPath, console);
        }
    }
}
