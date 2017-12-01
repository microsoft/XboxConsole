//------------------------------------------------------------------------------
// <copyright file="XboxConsoleDeploymentTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Internal.GamesTest.Xbox.Configuration;
    using Microsoft.Internal.GamesTest.Xbox.Configuration.Fakes;
    using Microsoft.Internal.GamesTest.Xbox.Deployment;
    using Microsoft.Internal.GamesTest.Xbox.Fakes;
    using Microsoft.Internal.GamesTest.Xbox.Input;
    using Microsoft.Internal.GamesTest.Xbox.Tasks;
    using Microsoft.QualityTools.Testing.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Represents tests for the XboxConsole type.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "This is test code and the object is disposed in the TestCleanup method.")]
    [TestClass]
    public class XboxConsoleDeploymentTests
    {
        private const string XboxConsoleTestCategory = "XboxConsole.XboxConsole.Deployment";

        private const string ConsoleAddress = "10.124.151.244"; // The actual IP address used here is irrelevant.
        private const string ConsoleSystemIpAddress = "10.124.151.245"; // The actual IP address used here is irrelevant.

        private const string PackageFamilyName = "NuiView.ERA_8wekyb3d8bbwe";
        private const string PackageFullName = "NuiView.ERA_1.0.0.0_neutral__8wekyb3d8bbwe";
        private const string ApplicationId = "NuiView.ERA";
        private const string Aumid = PackageFamilyName + "!" + ApplicationId;
        private static readonly string[] Aumids = { Aumid };

        private IDisposable shimsContext;
        private StubXboxConsoleAdapterBase stubAdapter;
        private XboxConsole console;

        /// <summary>
        /// Called once before each test to setup shim and stub objects.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.shimsContext = ShimsContext.Create();

            ShimXboxConsoleAdapterBase.ConstructorXboxXdkBase = (adapter, xboxXdk) =>
            {
            };

            this.stubAdapter = new StubXboxConsoleAdapterBase(null);

            ShimXboxConsole.ConstructorIPAddress = (realConsole, address) =>
            {
                var myShim = new ShimXboxConsole(realConsole)
                {
                    AdapterGet = () => this.stubAdapter,
                    SystemIpAddressGet = () => IPAddress.Parse(XboxConsoleDeploymentTests.ConsoleAddress),
                    SystemIpAddressStringGet = () => this.console.SystemIpAddress.ToString(),
                };
            };

            this.console = new XboxConsole((IPAddress)null);
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
        /// Verifies that if the adapter level returns a null value for the list of installed applications,
        /// then the XboxConsole level will throw and Exception.
        /// </summary>
        /// <returns>Returns a task for the running test.</returns>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxConsoleTestCategory)]
        public async Task TestDeployPushAsyncThrowsAnExceptionIfAdapterReturnsNull()
        {
            ShimXboxConsoleAdapterBase.AllInstances.DeployPushAsyncStringStringBooleanIProgressOfXboxDeploymentMetricIProgressOfXboxDeploymentErrorIProgressOfXboxDeploymentExtraFile
                = (adapter, systemIpAddress, deployPath, removeExtraFiles, progressMetric, progressError, progressExtraFile) =>
                {
                    return Task.FromResult<XboxPackageDefinition>(null);
                };

            ShimXboxConsoleAdapterBase.AllInstances.DeployPushAsyncStringStringBooleanCancellationTokenIProgressOfXboxDeploymentMetricIProgressOfXboxDeploymentErrorIProgressOfXboxDeploymentExtraFile
                = (adapter, systemIpAddress, deployPath, removeExtraFiles, cancellationToken, progressMetric, progressError, progressExtraFile) =>
                {
                    return Task.FromResult<XboxPackageDefinition>(null);
                };

            try
            {
                await this.console.DeployPushAsync(@"C:\", false).WithTimeout(TimeSpan.FromSeconds(30));
                Assert.Fail("XboxConsole should have thrown an exception if the adapter returns null.");
            }
            catch (XboxConsoleException e)
            {
                Assert.AreEqual("Adapter returned an unexpected value", e.Message, "DeployPushAsync threw an unexpected exception");
            }

            try
            {
                await this.console.DeployPushAsync(@"C:\", false, null, null, null).WithTimeout(TimeSpan.FromSeconds(30));
                Assert.Fail("XboxConsole should have thrown an exception if the adapter returns null.");
            }
            catch (XboxConsoleException e)
            {
                Assert.AreEqual("Adapter returned an unexpected value", e.Message, "DeployPushAsync threw an unexpected exception");
            }

            try
            {
                await this.console.DeployPushAsync(@"C:\", false, CancellationToken.None).WithTimeout(TimeSpan.FromSeconds(30));
                Assert.Fail("XboxConsole should have thrown an exception if the adapter returns null.");
            }
            catch (XboxConsoleException e)
            {
                Assert.AreEqual("Adapter returned an unexpected value", e.Message, "DeployPushAsync threw an unexpected exception");
            }

            try
            {
                await this.console.DeployPushAsync(@"C:\", false, CancellationToken.None, null, null, null).WithTimeout(TimeSpan.FromSeconds(30));
                Assert.Fail("XboxConsole should have thrown an exception if the adapter returns null.");
            }
            catch (XboxConsoleException e)
            {
                Assert.AreEqual("Adapter returned an unexpected value", e.Message, "DeployPushAsync threw an unexpected exception");
            }
        }

        /// <summary>
        /// Verifies that DeployPushAsync calls the adapter's DeployPushAsync(). 
        /// </summary>
        /// <returns>Returns a task for the running test.</returns>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxConsoleTestCategory)]
        public async Task TestDeployPushAsyncCallsAdapterDeployPushAsync()
        {
            bool isCorrectMethodCalled = false;
            ShimXboxConsoleAdapterBase.AllInstances.DeployPushAsyncStringStringBooleanIProgressOfXboxDeploymentMetricIProgressOfXboxDeploymentErrorIProgressOfXboxDeploymentExtraFile
                = (adapter, systemIpAddress, deployPath, removeExtraFiles, progressMetric, progressError, progressExtraFile) =>
                {
                    isCorrectMethodCalled = true;
                    return Task.FromResult(new XboxPackageDefinition(PackageFullName, Aumids));
                };

            ShimXboxConsoleAdapterBase.AllInstances.DeployPushAsyncStringStringBooleanCancellationTokenIProgressOfXboxDeploymentMetricIProgressOfXboxDeploymentErrorIProgressOfXboxDeploymentExtraFile
                = (adapter, systemIpAddress, deployPath, removeExtraFiles, cancellationToken, progressMetric, progressError, progressExtraFile) =>
                {
                    isCorrectMethodCalled = true;
                    return Task.FromResult(new XboxPackageDefinition(PackageFullName, Aumids));
                };

            await this.console.DeployPushAsync(@"C:\", false).WithTimeout(TimeSpan.FromSeconds(30));

            Assert.IsTrue(isCorrectMethodCalled, "The XboxConsole object did not call the adapter's DeployPushAsync method.");

            isCorrectMethodCalled = false;
            await this.console.DeployPushAsync(@"C:\", false, null, null, null).WithTimeout(TimeSpan.FromSeconds(30));

            Assert.IsTrue(isCorrectMethodCalled, "The XboxConsole object did not call the adapter's DeployPushAsync method.");

            isCorrectMethodCalled = false;
            await this.console.DeployPushAsync(@"C:\", false, CancellationToken.None).WithTimeout(TimeSpan.FromSeconds(30));

            Assert.IsTrue(isCorrectMethodCalled, "The XboxConsole object did not call the adapter's DeployPushAsync method.");

            isCorrectMethodCalled = false;
            await this.console.DeployPushAsync(@"C:\", false, CancellationToken.None, null, null, null).WithTimeout(TimeSpan.FromSeconds(30));

            Assert.IsTrue(isCorrectMethodCalled, "The XboxConsole object did not call the adapter's DeployPushAsync method.");
        }

        /// <summary>
        /// Verifies that DeployPushAsync overload with no progress calls the adapter with null progress arguments. 
        /// </summary>
        /// <returns>Returns a task for the running test.</returns>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxConsoleTestCategory)]
        public async Task TestDeployPushAsyncNoProgressOverloadCallsAdapterWithNullProgress()
        {
            ShimXboxConsoleAdapterBase.AllInstances.DeployPushAsyncStringStringBooleanIProgressOfXboxDeploymentMetricIProgressOfXboxDeploymentErrorIProgressOfXboxDeploymentExtraFile
                = (adapter, systemIpAddress, deployPath, removeExtraFiles, progressMetric, progressError, progressExtraFile) =>
                {
                    Assert.IsNull(progressMetric, "XboxConsole overload with no progress did not pass in null to adapter progress arguments");
                    Assert.IsNull(progressError, "XboxConsole overload with no progress did not pass in null to adapter progress arguments");
                    Assert.IsNull(progressExtraFile, "XboxConsole overload with no progress did not pass in null to adapter progress arguments");

                    return Task.FromResult(new XboxPackageDefinition(PackageFullName, Aumids));
                };

            ShimXboxConsoleAdapterBase.AllInstances.DeployPushAsyncStringStringBooleanCancellationTokenIProgressOfXboxDeploymentMetricIProgressOfXboxDeploymentErrorIProgressOfXboxDeploymentExtraFile
                = (adapter, systemIpAddress, deployPath, removeExtraFiles, cancellationToken, progressMetric, progressError, progressExtraFile) =>
                {
                    Assert.IsNull(progressMetric, "XboxConsole overload with no progress did not pass in null to adapter progress arguments");
                    Assert.IsNull(progressError, "XboxConsole overload with no progress did not pass in null to adapter progress arguments");
                    Assert.IsNull(progressExtraFile, "XboxConsole overload with no progress did not pass in null to adapter progress arguments");

                    return Task.FromResult(new XboxPackageDefinition(PackageFullName, Aumids));
                };

            await this.console.DeployPushAsync(@"C:\", false).WithTimeout(TimeSpan.FromSeconds(30));

            await this.console.DeployPushAsync(@"C:\", false, CancellationToken.None).WithTimeout(TimeSpan.FromSeconds(30));
        }

        /// <summary>
        /// Verifies that DeployPushAsync correctly handles valid adapter output. 
        /// </summary>
        /// <returns>Returns a task for the running test.</returns>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxConsoleTestCategory)]
        public async Task TestDeployPushAsync()
        {
            ShimXboxConsoleAdapterBase.AllInstances.DeployPushAsyncStringStringBooleanIProgressOfXboxDeploymentMetricIProgressOfXboxDeploymentErrorIProgressOfXboxDeploymentExtraFile
                = (adapter, systemIpAddress, deployPath, removeExtraFiles, progressMetric, progressError, progressExtraFile) =>
                {
                    return Task.FromResult(new XboxPackageDefinition(PackageFullName, Aumids));
                };
            ShimXboxConsoleAdapterBase.AllInstances.DeployPushAsyncStringStringBooleanCancellationTokenIProgressOfXboxDeploymentMetricIProgressOfXboxDeploymentErrorIProgressOfXboxDeploymentExtraFile
                = (adapter, systemIpAddress, deployPath, removeExtraFiles, cancellationToken, progressMetric, progressError, progressExtraFile) =>
                {
                    return Task.FromResult(new XboxPackageDefinition(PackageFullName, Aumids));
                };

            XboxPackage package = await this.console.DeployPushAsync(@"C:\", false).WithTimeout(TimeSpan.FromSeconds(30));

            Assert.IsNotNull(package, "DeployPushAsync returned a null value when adapter return a valid value");
            Assert.AreEqual(PackageFullName, package.FullName, "Package FullName did not match the expected name of the value returned by the adapter");

            package = await this.console.DeployPushAsync(@"C:\", false, null, null, null).WithTimeout(TimeSpan.FromSeconds(30));

            Assert.IsNotNull(package, "DeployPushAsync returned a null value when adapter return a valid value");
            Assert.AreEqual(PackageFullName, package.FullName, "Package FullName did not match the expected name of the value returned by the adapter");

            package = await this.console.DeployPushAsync(@"C:\", false, CancellationToken.None).WithTimeout(TimeSpan.FromSeconds(30));

            Assert.IsNotNull(package, "DeployPushAsync returned a null value when adapter return a valid value");
            Assert.AreEqual(PackageFullName, package.FullName, "Package FullName did not match the expected name of the value returned by the adapter");

            package = await this.console.DeployPushAsync(@"C:\", false, CancellationToken.None, null, null, null).WithTimeout(TimeSpan.FromSeconds(30));

            Assert.IsNotNull(package, "DeployPushAsync returned a null value when adapter return a valid value");
            Assert.AreEqual(PackageFullName, package.FullName, "Package FullName did not match the expected name of the value returned by the adapter");
        }

        /// <summary>
        /// Verifies that the adapter correctly passes on the parameters to the Xdk.
        /// </summary>
        /// <returns>Returns a task for the running test.</returns>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxConsoleTestCategory)]
        public async Task TestDeployPushAsyncPassesOnParametersToAdapter()
        {
            Progress<XboxDeploymentMetric> progressMetricObject = new Progress<XboxDeploymentMetric>();
            Progress<XboxDeploymentError> progressErrorObject = new Progress<XboxDeploymentError>();
            Progress<XboxDeploymentExtraFile> progressExtraFileObject = new Progress<XboxDeploymentExtraFile>();

            bool expectedRemoveExtraFilesValue = false;
            string expectedDeployPathValue = @"C:\";
            CancellationToken expectedCancellationToken = new CancellationToken();

            ShimXboxConsoleAdapterBase.AllInstances.DeployPushAsyncStringStringBooleanIProgressOfXboxDeploymentMetricIProgressOfXboxDeploymentErrorIProgressOfXboxDeploymentExtraFile
                = (adapter, systemIpAddress, deployPath, removeExtraFiles, progressMetric, progressError, progressExtraFile) =>
                {
                    Assert.AreSame(progressMetricObject, progressMetric, "Adapter failed to pass in the same XboxDeploymentMetric object to the Xdk");
                    Assert.AreSame(progressErrorObject, progressError, "Adapter failed to pass in the same XboxDeploymentError object to the Xdk");
                    Assert.AreSame(progressExtraFileObject, progressExtraFile, "Adapter failed to pass in the same XboxDeploymentExtraFile object to the Xdk");

                    Assert.AreEqual(expectedRemoveExtraFilesValue, removeExtraFiles, "Adapter failed to pass in the correct value for the removeExtraFiles parameter to the Xdk");
                    Assert.AreEqual(expectedDeployPathValue, deployPath, "Adapter failed to pass in the correct value for the deployPath parameter to the Xdk");

                    return Task.FromResult(new XboxPackageDefinition(PackageFullName, Aumids));
                };

            ShimXboxConsoleAdapterBase.AllInstances.DeployPushAsyncStringStringBooleanCancellationTokenIProgressOfXboxDeploymentMetricIProgressOfXboxDeploymentErrorIProgressOfXboxDeploymentExtraFile
                = (adapter, systemIpAddress, deployPath, removeExtraFiles, cancellationToken, progressMetric, progressError, progressExtraFile) =>
                {
                    Assert.AreSame(progressMetricObject, progressMetric, "Adapter failed to pass in the same XboxDeploymentMetric object to the Xdk");
                    Assert.AreSame(progressErrorObject, progressError, "Adapter failed to pass in the same XboxDeploymentError object to the Xdk");
                    Assert.AreSame(progressExtraFileObject, progressExtraFile, "Adapter failed to pass in the same XboxDeploymentExtraFile object to the Xdk");

                    Assert.AreEqual(expectedRemoveExtraFilesValue, removeExtraFiles, "Adapter failed to pass in the correct value for the removeExtraFiles parameter to the Xdk");
                    Assert.AreEqual(expectedDeployPathValue, deployPath, "Adapter failed to pass in the correct value for the deployPath parameter to the Xdk");

                    Assert.AreEqual(expectedCancellationToken, cancellationToken, "Adapter failed to pass in the correct value for the cancellationToken parameter to the Xdk");

                    return Task.FromResult(new XboxPackageDefinition(PackageFullName, Aumids));
                };

            await this.console.DeployPushAsync(
                    expectedDeployPathValue,
                    expectedRemoveExtraFilesValue,
                    progressMetricObject,
                    progressErrorObject,
                    progressExtraFileObject).WithTimeout(TimeSpan.FromSeconds(30));

            expectedRemoveExtraFilesValue = true;

            await this.console.DeployPushAsync(
                    expectedDeployPathValue,
                    expectedRemoveExtraFilesValue,
                    progressMetricObject,
                    progressErrorObject,
                    progressExtraFileObject).WithTimeout(TimeSpan.FromSeconds(30));

            expectedRemoveExtraFilesValue = false;

            await this.console.DeployPushAsync(
                    expectedDeployPathValue,
                    expectedRemoveExtraFilesValue,
                    expectedCancellationToken,
                    progressMetricObject,
                    progressErrorObject,
                    progressExtraFileObject).WithTimeout(TimeSpan.FromSeconds(30));

            expectedRemoveExtraFilesValue = true;

            await this.console.DeployPushAsync(
                    expectedDeployPathValue,
                    expectedRemoveExtraFilesValue,
                    expectedCancellationToken,
                    progressMetricObject,
                    progressErrorObject,
                    progressExtraFileObject).WithTimeout(TimeSpan.FromSeconds(30));
        }
    }
}