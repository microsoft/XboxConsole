//------------------------------------------------------------------------------
// <copyright file="XboxPath.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.IO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using SystemIoPath = System.IO.Path;

    /// <summary>
    /// Uniquely identifies a file or directory on an Xbox by combining its path with the 
    /// operating system on which the file or directory resides.
    /// </summary>
    [DebuggerDisplay("{FullName}, {OperatingSystem}")]
    public class XboxPath
    {
        // Number comes from Windows and is the known maximum path length
        // on an NTFS file system without using the long path prefix.
        internal const uint MaxPathLength = 260;

        /// <summary>
        /// Initializes a new instance of the XboxPath class.
        /// </summary>
        /// <param name="path">The complete path to the file or directory on the Xbox.</param>
        /// <param name="operatingSystem">The Xbox operating system on which the file resides.</param>
        public XboxPath(string path, XboxOperatingSystem operatingSystem)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException("path");
            }

            this.FullName = path;
            this.OperatingSystem = operatingSystem;
        }

        /// <summary>
        /// Gets a value indicating whether or not the given path is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return XboxPath.IsValidPath(this.FullName);
            }
        }

        /// <summary>
        /// Gets or sets the complete path to the file or directory on the Xbox.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the Xbox operating system on which the file resides.
        /// </summary>
        public XboxOperatingSystem OperatingSystem { get; set; }

        /// <summary>
        /// Determines whether a given path is a path to an xbox console or not.
        /// </summary>
        /// <param name="path">The  file or directory path to evaluate.</param>
        /// <returns>A value indicating whether the path is pointing to an xbox console or not.</returns>
        public static bool HasXboxOrigin(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException("path");
            }

            return Regex.IsMatch(path, @"^(\{.+\}|x[a-z]):\\?", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Returns the directory information for the specified path string.
        /// </summary>
        /// <param name="path">The path of a file or directory.</param>
        /// <returns>
        /// A System.String containing directory information for path, or null if path
        /// denotes a root directory, is the empty string (""), or is null. Returns System.String.Empty
        /// if path does not contain directory information.
        /// </returns>
        public static string GetDirectoryName(string path)
        {
            string directoryName = SystemIoPath.GetDirectoryName(path);

            if (!string.IsNullOrWhiteSpace(directoryName))
            {
                if (directoryName[directoryName.Length - 1] == SystemIoPath.VolumeSeparatorChar)
                {
                    return directoryName + SystemIoPath.DirectorySeparatorChar;
                }

                return directoryName;
            }

            return null;
        }

        /// <summary>
        /// Combines two XboxPath objects.
        /// </summary>
        /// <param name="path1">The first path.</param>
        /// <param name="path2">The second path.</param>
        /// <returns> An XboxPath containing the combined paths. If path2 contains an absolute path, this method returns path2.</returns>
        public static XboxPath Combine(XboxPath path1, XboxPath path2)
        {
            if (path1 == null)
            {
                throw new ArgumentNullException("path1");
            }

            if (path2 == null)
            {
                throw new ArgumentNullException("path2");
            }

            if (path1.OperatingSystem != path2.OperatingSystem)
            {
                throw new ArgumentException("Both paths must target the same operating system.");
            }

            string resultPath = XboxPath.Combine(path1.FullName, path2.FullName);
            return new XboxPath(resultPath, path1.OperatingSystem);
        }

        /// <summary>
        /// Combines two path strings.
        /// </summary>
        /// <param name="path1">The first path.</param>
        /// <param name="path2">The second path.</param>
        /// <returns> A string containing the combined paths. If path2 contains an absolute path, this method returns path2.</returns>
        public static string Combine(string path1, string path2)
        {
            if (string.IsNullOrWhiteSpace(path1))
            {
                throw new ArgumentNullException("path1");
            }

            if (string.IsNullOrWhiteSpace(path2))
            {
                throw new ArgumentNullException("path2");
            }

            if (!IsValidPath(path1))
            {
                throw new ArgumentException("path1 contains invalid characters.");
            }

            if (!HasXboxOrigin(path1))
            {
                throw new ArgumentException("path1 must be an Xbox path.");
            }

            if (IsRoot(path1) && path1[path1.Length - 1] != SystemIoPath.DirectorySeparatorChar)
            {
                path1 += SystemIoPath.DirectorySeparatorChar;
            }

            if (!IsValidPath(path2))
            {
                throw new ArgumentException("path2 contains invalid characters.");
            }

            if (HasXboxOrigin(path2))
            {
                return path2;
            }

            return SystemIoPath.Combine(path1, path2);
        }

        /// <summary>
        /// Determines whether the specified path is the root of the drive.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns>A value indicating whether the path is the drive root or not.</returns>
        public static bool IsRoot(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            if (!IsValidPath(path))
            {
                throw new ArgumentException("The path contains invalid characters.");
            }

            if (!HasXboxOrigin(path))
            {
                throw new ArgumentException("The path is not a valid Xbox path.");
            }

            string[] splitPath = path.Split('\\');

            return (splitPath.Length == 1) || ((splitPath.Length == 2) && string.IsNullOrEmpty(splitPath[1]));
        }

        /// <summary>
        /// Determines whether a given path is rooted, meaning that it contains a complete path
        /// starting from a drive letter.  Example, "xd:\parentDirectory" is rooted.
        /// A relative path like "..\someDirectory" is not rooted.
        /// </summary>
        /// <param name="path">The path to be examined.</param>
        /// <returns>True if the path is rooted.</returns>
        [Obsolete("This method has been replaced by the \"HasXboxOrigin\" method.")]
        public static bool IsPathRooted(string path)
        {
            return HasXboxOrigin(path);
        }

        /// <summary>
        /// Determines whether the specified xbox path is valid.
        /// </summary>
        /// <param name="path">The xbox path.</param>
        /// <returns>
        ///     <c>True</c> if the specified path is valid; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsValidPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            if (path.IndexOfAny(SystemIoPath.GetInvalidPathChars()) >= 0)
            {
                return false;
            }

            if (path.Length > MaxPathLength)
            {
                return false;
            }

            if (path.Contains(SystemIoPath.VolumeSeparatorChar))
            {
                return HasXboxOrigin(path);
            }

            return true;
        }
    }
}
