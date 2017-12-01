//------------------------------------------------------------------------------
// <copyright file="FileInfoExtensionsTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Tests
{
    using System;
    using System.IO;
    using System.IO.Fakes;
    using System.Linq;
    using System.Net;
    using Microsoft.Internal.GamesTest.Xbox.Fakes;
    using Microsoft.Internal.GamesTest.Xbox.IO;
    using Microsoft.QualityTools.Testing.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// A class to house tests for the FileInfo extension methods.
    /// </summary>
    [TestClass]
    public class FileInfoExtensionsTests
    {
        private const string FileInfoExtensionsTestCategory = "XboxConsole.FileInfoExtensions";

        private const string ConsoleAddress = "10.124.151.244"; // The actual IP address used here is irrelevant.

        private XboxPath xboxPath = new XboxPath(@"xd:\directory", XboxOperatingSystem.System);

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
        /// Called once before each test to setup shim and stub objects.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The StubConsoleAdapterBase doesn't really need to be disposed.")]
        [TestInitialize]
        public void TestInitialize()
        {
            this.shimsContext = ShimsContext.Create();

            ShimXboxConsoleAdapterBase.ConstructorXboxXdkBase = (@base, xboxXdk) => { };

            this.shimAdapter = new ShimXboxConsoleAdapterBase(new StubXboxConsoleAdapterBase(null))
            {
                SendDirectoryStringStringXboxPathBooleanIProgressOfXboxFileTransferMetric = (systemIpAddress, sourceDirectory, destinationDirectory, isRecursive, metrics) => { },
                SendFileStringStringXboxPathIProgressOfXboxFileTransferMetric = (systemIpAddress, sourceFile, destinationFile, metrics) => { },
                ReceiveDirectoryStringXboxPathStringBooleanIProgressOfXboxFileTransferMetric = (systemIpAddress, sourceDirectory, destinationDirectory, isRecursive, metrics) => { },
                ReceiveFileStringXboxPathStringIProgressOfXboxFileTransferMetric = (systemIpAddress, sourceFilePath, destinationFilePath, metrics) => { },
                CreateDirectoryStringXboxPath = (systemIpAddress, path) => { },
            };

            this.shimXboxConsole = new ShimXboxConsole
            {
                AdapterGet = () => this.shimAdapter,
                SystemIpAddressGet = () => IPAddress.Parse(FileInfoExtensionsTests.ConsoleAddress),
                SystemIpAddressAndSessionKeyCombinedGet = () => FileInfoExtensionsTests.ConsoleAddress,
            };
        }

        /// <summary>
        /// Called once after each test to clean up shim and stub objects.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            this.shimsContext.Dispose();
        }

        /// <summary>
        /// Verifies that the CopyTo method throws if given a null value for the fileInfo parameter.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(FileInfoExtensionsTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestArgumentNullFileInfo()
        {
            FileInfo fileInfo = null;
            fileInfo.CopyTo(this.xboxPath, this.XboxConsole);
        }

        /// <summary>
        /// Verifies that the CopyTo method throws if given a null value for the console parameter.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(FileInfoExtensionsTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestArgumentNullConsole()
        {
            FileInfo fileInfo = new FileInfo(@"c:\test");
            fileInfo.CopyTo(this.xboxPath, null);
        }

        /// <summary>
        /// Verifies that the CopyTo method throws if given a null value for the xboxPath parameter.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(FileInfoExtensionsTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestArgumentNullXboxPath()
        {
            FileInfo fileInfo = new FileInfo(@"c:\test");
            fileInfo.CopyTo(null, this.XboxConsole);
        }

        /// <summary>
        /// Verifies that the CopyTo method throws if given a file that doesn't exist.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(FileInfoExtensionsTestCategory)]
        [ExpectedException(typeof(FileNotFoundException))]
        public void TestFileNotFound()
        {
            FileInfo fileInfo = new FileInfo(@"c:\test\fileThatDoesntExist.txt");
            fileInfo.CopyTo(this.xboxPath, this.XboxConsole);
        }

        /// <summary>
        /// Verifies that the CopyTo method corretly invokes the adapter.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(FileInfoExtensionsTestCategory)]
        public void TestCallsAdapterSendFileCorrectly()
        {
            ShimFileInfo.AllInstances.ExistsGet = info => true;
            FileInfo localFile = new FileInfo(@"c:\test\foo.txt");

            IProgress<XboxFileTransferMetric> expectedProgress = null;

            bool success = false;
            this.shimAdapter.SendFileStringStringXboxPathIProgressOfXboxFileTransferMetric = (systemIpAddress, localFilePath, destinationPath, metrics) =>
                {
                    success = true;
                    Assert.AreEqual(localFile.FullName, localFilePath);
                    Assert.AreEqual(this.xboxPath.FullName, destinationPath.FullName);
                    Assert.AreEqual(this.xboxPath.OperatingSystem, destinationPath.OperatingSystem);
                    Assert.AreSame(expectedProgress, metrics, "CopyTo didn't pass on progress object to adapter.");
                };

            var returnedValue = localFile.CopyTo(this.xboxPath, this.XboxConsole);

            Assert.IsTrue(success);
            Assert.AreSame(this.XboxConsole, returnedValue.Console);
            Assert.AreEqual(this.xboxPath.FullName, returnedValue.FullName);
            Assert.AreEqual(this.xboxPath.OperatingSystem, returnedValue.OperatingSystem);

            localFile.CopyTo(this.xboxPath, this.XboxConsole, null);
            expectedProgress = new Progress<XboxFileTransferMetric>();
            localFile.CopyTo(this.xboxPath, this.XboxConsole, expectedProgress);
        }
    }
}
