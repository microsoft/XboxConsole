//------------------------------------------------------------------------------
// <copyright file="XboxDirectoryTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Tests
{
    using System;
    using System.Globalization;
    using System.IO.Fakes;
    using System.Net;
    using Microsoft.Internal.GamesTest.Xbox.Fakes;
    using Microsoft.Internal.GamesTest.Xbox.IO;
    using Microsoft.Internal.GamesTest.Xbox.IO.Fakes;
    using Microsoft.QualityTools.Testing.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Class to house tests for the XboxDirectory class.
    /// </summary>
    [TestClass]
    public class XboxDirectoryTests
    {
        private const string XboxDirectoryTestCategory = "XboxConsole.XboxDirectory";

        private const string ConsoleAddress = "10.124.151.244"; // The actual IP address used here is irrelevant.

        private string pcDirectory = @"c:\TestDirectory";
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
            this.shimAdapter = new ShimXboxConsoleAdapterBase(new StubXboxConsoleAdapterBase(null));
            this.shimXboxConsole = new ShimXboxConsole
            {
                AdapterGet = () => this.shimAdapter,
                SystemIpAddressGet = () => IPAddress.Parse(XboxDirectoryTests.ConsoleAddress),
                SystemIpAddressAndSessionKeyCombinedGet = () => XboxDirectoryTests.ConsoleAddress,
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
        /// Verifies that the XboxDirectory.Copy method throws if given a null value for the destinationDirectory parameter.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxDirectoryTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestXboxDirectoryCopyPCXboxArgumentNullDestinationDirectory()
        {
            XboxDirectory.Copy(this.pcDirectory, null, this.XboxConsole);
        }

        /// <summary>
        /// Verifies that the XboxDirectory.Copy method throws if given a value for the destinationDirectory parameter that doesn't have an Xbox origin.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxDirectoryTestCategory)]
        [ExpectedException(typeof(XboxConsoleFeatureNotSupportedException))]
        public void TestXboxDirectoryCopyPCXboxNotSupportedDestinationDirectory()
        {
            ShimXboxPath.HasXboxOriginString = path => false;
            XboxDirectory.Copy(this.pcDirectory, this.xboxDirectoryPath, this.XboxConsole);
        }

        /// <summary>
        /// Verifies that the XboxDirectory.Copy method correctly invokes the DirectoryInfo.CopyTo.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxDirectoryTestCategory)]
        public void TestXboxDirectoryCopyPCXboxCallsDirectoryInfoCopyToCorrectly()
        {
            bool success = false;
            IProgress<XboxFileTransferMetric> expectedProgress = null;

            ShimXboxPath.HasXboxOriginString = path => true;
            ShimDirectoryInfoExtensions.CopyToDirectoryInfoXboxPathXboxConsoleIProgressOfXboxFileTransferMetric = (directoryInfo, xboxPath, console, metrics) => 
                { 
                    success = true; 
                    Assert.AreEqual(this.pcDirectory, directoryInfo.FullName, "Copy didn't pass on the correct directory.");
                    Assert.AreEqual(this.xboxDirectoryPath, xboxPath, "Copy didn't pass on the correct xbox directory.");
                    Assert.AreSame(expectedProgress, metrics, "Copy didn't pass on the same progress object.");
                    return null;
                };

            XboxDirectory.Copy(this.pcDirectory, this.xboxDirectoryPath, this.XboxConsole);
            Assert.IsTrue(success, "DirectoryInfo.CopyTo wasn't called.");

            XboxDirectory.Copy(this.pcDirectory, this.xboxDirectoryPath, this.XboxConsole, null);

            expectedProgress = new Progress<XboxFileTransferMetric>();
            XboxDirectory.Copy(this.pcDirectory, this.xboxDirectoryPath, this.XboxConsole, expectedProgress);
        }

        /// <summary>
        /// Verifies that the XboxDirectory.Copy method throws if given a null value for the sourceDirectory parameter.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxDirectoryTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestXboxDirectoryCopyXboxPCArgumentNullSourceDirectory()
        {
            XboxDirectory.Copy(null, this.pcDirectory, this.XboxConsole);
        }

        /// <summary>
        /// Verifies that the XboxDirectory.Copy method throws if given a value for the sourceDirectory parameter that doesn't have an Xbox origin.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxDirectoryTestCategory)]
        [ExpectedException(typeof(XboxConsoleFeatureNotSupportedException))]
        public void TestXboxDirectoryCopyXboxPCNotSupportedSourceDirectory()
        {
            ShimXboxPath.HasXboxOriginString = path => false;
            XboxDirectory.Copy(this.xboxDirectoryPath, this.pcDirectory, this.XboxConsole);
        }

        /// <summary>
        /// Verifies that the XboxDirectory.Copy method correctly invokes the XboxDirectoryInfo.Copy.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxDirectoryTestCategory)]
        public void TestXboxDirectoryCopyXboxPCCallsXboxDirectoryInfoCopyCorrectly()
        {
            bool success = false;
            IProgress<XboxFileTransferMetric> expectedProgress = null;

            ShimXboxPath.HasXboxOriginString = path => true;
            ShimXboxDirectoryInfo.AllInstances.CopyStringIProgressOfXboxFileTransferMetric = (xboxDirectoryInfo, path, metrics) => 
            { 
                success = true;

                Assert.AreEqual(this.xboxDirectoryPath.FullName, xboxDirectoryInfo.FullName, "Copy didn't pass on the correct directory.");
                Assert.AreEqual(this.pcDirectory, path, "Copy didn't pass on the correct xbox directory.");
                Assert.AreSame(expectedProgress, metrics, "Copy didn't pass on the same progress object.");
            };

            XboxDirectory.Copy(this.xboxDirectoryPath, this.pcDirectory, this.XboxConsole);
            Assert.IsTrue(success, "XboxDirectoryInfo.Copy wasn't called.");

            XboxDirectory.Copy(this.xboxDirectoryPath, this.pcDirectory, this.XboxConsole, null);

            expectedProgress = new Progress<XboxFileTransferMetric>();
            XboxDirectory.Copy(this.xboxDirectoryPath, this.pcDirectory, this.XboxConsole, expectedProgress);
        }

        /// <summary>
        /// Verifies that the XboxDirectory.Create method correctly invokes the XboxDirectoryInfo.Create.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxDirectoryTestCategory)]
        public void TestXboxDirectoryCreateCallsXboxDirectoryInfoCreateCorrectly()
        {
            bool success = false;
            ShimXboxDirectoryInfo.AllInstances.Create = xboxDirectoryInfo => { success = true; };
            XboxDirectory.Create(this.xboxDirectoryPath, this.XboxConsole);
            Assert.IsTrue(success, "XboxDirectoryInfo.Create wasn't called.");
        }

        /// <summary>
        /// Verifies that the XboxDirectory.Delete method correctly invokes the XboxDirectoryInfo.Delete.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxDirectoryTestCategory)]
        public void TestXboxDirectoryDeleteCallsXboxDirectoryInfoDeleteCorrectly()
        {
            bool success = false;
            ShimXboxDirectoryInfo.AllInstances.Delete = xboxDirectoryInfo => { success = true; };
            XboxDirectory.Delete(this.xboxDirectoryPath, this.XboxConsole);
            Assert.IsTrue(success, "XboxDirectoryInfo.Delete wasn't called.");
        }

        /// <summary>
        /// Verifies that the XboxDirectory.Delete (recursive) method correctly invokes the XboxDirectoryInfo.Delete (recursive).
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxDirectoryTestCategory)]
        public void TestXboxDirectoryDeleteRecursiveCallsXboxDirectoryInfoDeleteRecursiveCorrectly()
        {
            // test non-recursive:
            bool success = false;
            bool recursive = false;
            ShimXboxDirectoryInfo.AllInstances.DeleteBoolean = (xboxDirectoryInfo, isRecursive) =>
            {
                success = true;
                Assert.IsTrue(recursive == isRecursive, string.Format(CultureInfo.CurrentCulture, "Recursion behavior does not correspond to the specified parameter ({0}).", recursive));
            };

            XboxDirectory.Delete(this.xboxDirectoryPath, recursive, this.XboxConsole);
            Assert.IsTrue(success, "XboxDirectoryInfo.Delete (recursive) wasn't called.");

            // test recursive:
            success = false;
            recursive = true;
            XboxDirectory.Delete(this.xboxDirectoryPath, recursive, this.XboxConsole);
            Assert.IsTrue(success, "XboxDirectoryInfo.Delete (recursive) wasn't called.");
        }

        /// <summary>
        /// Verifies that the XboxDirectory.Exists method throws if given a null value for the console parameter.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxDirectoryTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestXboxDirectoryExistsArgumentNullConsole()
        {
            XboxDirectory.Exists(this.xboxDirectoryPath, null);
        }

        /// <summary>
        /// Verifies that the XboxDirectory.Exists method correctly invokes the XboxDirectoryInfo.ExistsImpl.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxDirectoryTestCategory)]
        public void TestXboxDirectoryExistsCallsXboxDirectoryInfoExistsImplCorrectly()
        {
            // test when a directory exists:
            bool success = false;
            ShimXboxDirectoryInfo.ExistsImplStringXboxPathXboxConsoleAdapterBase = (systemIpAddress, xboxPath, adapterBase) =>
                {
                    success = true;
                    Assert.AreEqual(this.XboxConsole.Adapter, adapterBase, "The passed adapter does not equal the original one.");
                    return true;
                };

            Assert.IsTrue(XboxDirectory.Exists(this.xboxDirectoryPath, this.XboxConsole), "The directory should exist.");
            Assert.IsTrue(success, "XboxDirectoryInfo.ExistsImpl wasn't called.");

            // test when a directory doesn't exist:
            success = false;
            ShimXboxDirectoryInfo.ExistsImplStringXboxPathXboxConsoleAdapterBase = (systemIpAddress, xboxPath, adapterBase) =>
                {
                    success = true;
                    return false;
                };

            Assert.IsFalse(XboxDirectory.Exists(this.xboxDirectoryPath, this.XboxConsole), "The directory shouldn't exist.");
            Assert.IsTrue(success, "XboxDirectoryInfo.ExistsImpl wasn't called.");
        }

        /// <summary>
        /// Verifies that the XboxDirectory.Move method correctly invokes the XboxDirectory.Copy and DirectoryInfo.Delete.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxDirectoryTestCategory)]
        public void TestXboxDirectoryMovePCXboxCallsCopyDirectoryInfoDeleteCorrectly()
        {
            int successSequenceChecker = 1;
            ShimXboxDirectory.CopyStringXboxPathXboxConsole = (sourceDirectory, destinationDirectory, console) =>
                {
                    successSequenceChecker *= 2;
                    Assert.AreEqual(this.pcDirectory, sourceDirectory, "The source directory is incorrect.");
                    Assert.AreEqual(this.xboxDirectoryPath, destinationDirectory, "The destination directory is incorrect.");
                    Assert.AreEqual(this.XboxConsole, console, "The console is incorrect.");
                };

            ShimDirectoryInfo.AllInstances.DeleteBoolean = (directoryInfo, isRecursive) => 
                { 
                    successSequenceChecker++;
                    Assert.IsTrue(string.Equals(this.pcDirectory, directoryInfo.FullName, StringComparison.OrdinalIgnoreCase), "The directory to be deleted is incorrect.");
                    Assert.IsTrue(isRecursive, "The delete should be recursive.");
                };

            XboxDirectory.Move(this.pcDirectory, this.xboxDirectoryPath, this.XboxConsole);
            Assert.IsTrue(successSequenceChecker == 3, "XboxDirectory.Copy and DirectoryInfo.Delete were not called in the required order.");
        }

        /// <summary>
        /// Verifies that the XboxDirectory.Move method correctly invokes the XboxDirectory.Copy and XboxDirectoryInfo.Delete.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxDirectoryTestCategory)]
        public void TestXboxDirectoryMoveXboxPCCallsCopyXboxDirectoryInfoDeleteCorrectly()
        {
            int successSequenceChecker = 1;
            ShimXboxDirectory.CopyXboxPathStringXboxConsole = (sourceDirectory, destinationDirectory, console) =>
            {
                successSequenceChecker *= 2;
                Assert.AreEqual(this.xboxDirectoryPath, sourceDirectory, "The source directory is incorrect.");
                Assert.AreEqual(this.pcDirectory, destinationDirectory, "The destination directory is incorrect.");
                Assert.AreEqual(this.XboxConsole, console, "The console is incorrect.");
            };

            ShimXboxDirectoryInfo.AllInstances.DeleteBoolean = (xboxDirectoryInfo, isRecursive) =>
            {
                successSequenceChecker++;
                Assert.IsTrue(string.Equals(this.xboxDirectoryPath.FullName, xboxDirectoryInfo.FullName, StringComparison.OrdinalIgnoreCase) && this.xboxDirectoryPath.OperatingSystem == xboxDirectoryInfo.OperatingSystem, "The directory to be deleted is incorrect.");
                Assert.IsTrue(isRecursive, "The delete should be recursive.");
            };

            XboxDirectory.Move(this.xboxDirectoryPath, this.pcDirectory, this.XboxConsole);
            Assert.IsTrue(successSequenceChecker == 3, "XboxDirectory.Copy and XboxDirectoryInfo.Delete were not called in the required order.");
        }
    }
}
