//------------------------------------------------------------------------------
// <copyright file="XboxFileSystemInfo.cs" company="Microsoft">
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
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a base Xbox FileSystem object.
    /// </summary>
    public abstract class XboxFileSystemInfo : XboxItem
    {
        private readonly object definitionLock = new object();
        private XboxFileSystemInfoDefinition definition;

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxFileSystemInfo"/> class.
        /// </summary>
        /// <param name="definition">The definition of this file sytsem object.</param>
        /// <param name="console">The console on which this file system object resides.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if the <paramref name="definition"/> or <paramref name="console"/> parameter is null.</exception>
        internal XboxFileSystemInfo(XboxFileSystemInfoDefinition definition, XboxConsole console)
            : base(console)
        {
            if (definition == null)
            {
                throw new ArgumentNullException("definition");
            }

            this.XboxPath = definition.Path;
            this.definition = definition;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxFileSystemInfo"/> class.
        /// </summary>
        /// <param name="path">The path to the file system object.</param>
        /// <param name="operatingSystem">The operating system on which the file system object resides.</param>
        /// <param name="console">The console on which the file system object resides.</param>
        /// <exception cref="System.ArgumentException">Thrown if given an invalid path.</exception>
        protected XboxFileSystemInfo(string path, XboxOperatingSystem operatingSystem, XboxConsole console)
            : base(console)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException("path");    
            }

            if (!XboxPath.IsValidPath(path))
            {
                throw new ArgumentException("Invalid path.", "path");
            }

            this.XboxPath = new XboxPath(path, operatingSystem);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxFileSystemInfo"/> class.
        /// </summary>
        /// <param name="xboxPath">The Xbox path to the file system object.</param>
        /// <param name="console">The console on which the file system object resides.</param>
        /// <exception cref="System.ArgumentException">Thrown if given an invalid path.</exception>
        protected XboxFileSystemInfo(XboxPath xboxPath, XboxConsole console)
            : base(console)
        {
            if (xboxPath == null)
            {
                throw new ArgumentNullException("xboxPath");
            }

            if (!xboxPath.IsValid)
            {
                throw new ArgumentException("Invalid Xbox path.", "xboxPath");
            }

            this.XboxPath = xboxPath;
        }

        /// <summary>
        /// Gets the full name of the Xbox file.
        /// </summary>
        public string FullName
        {
            get
            {
                return this.XboxPath.FullName;
            }
        }

        /// <summary>
        /// Gets the operating system on which this file resides.
        /// </summary>
        public XboxOperatingSystem OperatingSystem
        {
            get
            {
                return this.XboxPath.OperatingSystem;
            }
        }

        /// <summary>
        /// Gets the creation time.
        /// </summary>
        public DateTime CreationTime
        {
            get
            {
                return this.Definition.CreationTime;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="XboxFileSystemInfo"/> exists.
        /// </summary>
        /// <value><c>True</c> if exists; otherwise, <c>false</c>.</value>
        public virtual bool Exists
        {
            get
            {
                return ExistsImpl(this.Console.SystemIpAddressAndSessionKeyCombined, this.XboxPath, null, this.Console.Adapter);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is a directory.
        /// </summary>
        /// <value>
        ///     <c>True</c> if this instance is a directory; otherwise, <c>false</c>.
        /// </value>
        public bool IsDirectory
        {
            get
            {
                return this.Definition.IsDirectory;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        ///     <c>True</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        public bool IsReadOnly
        {
            get
            {
                return this.Definition.FileAttributes.HasFlag(FileAttributes.ReadOnly);
            }
        }

        /// <summary>
        /// Gets the change time.
        /// </summary>
        /// <value>The change time.</value>
        public DateTime LastWriteTime
        {
            get
            {
                return this.Definition.LastWriteTime;
            }
        }

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>The length.</value>
        public long Length
        {
            get
            {
                return (long)this.Definition.FileSize;
            }
        }

        /// <summary>
        /// Gets the file system object name.
        /// </summary>
        /// <value>The file system object name.</value>
        public string Name
        {
            get
            {
                string fullPath = this.FullName;

                if (fullPath[fullPath.Length - 1] == Path.DirectorySeparatorChar)
                {
                    fullPath = fullPath.Substring(0, fullPath.Length - 1);
                }

                string fileName = Path.GetFileName(fullPath);

                if (string.IsNullOrEmpty(fileName))
                {
                    return this.FullName;
                }

                return fileName;
            }
        }

        /// <summary>
        /// Gets the defintion object for this file.
        /// </summary>
        internal XboxFileSystemInfoDefinition Definition
        {
            get
            {
                if (this.definition == null)
                {
                    lock (this.definitionLock)
                    {
                        if (this.definition == null)
                        {
                            XboxFileSystemInfoDefinition newDefinition = this.Console.Adapter.GetFileSystemInfoDefinition(this.Console.SystemIpAddressAndSessionKeyCombined, this.XboxPath);
                            Thread.MemoryBarrier();
                            this.definition = newDefinition;
                            this.XboxPath = this.definition.Path;
                        }
                    }
                }

                return this.definition;
            }
        }

        /// <summary>
        /// Gets the path for this file system object.
        /// </summary>
        protected XboxPath XboxPath { get; private set; }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        public abstract void Delete();

        /// <summary>
        /// Copies this instance to a path on the local machine.
        /// </summary>
        /// <param name="localPath">The path on the local machine to copy to.</param>
        public abstract void Copy(string localPath);

        /// <summary>
        /// Refreshes this instance.
        /// </summary>
        public void Refresh()
        {
            lock (this.definitionLock)
            {
                try
                {
                    this.definition = this.Console.Adapter.GetFileSystemInfoDefinition(this.Console.SystemIpAddressAndSessionKeyCombined, this.XboxPath);
                }
                catch (FileNotFoundException)
                {
                    this.definition = null;
                }
            }
        }

        /// <summary>
        /// Moves a file between an xbox console and a PC.
        /// </summary>
        /// <param name="destinationPath">The path to move to.</param>
        public virtual void MoveTo(string destinationPath)
        {
            this.Copy(destinationPath);
            this.Delete();
        }

        /// <summary>
        /// Determines whether a file or directory exists on an Xbox.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="path">The file or directory to look for.</param>
        /// <param name="additionalExistsPredicate">The optional additional logic to determine existence based on XboxFileSystemInfoDefinition.</param>
        /// <param name="consoleAdapter">The Xbox console to search on.</param>
        /// <returns>A value indicating whether the file or directory exists on the console or not.</returns>
        internal static bool ExistsImpl(string systemIpAddress, XboxPath path, Func<XboxFileSystemInfoDefinition, bool> additionalExistsPredicate, XboxConsoleAdapterBase consoleAdapter)
        {
            XboxFileSystemInfoDefinition definition;
            try
            {
                definition = consoleAdapter.GetFileSystemInfoDefinition(systemIpAddress, path);
            }
            catch (FileNotFoundException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                // Following the same approach as in .NET, returning false without throwing an exception in case of insufficient permissions:
                // http://msdn.microsoft.com/en-us/library/system.io.file.exists.aspx
                return false;
            }

            if (additionalExistsPredicate != null)
            {
                return additionalExistsPredicate(definition);
            }

            return true;
        }
    }
}
