//------------------------------------------------------------------------------
// <copyright file="XboxFileInfo.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.IO
{
    using System;
    using System.IO;
    using Microsoft.Internal.GamesTest.Xbox.Telemetry;

    /// <summary>
    /// Represents an Xbox file.
    /// </summary>
    public class XboxFileInfo : XboxFileSystemInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XboxFileInfo"/> class.
        /// </summary>
        /// <param name="path">The path to the file on the Xbox.</param>
        /// <param name="operatingSystem">The operating system on the Xbox where the file resides.</param>
        /// <param name="console">The console on which the file resides.</param>
        public XboxFileInfo(string path, XboxOperatingSystem operatingSystem, XboxConsole console)
            : base(path, operatingSystem, console)
        {
            XboxConsoleEventSource.Logger.ObjectCreated(XboxConsoleEventSource.GetCurrentConstructor());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxFileInfo"/> class.
        /// </summary>
        /// <param name="xboxPath">The Xbox path to the file on the Xbox.</param>
        /// <param name="console">The console on which the directory resides.</param>
        public XboxFileInfo(XboxPath xboxPath, XboxConsole console)            
            : base(xboxPath, console)
        {
            XboxConsoleEventSource.Logger.ObjectCreated(XboxConsoleEventSource.GetCurrentConstructor());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxFileInfo"/> class.
        /// </summary>
        /// <param name="definition">The definition object describing this file.</param>
        /// <param name="console">The console on which the file resides.</param>
        internal XboxFileInfo(XboxFileSystemInfoDefinition definition, XboxConsole console)
            : base(definition, console)
        {
            if (definition.FileAttributes.HasFlag(FileAttributes.Directory))
            {
                throw new ArgumentException("Cannot pass a directory to the XboxFileInfo constructor", "definition");
            }
        }

        /// <summary>
        /// Gets the parent directory.
        /// </summary>
        /// <value>The parent directory.</value>
        public XboxDirectoryInfo Directory
        {
            get
            {
                XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

                string directoryName = this.DirectoryName;

                if (directoryName == null)
                {
                    return null;
                }

                return new XboxDirectoryInfo(directoryName, this.XboxPath.OperatingSystem, this.Console);
            }
        }

        /// <summary>
        /// Gets the name of the parent directory.
        /// </summary>
        /// <value>The name of the parent directory.</value>
        public string DirectoryName
        {
            get
            {
                return XboxPath.GetDirectoryName(this.FullName);
            }
        }

        /// <summary>
        /// Gets the file extension.
        /// </summary>
        /// <value>The file extension.</value>
        public string Extension
        {
            get
            {
                return Path.GetExtension(this.FullName);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="XboxFileInfo"/> exists.
        /// </summary>
        /// <value><c>True</c> if exists; otherwise, <c>false</c>.</value>
        public override bool Exists
        {
            get
            {
                XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

                return ExistsImpl(this.Console.SystemIpAddressAndSessionKeyCombined, this.XboxPath, this.Console.Adapter);
            }
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        public override void Delete()
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            if (this.Exists)
            {
                this.Console.Adapter.DeleteFile(this.Console.SystemIpAddressAndSessionKeyCombined, this.XboxPath);
            }

            this.Refresh();
        }

        /// <summary>
        /// Copies this instance to a path on the local machine.
        /// </summary>
        /// <param name="localPath">The path on the local machine to copy to.</param>
        public override void Copy(string localPath)
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            this.Copy(localPath, null);
        }

        /// <summary>
        /// Copies this instance to a path on the local machine.
        /// </summary>
        /// <param name="localPath">The path on the local machine to copy to.</param>
        /// <param name="metrics">The progress handler that the calling app uses to receive progress updates about metrics. This may be null.</param>
        public void Copy(string localPath, IProgress<XboxFileTransferMetric> metrics)
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            this.Console.Adapter.ReceiveFile(this.Console.SystemIpAddressAndSessionKeyCombined, this.XboxPath, localPath, metrics);
        }

        /// <summary>
        /// Determines whether a file exists on an Xbox.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="path">The file to look for.</param>
        /// <param name="consoleAdapter">The Xbox console to search on.</param>
        /// <returns>A value indicating whether the file exists on the console or not.</returns>
        internal static bool ExistsImpl(string systemIpAddress, XboxPath path, XboxConsoleAdapterBase consoleAdapter)
        {
            return XboxFileSystemInfo.ExistsImpl(systemIpAddress, path, xboxFileSystemInfoDefinition => !xboxFileSystemInfoDefinition.IsDirectory, consoleAdapter);
        }
    }
}
