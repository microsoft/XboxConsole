//------------------------------------------------------------------------------
// <copyright file="XboxFile.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.IO
{
    using System;
    using System.IO;

    /// <summary>
    /// Performs file input/output operations on an Xbox console.
    /// </summary>
    public static class XboxFile
    {
        /// <summary>
        /// Copies a file from a PC to an Xbox console.
        /// </summary>
        /// <param name="sourceFile">The file to copy from.</param>
        /// <param name="destinationFile">The file to copy to.</param>
        /// <param name="console">The Xbox to copy to.</param>
        /// <exception cref="Microsoft.Internal.GamesTest.Xbox.XboxConsoleFeatureNotSupportedException">Thrown if the destination file is not a path with an Xbox origin.</exception>
        public static void Copy(string sourceFile, XboxPath destinationFile, XboxConsole console)
        {
            Copy(sourceFile, destinationFile, console, null);
        }

        /// <summary>
        /// Copies a file from a PC to an Xbox console.
        /// </summary>
        /// <param name="sourceFile">The file to copy from.</param>
        /// <param name="destinationFile">The file to copy to.</param>
        /// <param name="console">The Xbox to copy to.</param>
        /// <param name="metrics">The progress handler that the calling app uses to receive progress updates about metrics. This may be null.</param>
        /// <exception cref="Microsoft.Internal.GamesTest.Xbox.XboxConsoleFeatureNotSupportedException">Thrown if the destination file is not a path with an Xbox origin.</exception>
        public static void Copy(string sourceFile, XboxPath destinationFile, XboxConsole console, IProgress<XboxFileTransferMetric> metrics)
        {
            if (destinationFile == null)
            {
                throw new ArgumentNullException("destinationFile");
            }

            if (XboxPath.HasXboxOrigin(destinationFile.FullName))
            {
                new FileInfo(sourceFile).CopyTo(destinationFile, console, metrics);
            }
            else
            {
                throw new XboxConsoleFeatureNotSupportedException("The destination XboxPath object does not contain a valid Xbox file path.");
            }
        }

        /// <summary>
        /// Copies a file from an Xbox console to a PC.
        /// </summary>
        /// <param name="sourceFile">The file to copy from.</param>
        /// <param name="destinationFile">The file to copy to. </param>
        /// <param name="console">The Xbox to copy from.</param>
        /// <exception cref="Microsoft.Internal.GamesTest.Xbox.XboxConsoleFeatureNotSupportedException">Thrown if the source file is not a path with an Xbox origin.</exception>
        public static void Copy(XboxPath sourceFile, string destinationFile, XboxConsole console)
        {
            Copy(sourceFile, destinationFile, console, null);
        }

        /// <summary>
        /// Copies a file from an Xbox console to a PC.
        /// </summary>
        /// <param name="sourceFile">The file to copy from.</param>
        /// <param name="destinationFile">The file to copy to. </param>
        /// <param name="console">The Xbox to copy from.</param>
        /// <param name="metrics">The progress handler that the calling app uses to receive progress updates about metrics. This may be null.</param>
        /// <exception cref="Microsoft.Internal.GamesTest.Xbox.XboxConsoleFeatureNotSupportedException">Thrown if the source file is not a path with an Xbox origin.</exception>
        public static void Copy(XboxPath sourceFile, string destinationFile, XboxConsole console, IProgress<XboxFileTransferMetric> metrics)
        {
            if (sourceFile == null)
            {
                throw new ArgumentNullException("sourceFile");
            }

            if (XboxPath.HasXboxOrigin(sourceFile.FullName))
            {
                new XboxFileInfo(sourceFile, console).Copy(destinationFile, metrics);
            }
            else
            {
                throw new XboxConsoleFeatureNotSupportedException("The source XboxPath object does not contain a valid Xbox file path.");
            }
        }

        /// <summary>
        /// Deletes a file from an Xbox.
        /// </summary>
        /// <param name="file">The file to delete from the Xbox.</param>
        /// <param name="console">The Xbox that contains the file to delete.</param>
        public static void Delete(XboxPath file, XboxConsole console)
        {
            new XboxFileInfo(file, console).Delete();
        }

        /// <summary>
        /// Investigates whether a file exists on an Xbox.
        /// </summary>
        /// <param name="file">The file to look for.</param>
        /// <param name="console">The Xbox Console to look on.</param>
        /// <returns>A value indicating whether the file exists on the console or not.</returns>
        public static bool Exists(XboxPath file, XboxConsole console)
        {
            if (console == null)
            {
                throw new ArgumentNullException("console");
            }

            return XboxFileInfo.ExistsImpl(console.SystemIpAddressAndSessionKeyCombined, file, console.Adapter);
        }

        /// <summary>
        /// Moves a file from a PC to an Xbox console.
        /// </summary>
        /// <param name="sourceFile">The file to move from.</param>
        /// <param name="destinationFile">The file to move to.</param>
        /// <param name="console">The console to move the file to.</param>
        public static void Move(string sourceFile, XboxPath destinationFile, XboxConsole console)
        {
            Copy(sourceFile, destinationFile, console);
            new FileInfo(sourceFile).Delete();
        }

        /// <summary>
        /// Moves a file from an Xbox console to a PC.
        /// </summary>
        /// <param name="sourceFile">The file to move from.</param>
        /// <param name="destinationFile">The file to move to.</param>
        /// <param name="console">The console to move the file from.</param>
        public static void Move(XboxPath sourceFile, string destinationFile, XboxConsole console)
        {
            Copy(sourceFile, destinationFile, console);
            new XboxFileInfo(sourceFile, console).Delete();
        }
    }
}
