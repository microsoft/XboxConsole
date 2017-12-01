//------------------------------------------------------------------------------
// <copyright file="XboxFileTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Tests
{
    using System;
    using System.IO.Fakes;
    using System.Net;
    using Microsoft.Internal.GamesTest.Xbox.Fakes;
    using Microsoft.Internal.GamesTest.Xbox.IO;
    using Microsoft.Internal.GamesTest.Xbox.IO.Fakes;
    using Microsoft.QualityTools.Testing.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Class to house tests for the XboxFile class.
    /// </summary>
    [TestClass]
    public class XboxFileTests
    {
        private const string XboxFileTestCategory = "XboxConsole.XboxFile";

        private const string ConsoleAddress = "10.124.151.244"; // The actual IP address used here is irrelevant.

        private string pcFile = @"c:\TestFile";
        private XboxPath xboxFilePath = new XboxPath(@"xd:\TestFile", XboxOperatingSystem.System);
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
            this.shimAdapter = new ShimXboxConsoleAdapterBase(new StubXboxConsoleAdapterBase(null));
            this.shimXboxConsole = new ShimXboxConsole
            {
                AdapterGet = () => this.shimAdapter,
                SystemIpAddressGet = () => IPAddress.Parse(XboxFileTests.ConsoleAddress),
                SystemIpAddressAndSessionKeyCombinedGet = () => XboxFileTests.ConsoleAddress,
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
        /// Verifies that the XboxFile.Copy method throws if given a null value for the destinationFile parameter.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestXboxFileCopyPCXboxArgumentNullDestinationFile()
        {
            XboxFile.Copy(this.pcFile, null, this.XboxConsole);
        }

        /// <summary>
        /// Verifies that the XboxFile.Copy method throws if given a value for the destinationFile parameter that doesn't have an Xbox origin.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileTestCategory)]
        [ExpectedException(typeof(XboxConsoleFeatureNotSupportedException))]
        public void TestXboxFileCopyPCXboxNotSupportedDestinationFile()
        {
            ShimXboxPath.HasXboxOriginString = path => false;
            XboxFile.Copy(this.pcFile, this.xboxFilePath, this.XboxConsole);
        }

        /// <summary>
        /// Verifies that the XboxFile.Copy method correctly invokes the FileInfo.CopyTo.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileTestCategory)]
        public void TestXboxFileCopyPCXboxCallsFileInfoCopyToCorrectly()
        {
            IProgress<XboxFileTransferMetric> expectedProgress = null;

            bool success = false;
            ShimXboxPath.HasXboxOriginString = path => true;
            ShimFileInfoExtensions.CopyToFileInfoXboxPathXboxConsoleIProgressOfXboxFileTransferMetric = (fileInfo, xboxPath, console, metrics) => 
                { 
                    success = true; 

                    Assert.AreEqual(this.pcFile, fileInfo.FullName, "Copy didn't pass the expected source file path to the adapter.");
                    Assert.AreEqual(this.xboxFilePath.FullName, xboxPath.FullName, "Copy didn't pass the expected destination file path to the adapter.");
                    Assert.AreSame(expectedProgress, metrics, "Copy didn't pass the expected destination file path to the adapter.");

                    return null;
                };

            XboxFile.Copy(this.pcFile, this.xboxFilePath, this.XboxConsole);
            Assert.IsTrue(success, "FileInfo.CopyTo wasn't called.");

            XboxFile.Copy(this.pcFile, this.xboxFilePath, this.XboxConsole, null);

            expectedProgress = new Progress<XboxFileTransferMetric>();
            XboxFile.Copy(this.pcFile, this.xboxFilePath, this.XboxConsole, expectedProgress);
        }

        /// <summary>
        /// Verifies that the XboxFile.Copy method throws if given a null value for the sourceFile parameter.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestXboxFileCopyXboxPCArgumentNullSourceFile()
        {
            XboxFile.Copy(null, this.pcFile, this.XboxConsole);
        }

        /// <summary>
        /// Verifies that the XboxFile.Copy method throws if given a value for the sourceFile parameter that doesn't have an Xbox origin.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileTestCategory)]
        [ExpectedException(typeof(XboxConsoleFeatureNotSupportedException))]
        public void TestXboxFileCopyXboxPCNotSupportedSourceFile()
        {
            ShimXboxPath.HasXboxOriginString = path => false;
            XboxFile.Copy(this.xboxFilePath, this.pcFile, this.XboxConsole);
        }

        /// <summary>
        /// Verifies that the XboxFile.Copy method correctly invokes the XboxFileInfo.Copy.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileTestCategory)]
        public void TestXboxFileCopyXboxPCCallsXboxFileInfoCopyCorrectly()
        {
            IProgress<XboxFileTransferMetric> expectedProgress = null;

            bool success = false;
            ShimXboxPath.HasXboxOriginString = path => true;
            ShimXboxFileInfo.AllInstances.CopyStringIProgressOfXboxFileTransferMetric = (xboxFileInfo, path, metrics) => 
            { 
                success = true;

                Assert.AreEqual(this.xboxFilePath.FullName, xboxFileInfo.FullName, "Copy didn't pass the expected source file path to the adapter.");
                Assert.AreEqual(this.pcFile, path, "Copy didn't pass the expected destination file path to the adapter.");
                Assert.AreSame(expectedProgress, metrics, "Copy didn't pass the expected destination file path to the adapter.");
            };

            XboxFile.Copy(this.xboxFilePath, this.pcFile, this.XboxConsole);
            Assert.IsTrue(success, "XboxFileInfo.Copy wasn't called.");
            XboxFile.Copy(this.xboxFilePath, this.pcFile, this.XboxConsole, null);

            expectedProgress = new Progress<XboxFileTransferMetric>();
            XboxFile.Copy(this.xboxFilePath, this.pcFile, this.XboxConsole, expectedProgress);
        }

        /// <summary>
        /// Verifies that the XboxFile.Delete method correctly invokes the XboxFileInfo.Delete.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileTestCategory)]
        public void TestXboxFileDeleteCallsXboxFileInfoDeleteCorrectly()
        {
            bool success = false;
            ShimXboxFileInfo.AllInstances.Delete = xboxFileInfo => { success = true; };
            XboxFile.Delete(this.xboxFilePath, this.XboxConsole);
            Assert.IsTrue(success, "XboxFileInfo.Delete wasn't called.");
        }

        /// <summary>
        /// Verifies that the XboxFile.Exists method throws if given a null value for the console parameter.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestXboxFileExistsArgumentNullConsole()
        {
            XboxFile.Exists(this.xboxFilePath, null);
        }

        /// <summary>
        /// Verifies that the XboxFile.Exists method correctly invokes the XboxFileInfo.ExistsImpl.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileTestCategory)]
        public void TestXboxFileExistsCallsXboxFileInfoExistsImplCorrectly()
        {
            // test when a file exists:
            bool success = false;
            ShimXboxFileInfo.ExistsImplStringXboxPathXboxConsoleAdapterBase = (systemIpAddress, xboxPath, adapterBase) =>
                {
                    success = true;
                    Assert.AreEqual(this.XboxConsole.Adapter, adapterBase, "The passed adapter does not equal the original one.");
                    return true;
                };

            Assert.IsTrue(XboxFile.Exists(this.xboxFilePath, this.XboxConsole), "The file should exist.");
            Assert.IsTrue(success, "XboxFileInfo.ExistsImpl wasn't called.");

            // test when a file doesn't exist:
            success = false;
            ShimXboxFileInfo.ExistsImplStringXboxPathXboxConsoleAdapterBase = (systemIpAddress, xboxPath, adapterBase) =>
                {
                    success = true;
                    return false;
                };

            Assert.IsFalse(XboxFile.Exists(this.xboxFilePath, this.XboxConsole), "The file shouldn't exist.");
            Assert.IsTrue(success, "XboxFileInfo.ExistsImpl wasn't called.");
        }

        /// <summary>
        /// Verifies that the XboxFile.Move method correctly invokes the XboxFile.Copy and FileInfo.Delete.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileTestCategory)]
        public void TestXboxFileMovePCXboxCallsCopyFileInfoDeleteCorrectly()
        {
            int successSequenceChecker = 1;
            ShimXboxFile.CopyStringXboxPathXboxConsole = (sourceFile, destinationFile, console) =>
                {
                    successSequenceChecker *= 2;
                    Assert.AreEqual(this.pcFile, sourceFile, "The source file is incorrect.");
                    Assert.AreEqual(this.xboxFilePath, destinationFile, "The destination file is incorrect.");
                    Assert.AreEqual(this.XboxConsole, console, "The console is incorrect.");
                };

            ShimFileInfo.AllInstances.Delete = fileInfo => 
                { 
                    successSequenceChecker++;
                    Assert.IsTrue(string.Equals(this.pcFile, fileInfo.FullName, StringComparison.OrdinalIgnoreCase), "The file to be deleted is incorrect.");
                };

            XboxFile.Move(this.pcFile, this.xboxFilePath, this.XboxConsole);
            Assert.IsTrue(successSequenceChecker == 3, "XboxFile.Copy and FileInfo.Delete were not called in the required order.");
        }

        /// <summary>
        /// Verifies that the XboxFile.Move method correctly invokes the XboxFile.Copy and XboxFileInfo.Delete.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileTestCategory)]
        public void TestXboxFileMoveXboxPCCallsCopyXboxFileInfoDeleteCorrectly()
        {
            int successSequenceChecker = 1;
            ShimXboxFile.CopyXboxPathStringXboxConsole = (sourceFile, destinationFile, console) =>
            {
                successSequenceChecker *= 2;
                Assert.AreEqual(this.xboxFilePath, sourceFile, "The source file is incorrect.");
                Assert.AreEqual(this.pcFile, destinationFile, "The destination file is incorrect.");
                Assert.AreEqual(this.XboxConsole, console, "The console is incorrect.");
            };

            ShimXboxFileInfo.AllInstances.Delete = xboxFileInfo =>
            {
                successSequenceChecker++;
                Assert.IsTrue(string.Equals(this.xboxFilePath.FullName, xboxFileInfo.FullName, StringComparison.OrdinalIgnoreCase) && this.xboxFilePath.OperatingSystem == xboxFileInfo.OperatingSystem, "The file to be deleted is incorrect.");
            };

            XboxFile.Move(this.xboxFilePath, this.pcFile, this.XboxConsole);
            Assert.IsTrue(successSequenceChecker == 3, "XboxFile.Copy and XboxFileInfo.Delete were not called in the required order.");
        }
    }
}
