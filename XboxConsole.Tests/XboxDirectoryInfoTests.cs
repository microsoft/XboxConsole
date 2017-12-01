//------------------------------------------------------------------------------
// <copyright file="XboxDirectoryInfoTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Fakes;
    using System.Linq;
    using System.Net;
    using Microsoft.Internal.GamesTest.Xbox.Fakes;
    using Microsoft.Internal.GamesTest.Xbox.IO;
    using Microsoft.Internal.GamesTest.Xbox.IO.Fakes;
    using Microsoft.QualityTools.Testing.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Class to house tests for the XboxDirectoryInfo class.
    /// </summary>
    [TestClass]
    public class XboxDirectoryInfoTests
    {
        private const string XboxDirectoryInfoTestCategory = "XboxConsole.XboxDirectoryInfo";

        private const string ConsoleAddress = "10.124.151.244"; // The actual IP address used here is irrelevant.

        private XboxPath xboxDirectoryPath = new XboxPath(@"xd:\TestDirectory", XboxOperatingSystem.System);

        private IDisposable shimsContext;

        private ShimXboxConsole shimXboxConsole;

        private ShimXboxConsoleAdapterBase shimAdapter;

        private XboxConsole XboxConsole
        {
            get
            {
                return this.shimXboxConsole.Instance;
            }
        }

        /// <summary>
        /// Initializes this class before each test is executed.
        /// </summary>
        [TestInitialize]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The StubXboxConsolAdapterBase object is not important.")]
        public void TestInitialize()
        {
            this.shimsContext = ShimsContext.Create();
            ShimXboxConsoleAdapterBase.ConstructorXboxXdkBase = (@base, xboxXdk) => { };

            this.shimAdapter = new ShimXboxConsoleAdapterBase(new StubXboxConsoleAdapterBase(null))
                {
                    SendDirectoryStringStringXboxPathBooleanIProgressOfXboxFileTransferMetric = (systemIpAddress, sourceDirectory, destinationDirectory, isRecursive, metrics) => { },
                    SendFileStringStringXboxPathIProgressOfXboxFileTransferMetric = (systemIpAddress, sourceFile, destinationFile, metrics) => { },
                    ReceiveDirectoryStringXboxPathStringBooleanIProgressOfXboxFileTransferMetric = (systemIpAddress, sourceDiretory, destinationDirectory, isRecursive, metrics) => { },
                    ReceiveFileStringXboxPathStringIProgressOfXboxFileTransferMetric = (systemIpAddress, sourceFilePath, destinationFilePath, metrics) => { },
                    CreateDirectoryStringXboxPath = (systemIpAddress, path) => { },
                };

            this.shimXboxConsole = new ShimXboxConsole
                {
                    AdapterGet = () => this.shimAdapter,
                    SystemIpAddressGet = () => IPAddress.Parse(XboxDirectoryInfoTests.ConsoleAddress),
                    SystemIpAddressAndSessionKeyCombinedGet = () => XboxDirectoryInfoTests.ConsoleAddress,
                };
        }

        /// <summary>
        /// Cleans up after each test is executed.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            this.shimsContext.Dispose();
        }

        /// <summary>
        /// Verifies that an XboxDirectoryInfo can be created with the proper input parameters.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxDirectoryInfoTestCategory)]
        public void XboxDirectoryInfoConstructorTest()
        {
            var directoryInfo = new XboxDirectoryInfo(this.xboxDirectoryPath.FullName, this.xboxDirectoryPath.OperatingSystem, this.XboxConsole);

            Assert.IsNotNull(directoryInfo);
            Assert.AreEqual(this.xboxDirectoryPath.FullName, directoryInfo.FullName);
            Assert.AreEqual(this.xboxDirectoryPath.OperatingSystem, directoryInfo.OperatingSystem);
        }

        /// <summary>
        /// Verifies that an XboxDirectoryInfo can be created with the proper input parameters (XboxPath).
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxDirectoryInfoTestCategory)]
        public void XboxDirectoryInfoConstructorXboxPathTest()
        {
            var directoryInfo = new XboxDirectoryInfo(this.xboxDirectoryPath, this.XboxConsole);

            Assert.IsNotNull(directoryInfo);
            Assert.AreEqual(this.xboxDirectoryPath.FullName, directoryInfo.FullName);
            Assert.AreEqual(this.xboxDirectoryPath.OperatingSystem, directoryInfo.OperatingSystem);
        }

        /// <summary>
        /// Verifies that an XboxDirectoryInfo can be created with a valid XboxFileSystemInfoDefintion and XboxConsole as its parameters.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxDirectoryInfoTestCategory)]
        public void TestXboxDirectoryInfoInternalConstructor()
        {
            XboxFileSystemInfoDefinition fileSystemInfo = new XboxFileSystemInfoDefinition(0, FileAttributes.Directory, this.xboxDirectoryPath.FullName, this.xboxDirectoryPath.OperatingSystem, 0, 0, 0);
            var directoryInfo = new XboxDirectoryInfo(fileSystemInfo, this.XboxConsole);

            Assert.IsNotNull(directoryInfo);
            Assert.AreEqual(this.xboxDirectoryPath.FullName, directoryInfo.FullName);
            Assert.AreEqual(this.xboxDirectoryPath.OperatingSystem, directoryInfo.OperatingSystem);
        }

        /// <summary>
        /// Verifies that the Parent property returns the correct value.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxDirectoryInfoTestCategory)]
        public void TestXboxDirectoryInfoGetParent()
        {
            ShimXboxFileSystemInfoDefinition xboxFileShim = new ShimXboxFileSystemInfoDefinition()
                {
                    PathGet = () => new XboxPath(@"xd:\parentDirectory\directory", XboxOperatingSystem.System)
                };

            var directoryInfo = new XboxDirectoryInfo(xboxFileShim, this.XboxConsole);
            Assert.AreEqual(@"xd:\parentDirectory", directoryInfo.Parent.FullName);
        }

        /// <summary>
        /// Verifies that an XboxDirectoryInfo returns a null parent if it points to the drive root.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxDirectoryInfoTestCategory)]
        public void TestXboxDirectoryInfoGetParentRoot()
        {
            ShimXboxFileSystemInfoDefinition xboxFileShim = new ShimXboxFileSystemInfoDefinition()
                {
                    PathGet = () => new XboxPath(@"xd:\", XboxOperatingSystem.System)
                };

            var directoryInfo = new XboxDirectoryInfo(xboxFileShim, this.XboxConsole);
            Assert.IsNull(directoryInfo.Parent);
        }

        /// <summary>
        /// Verifies that the Parent property correctly handles paths that end with a trailing slash.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxDirectoryInfoTestCategory)]
        public void TestXboxDirectoryInfoGetParentTrailingSlash()
        {
            ShimXboxFileSystemInfoDefinition xboxFileShim = new ShimXboxFileSystemInfoDefinition()
            {
                PathGet = () => new XboxPath(@"xd:\parentDirectory\directory\", XboxOperatingSystem.System)
            };

            var directoryInfo = new XboxDirectoryInfo(xboxFileShim, this.XboxConsole);
            Assert.AreEqual(@"xd:\parentDirectory", directoryInfo.Parent.FullName);
        }

        /// <summary>
        /// Verifies that the default Delete method does a non-recursive delete.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxDirectoryInfoTestCategory)]
        public void TestXboxDirectoryInfoDeleteIsNonRecursive()
        {
            bool success = false;
            this.shimAdapter.DeleteDirectoryStringXboxPathBoolean = (systemIpAddress, xboxDirectory, isRecursive) =>
                {
                    success = isRecursive == false;
                };

            ShimXboxFileSystemInfoDefinition xboxFileShim = new ShimXboxFileSystemInfoDefinition()
            {
                PathGet = () => new XboxPath(@"xd:\directory", XboxOperatingSystem.System)
            };
            ShimXboxFileSystemInfo.ExistsImplStringXboxPathFuncOfXboxFileSystemInfoDefinitionBooleanXboxConsoleAdapterBase = (address, xboxPath, existsPredicate, adapter) => true;

            ShimXboxFileSystemInfo.AllInstances.ExistsGet = info => true;
            XboxDirectoryInfo directoryInfo = new XboxDirectoryInfo(xboxFileShim, this.XboxConsole);
            directoryInfo.Delete();
            Assert.IsTrue(success);
        }

        /// <summary>
        /// Verifies that an exception is thrown if the user tries to delete a root directory.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxDirectoryInfoTestCategory)]
        [ExpectedException(typeof(IOException))]
        public void TestXboxDirectoryInfoDeleteThrowsRootDirectory()
        {
            ShimXboxFileSystemInfoDefinition xboxFileShim = new ShimXboxFileSystemInfoDefinition()
            {
                PathGet = () => new XboxPath(@"xd:\", XboxOperatingSystem.System)
            };

            XboxDirectoryInfo directoryInfo = new XboxDirectoryInfo(xboxFileShim, this.XboxConsole);
            directoryInfo.Delete();
        }

        /// <summary>
        /// Verifies that Delete calls refresh after deleting itself.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxDirectoryInfoTestCategory)]
        public void TestXboxDirectoryInfoDeleteCallsRefresh()
        {
            bool calledDeleteDirectory = false;
            this.shimAdapter.DeleteDirectoryStringXboxPathBoolean = (systemIpAddress, path, isRecursive) =>
            {
                calledDeleteDirectory = true;
            };

            bool success = false;
            ShimXboxFileSystemInfo.AllInstances.Refresh = info =>
            {
                success = true;
                Assert.IsTrue(calledDeleteDirectory);
            };

            ShimXboxFileSystemInfoDefinition xboxFileShim = new ShimXboxFileSystemInfoDefinition()
            {
                PathGet = () => new XboxPath(@"xd:\directory", XboxOperatingSystem.System)
            };
            ShimXboxFileSystemInfo.ExistsImplStringXboxPathFuncOfXboxFileSystemInfoDefinitionBooleanXboxConsoleAdapterBase = (address, xboxPath, existsPredicate, adapter) => true;

            XboxDirectoryInfo directoryInfo = new XboxDirectoryInfo(xboxFileShim, this.XboxConsole);
            directoryInfo.Delete();
            Assert.IsTrue(success);
        }

        /// <summary>
        /// Verifies that Delete(bool) calls refresh after deleting itself.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxDirectoryInfoTestCategory)]
        public void TestXboxDirectoryInfoDeleteBooleanCallsRefresh()
        {
            bool calledDeleteDirectory = false;
            this.shimAdapter.DeleteDirectoryStringXboxPathBoolean = (systemIpAddress, path, isRecursive) =>
            {
                calledDeleteDirectory = true;
            };

            bool success = false;
            ShimXboxFileSystemInfo.AllInstances.Refresh = info =>
            {
                success = true;
                Assert.IsTrue(calledDeleteDirectory);
            };

            ShimXboxFileSystemInfoDefinition xboxFileShim = new ShimXboxFileSystemInfoDefinition()
            {
                PathGet = () => new XboxPath(@"xd:\directory", XboxOperatingSystem.System)
            };
            ShimXboxFileSystemInfo.ExistsImplStringXboxPathFuncOfXboxFileSystemInfoDefinitionBooleanXboxConsoleAdapterBase = (address, xboxPath, existsPredicate, adapter) => true;

            XboxDirectoryInfo directoryInfo = new XboxDirectoryInfo(xboxFileShim, this.XboxConsole);
            directoryInfo.Delete(false);
            Assert.IsTrue(success);
        }

        /// <summary>
        /// Verifies that an XboxDirectoryInfo returns itself as the root when it points to an xbox drive root.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxDirectoryInfoTestCategory)]
        public void TestXboxDirectoryInfoGetRootIsRoot()
        {
            ShimXboxFileSystemInfoDefinition xboxFileShim = new ShimXboxFileSystemInfoDefinition()
            {
                PathGet = () => new XboxPath(@"xd:\", XboxOperatingSystem.System)
            };

            XboxDirectoryInfo directoryInfo = new XboxDirectoryInfo(xboxFileShim, this.XboxConsole);
            Assert.AreEqual(directoryInfo.FullName, directoryInfo.Root.FullName);
        }

        /// <summary>
        /// Verifies that an XboxDirectoryInfo returns the root when it points to an xbox drive root.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxDirectoryInfoTestCategory)]
        public void TestXboxDirectoryInfoGetRoot()
        {
            ShimXboxFileSystemInfoDefinition xboxFileShim = new ShimXboxFileSystemInfoDefinition()
            {
                PathGet = () => new XboxPath(@"xd:\parent\child", XboxOperatingSystem.System)
            };

            XboxDirectoryInfo directoryInfo = new XboxDirectoryInfo(xboxFileShim, this.XboxConsole);
            Assert.AreEqual(@"xd:\", directoryInfo.Root.FullName);
        }

        /// <summary>
        /// Verifies that an XboxDirectoryInfo returns the root when it points to an xbox drive root.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxDirectoryInfoTestCategory)]
        public void TestXboxDirectoryInfoGetRootInvalidDirectoryPath()
        {
            ShimXboxFileSystemInfoDefinition xboxFileShim = new ShimXboxFileSystemInfoDefinition()
            {
                PathGet = () => new ShimXboxPath() { FullNameGet = () => string.Empty }
            };

            XboxDirectoryInfo directoryInfo = new XboxDirectoryInfo(xboxFileShim, this.XboxConsole);
            Assert.IsNull(directoryInfo.Root);
        }

        /// <summary>
        /// Verifies that a DirectoryNotFoundException is thrown if a copy call is made on a directory that does not exist.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxDirectoryInfoTestCategory)]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void TestXboxDirectoryInfoCopyDirectoryDoesNotExist()
        {
            XboxDirectoryInfo directoryInfo = new XboxDirectoryInfo(@"xd:\test", XboxOperatingSystem.System, this.XboxConsole);
            ShimXboxDirectoryInfo shimDirectoryInfo = new ShimXboxDirectoryInfo(directoryInfo)
                {
                    ExistsGet = () => false
                };

            directoryInfo.Copy(@"c:\test");
        }

        /// <summary>
        /// Verifies that an ArgumentNullException is thrown if a copy call is made on a null directory.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxDirectoryInfoTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestXboxDirectoryInfoCopyNullDirectory()
        {
            XboxDirectoryInfo directoryInfo = new XboxDirectoryInfo(new ShimXboxFileSystemInfoDefinition(), this.XboxConsole);
            directoryInfo.Copy(null);
        }

        /// <summary>
        /// Verifies that a XboxConsoleFeatureNotSupportedException is thrown if the user tries to copy an Xbox directory
        /// to another Xbox.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxDirectoryInfoTestCategory)]
        [ExpectedException(typeof(XboxConsoleFeatureNotSupportedException))]
        public void TestXboxDirectoryInfoCopyXboxToXbox()
        {
            ShimXboxDirectoryInfo.AllInstances.ExistsGet = info => true;
            XboxDirectoryInfo directoryInfo = new XboxDirectoryInfo(new ShimXboxFileSystemInfoDefinition(), this.XboxConsole);
            directoryInfo.Copy(@"xd:\directory");
        }

        /// <summary>
        /// Verifies that XboxDirectoryInfo checks to see if the directory exists before
        /// trying to create it.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxDirectoryInfoTestCategory)]
        public void TestXboxDirectoryInfoCreatesChecksExistenceBeforeCreating()
        {
            bool checkedExistence = false;
            ShimXboxDirectoryInfo.AllInstances.ExistsGet = info =>
                {
                    checkedExistence = true;
                    return false;
                };

            this.shimAdapter.CreateDirectoryStringXboxPath = (systemIpAddress, path) =>
                {
                    Assert.IsTrue(checkedExistence);
                };

            XboxDirectoryInfo directoryInfo = new XboxDirectoryInfo(new ShimXboxFileSystemInfoDefinition(), this.XboxConsole);
            directoryInfo.Create();
        }

        /// <summary>
        /// Verifies that the GetDirectories method returns the correct values.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxDirectoryInfoTestCategory)]
        public void TestXboxDirectoryInfoGetDirectories()
        {
            ShimXboxFileSystemInfoDefinition directoryDefinition = new ShimXboxFileSystemInfoDefinition()
                {
                    FileAttributesGet = () => FileAttributes.Directory
                };

            ShimXboxFileSystemInfoDefinition fileDefinition = new ShimXboxFileSystemInfoDefinition()
            {
                FileAttributesGet = () => FileAttributes.Archive
            };

            ShimXboxDirectoryInfo.AllInstances.GetFileSystemInfos = _ =>
                {
                    return new XboxFileSystemInfo[]
                        {
                            new XboxDirectoryInfo(directoryDefinition, this.XboxConsole), 
                            new XboxFileInfo(fileDefinition, this.XboxConsole), 
                        };
                };

            XboxDirectoryInfo directoryInfo = new XboxDirectoryInfo(new ShimXboxFileSystemInfoDefinition(), this.XboxConsole);
            IEnumerable<XboxDirectoryInfo> directories = directoryInfo.GetDirectories();
            Assert.AreEqual(1, directories.Count());
        }

        /// <summary>
        /// Verifies that the GetFiles method returns the correct values.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxDirectoryInfoTestCategory)]
        public void TestXboxDirectoryInfoGetFiles()
        {
            ShimXboxFileSystemInfoDefinition directoryDefinition = new ShimXboxFileSystemInfoDefinition()
            {
                FileAttributesGet = () => FileAttributes.Directory
            };

            ShimXboxFileSystemInfoDefinition fileDefinition = new ShimXboxFileSystemInfoDefinition()
            {
                FileAttributesGet = () => FileAttributes.Archive
            };

            ShimXboxDirectoryInfo.AllInstances.GetFileSystemInfos = _ =>
            {
                return new XboxFileSystemInfo[]
                        {
                            new XboxDirectoryInfo(directoryDefinition, this.XboxConsole), 
                            new XboxFileInfo(fileDefinition, this.XboxConsole), 
                        };
            };

            XboxDirectoryInfo directoryInfo = new XboxDirectoryInfo(new ShimXboxFileSystemInfoDefinition(), this.XboxConsole);
            IEnumerable<XboxFileInfo> files = directoryInfo.GetFiles();
            Assert.AreEqual(1, files.Count());
        }

        /// <summary>
        /// Verifies that the default copy operation performs a recursive copy.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxDirectoryInfoTestCategory)]
        public void TestXboxDirectoryInfoCopyDefaultIsRecursive()
        {
            bool success = false;
            ShimXboxFileSystemInfo.AllInstances.ExistsGet = info => true;
            ShimXboxDirectoryInfo.AllInstances.CopyStringBoolean = (dirInfo, localPath, isRecursive) =>
                {
                    success = true;
                    Assert.IsTrue(isRecursive);
                };

            XboxDirectoryInfo directoryInfo = new XboxDirectoryInfo(new ShimXboxFileSystemInfoDefinition(), this.XboxConsole);
            directoryInfo.Copy(@"C:\test");
            Assert.IsTrue(success);
        }

        /// <summary>
        /// Verifies that a DirectoryNotFoundException is thrown if a copy call is made on a directory that does not exist.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxDirectoryInfoTestCategory)]
        public void TestXboxDirectoryInfoCopyCreatesDirectoryIfItDoesNotExist()
        {
            bool success = false;

            ShimXboxFileSystemInfoDefinition xboxFileShim = new ShimXboxFileSystemInfoDefinition()
            {
                PathGet = () => new XboxPath(@"xd:\directory", XboxOperatingSystem.System)
            };
            ShimDirectory.ExistsString = s => false;
            ShimDirectory.CreateDirectoryString = path =>
                {
                    success = true;
                    return null;
                };

            ShimXboxDirectoryInfo.AllInstances.ExistsGet = info => true;
            XboxDirectoryInfo directoryInfo = new XboxDirectoryInfo(xboxFileShim, this.XboxConsole);
            directoryInfo.Copy(@"c:\directory");

            Assert.IsTrue(success);
        }

        /// <summary>
        /// Verifies that the XboxDirectoryInfo constructor throws an ArgumentNullException if given a null value
        /// for the "path" parameter.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxDirectoryInfoTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestXboxDirectoryInfoThrowsNullPathArgument()
        {
            XboxDirectoryInfo dirInfo = new XboxDirectoryInfo(null, XboxOperatingSystem.System, this.XboxConsole);
            Assert.IsNotNull(dirInfo, "The XboxDirectoryInfo constructor should have thrown an exception.");
        }

        /// <summary>
        /// Verifies that the XboxDirectoryInfo constructor throws an ArgumentNullException if given a null
        /// value for the "console" parameter.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxDirectoryInfoTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestXboxDirectoryInfoThrowsNullConsoleArgument()
        {
            XboxDirectoryInfo dirInfo = new XboxDirectoryInfo(this.xboxDirectoryPath.FullName, XboxOperatingSystem.System, null);
            Assert.IsNotNull(dirInfo, "The XboxDirectoryInfo constructor should have thrown an exception.");
        }

        /// <summary>
        /// Verifies that the XboxDirectoryInfo constructor throws if given a non-Xbox path.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxDirectoryInfoTestCategory)]
        [ExpectedException(typeof(ArgumentException))]
        public void TestXboxDirectoryInfoThrowsInvalidXboxDirectoryArgument()
        {
            XboxDirectoryInfo dirInfo = new XboxDirectoryInfo(@"e:\\FakeDirectory", XboxOperatingSystem.System, this.XboxConsole);
            Assert.IsNotNull(dirInfo, "The XboxDirectoryInfo constructor should have thrown an exception.");
        }

        /// <summary>
        /// Verifies that the XboxDiretoryInfo constructor throws if given a path with invalid characters.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxDirectoryInfoTestCategory)]
        [ExpectedException(typeof(ArgumentException))]
        public void TestXboxDirectoryInfoThrowsWithInvalidCharactersInPath()
        {
            XboxDirectoryInfo dirInfo = new XboxDirectoryInfo(@"xd:\<>FakeDirectory", XboxOperatingSystem.System, this.XboxConsole);
            Assert.IsNotNull(dirInfo, "The XboxDirectoryInfo constructor should have thrown an exception.");
        }
    }
}
