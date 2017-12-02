//------------------------------------------------------------------------------
// <copyright file="XboxDirectory.cs" company="Microsoft">
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

    /// <summary>
    /// Performs directory input/output operations on an Xbox console.
    /// </summary>
    public static class XboxDirectory
    {
        /// <summary>
        /// Copies a directory from a PC to an Xbox console.
        /// </summary>
        /// <param name="sourceDirectory">The directory to copy from.</param>
        /// <param name="destinationDirectory">The directory to copy to.</param>
        /// <param name="console">The Xbox to copy to.</param>
        /// <exception cref="Microsoft.Internal.GamesTest.Xbox.XboxConsoleFeatureNotSupportedException">Thrown if the destination directory is not a path with an Xbox origin.</exception>
        public static void Copy(string sourceDirectory, XboxPath destinationDirectory, XboxConsole console)
        {
            Copy(sourceDirectory, destinationDirectory, console, null);
        }

        /// <summary>
        /// Copies a directory from a PC to an Xbox console.
        /// </summary>
        /// <param name="sourceDirectory">The directory to copy from.</param>
        /// <param name="destinationDirectory">The directory to copy to.</param>
        /// <param name="console">The Xbox to copy to.</param>
        /// <param name="metrics">The progress handler that the calling app uses to receive progress updates about metrics. This may be null.</param>
        /// <exception cref="Microsoft.Internal.GamesTest.Xbox.XboxConsoleFeatureNotSupportedException">Thrown if the destination directory is not a path with an Xbox origin.</exception>
        public static void Copy(string sourceDirectory, XboxPath destinationDirectory, XboxConsole console, IProgress<XboxFileTransferMetric> metrics)
        {
            if (destinationDirectory == null)
            {
                throw new ArgumentNullException("destinationDirectory");
            }

            if (XboxPath.HasXboxOrigin(destinationDirectory.FullName))
            {
                new DirectoryInfo(sourceDirectory).CopyTo(destinationDirectory, console, metrics);
            }
            else
            {
                throw new XboxConsoleFeatureNotSupportedException("No Xbox path specified.  This method cannot be used for PC to PC transfers.");
            }
        }

        /// <summary>
        /// Copies a directory from an Xbox console to a PC.
        /// </summary>
        /// <param name="sourceDirectory">The directory to copy from.</param>
        /// <param name="destinationDirectory">The directory to copy to. </param>
        /// <param name="console">The Xbox to copy from.</param>
        /// <exception cref="Microsoft.Internal.GamesTest.Xbox.XboxConsoleFeatureNotSupportedException">Thrown if the source directory is not a path with an Xbox origin.</exception>
        public static void Copy(XboxPath sourceDirectory, string destinationDirectory, XboxConsole console)
        {
            Copy(sourceDirectory, destinationDirectory, console, null);
        }

        /// <summary>
        /// Copies a directory from an Xbox console to a PC.
        /// </summary>
        /// <param name="sourceDirectory">The directory to copy from.</param>
        /// <param name="destinationDirectory">The directory to copy to. </param>
        /// <param name="console">The Xbox to copy from.</param>
        /// <param name="metrics">The progress handler that the calling app uses to receive progress updates about metrics. This may be null.</param>
        /// <exception cref="Microsoft.Internal.GamesTest.Xbox.XboxConsoleFeatureNotSupportedException">Thrown if the source directory is not a path with an Xbox origin.</exception>
        public static void Copy(XboxPath sourceDirectory, string destinationDirectory, XboxConsole console, IProgress<XboxFileTransferMetric> metrics)
        {
            if (sourceDirectory == null)
            {
                throw new ArgumentNullException("sourceDirectory");
            }

            if (XboxPath.HasXboxOrigin(sourceDirectory.FullName))
            {
                new XboxDirectoryInfo(sourceDirectory, console).Copy(destinationDirectory, metrics);
            }
            else
            {
                throw new XboxConsoleFeatureNotSupportedException("No Xbox path specified.  This method cannot be used for PC to PC transfers.");
            }
        }

        /// <summary>
        /// Creates a directory on an Xbox.
        /// </summary>
        /// <param name="directory">The directory to create.</param>
        /// <param name="console">The Xbox console to create the directory on.</param>
        public static void Create(XboxPath directory, XboxConsole console)
        {
            new XboxDirectoryInfo(directory, console).Create();
        }

        /// <summary>
        /// Deletes an empty directory from an Xbox.
        /// </summary>
        /// <param name="directory">The directory to delete on the Xbox.</param>
        /// <param name="console">The Xbox that contains the directory.</param>
        public static void Delete(XboxPath directory, XboxConsole console)
        {
            new XboxDirectoryInfo(directory, console).Delete();
        }

        /// <summary>
        /// Deletes a directory from an Xbox.
        /// </summary>
        /// <param name="directory">The directory to delete on the Xbox.</param>
        /// <param name="recursive">Whether or not to delete the contents inside the directory.</param>
        /// <param name="console">The Xbox that contains the directory.</param>
        public static void Delete(XboxPath directory, bool recursive, XboxConsole console)
        {
            new XboxDirectoryInfo(directory, console).Delete(recursive);
        }

        /// <summary>
        /// Investigates whether a directory exists on an Xbox.
        /// </summary>
        /// <param name="directory">The directory to look for.</param>
        /// <param name="console">The Xbox Console to look on.</param>
        /// <returns>A value indicating whether the directory exists on the console or not.</returns>
        public static bool Exists(XboxPath directory, XboxConsole console)
        {
            if (console == null)
            {
                throw new ArgumentNullException("console");
            }

            return XboxDirectoryInfo.ExistsImpl(console.SystemIpAddressAndSessionKeyCombined, directory, console.Adapter);
        }

        /// <summary>
        /// Moves a directory from a PC to an Xbox console.
        /// </summary>
        /// <param name="sourceDirectory">The directory to move.</param>
        /// <param name="destinationDirectory">The directory to move to.</param>
        /// <param name="console">The console to move the directory to.</param>
        public static void Move(string sourceDirectory, XboxPath destinationDirectory, XboxConsole console)
        {
            Copy(sourceDirectory, destinationDirectory, console);
            new DirectoryInfo(sourceDirectory).Delete(true);
        }

        /// <summary>
        /// Moves a directory from an Xbox console to a PC.
        /// </summary>
        /// <param name="sourceDirectory">The directory to move.</param>
        /// <param name="destinationDirectory">The directory to move to.</param>
        /// <param name="console">The console to move the directory from.</param>
        public static void Move(XboxPath sourceDirectory, string destinationDirectory, XboxConsole console)
        {
            Copy(sourceDirectory, destinationDirectory, console);
            new XboxDirectoryInfo(sourceDirectory, console).Delete(true);
        }
    }
}
