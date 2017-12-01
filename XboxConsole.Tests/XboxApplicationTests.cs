//------------------------------------------------------------------------------
// <copyright file="XboxApplicationTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Tests
{    
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Microsoft.Internal.GamesTest.Xbox.Fakes;
    using Microsoft.QualityTools.Testing.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Class housing tests for the XboxApplication class.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "This is a test class and the objects are disposed in the TestCleanup method.")]
    [TestClass]
    public class XboxApplicationTests
    {
        private const string XboxApplicationTestCategory = "XboxConsole.XboxApplication";

        private const string ConsoleAddress = "10.124.151.244"; // The actual IP address used here is irrelevant.

        private const string PackageFamilyName = "NuiView.ERA_8wekyb3d8bbwe";
        private const string PackageFullName = "NuiView.ERA_1.0.0.0_neutral__8wekyb3d8bbwe";
        private const string ApplicationId = "NuiView.ERA";
        private const string Aumid = PackageFamilyName + "!" + ApplicationId;
        private const string TestArguments = "firstargument secondArgument";
        private static readonly string[] Aumids = { Aumid };

        private XboxApplication xboxApplication;
        private XboxPackage xboxPackage;
        private XboxPackageDefinition packageDefinition;

        private IDisposable shimsContext;
        private XboxConsole xboxConsole;

        /// <summary>
        /// Called once before each test to setup shim and stub objects.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.shimsContext = ShimsContext.Create();

            this.packageDefinition = new XboxPackageDefinition(PackageFullName, PackageFamilyName, Aumids);

            ShimXboxConsole.ConstructorIPAddress = (console, address) =>
            {
                var myShim = new ShimXboxConsole(console)
                {
                    AdapterGet = () => new StubXboxConsoleAdapterBase(null),
                    SystemIpAddressGet = () => IPAddress.Parse(XboxApplicationTests.ConsoleAddress),
                    XboxGamepadsGet = () => new List<GamesTest.Xbox.Input.XboxGamepad>()
                };
            };

            ShimXboxConsoleAdapterBase.ConstructorXboxXdkBase = (adapter, xboxXdk) =>
            {
            };

            this.xboxConsole = new XboxConsole((IPAddress)null);
            this.xboxPackage = new XboxPackage(this.packageDefinition, this.xboxConsole);
            this.xboxApplication = this.xboxPackage.Applications.First();
        }

        /// <summary>
        /// Called once after each test to clean up shim and stub objects.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            this.xboxConsole.Dispose();
            this.shimsContext.Dispose();
        }

        /// <summary>
        /// Verifies that the Launch method calls the adapter's LaunchApplication method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxApplicationTestCategory)]
        public void TestLaunchCallsAdapterLaunchApplication()
        {
            bool isCorrectMethodCalled = false;
            ShimXboxConsoleAdapterBase.AllInstances.LaunchApplicationStringXboxApplicationDefinition = (adapter, systemIpAddress, application) =>
            {
                isCorrectMethodCalled = true;
            };

            this.xboxApplication.Launch();
            Assert.IsTrue(isCorrectMethodCalled, "The Launch() method didn't call the adapter's LaunchApplication(systemIpAddress, application).");
        }

        /// <summary>
        /// Verifies that the Launch method overload with command line arguments calls the adapter's LaunchApplication method with arguments.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxApplicationTestCategory)]
        public void TestLaunchWithArgumentsCallsAdapterLaunchApplicationWithArguments()
        {
            bool isCorrectMethodCalled = false;
            ShimXboxConsoleAdapterBase.AllInstances.LaunchApplicationStringXboxApplicationDefinitionString = (adapter, systemIpAddress, application, TestArguments) =>
            {
                isCorrectMethodCalled = true;
            };

            this.xboxApplication.Launch(XboxApplicationTests.TestArguments);
            Assert.IsTrue(isCorrectMethodCalled, "The Launch(arguments) method didn't call the adapter's LaunchApplication(systemIpAddress, application, arguments).");
        }

        /// <summary>
        /// Verifies that the Snap method calls the adapter's SnapApplication method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxApplicationTestCategory)]
        public void TestSnapCallsAdapterSnapApplication()
        {
            bool isCorrectMethodCalled = false;
            ShimXboxConsoleAdapterBase.AllInstances.SnapApplicationStringXboxApplicationDefinition = (adapter, systemIpAddress, application) =>
            {
                isCorrectMethodCalled = true;
            };

            this.xboxApplication.Snap();
            Assert.IsTrue(isCorrectMethodCalled, "The Snap() method didn't call the adapter's SnapApplication(systemIpAddress, application).");
        }

        /// <summary>
        /// Verifies that the XboxApplication class will throw an ArgumentNullException
        /// if given a null value for the applicationDefinition parameter.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxApplicationTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestXboxApplicationThrowsArgumentNullExceptionWithNullApplicationDefinition()
        {
#pragma warning disable 168
            XboxApplication notUsed = new XboxApplication(null, this.xboxPackage, this.xboxConsole);
            Assert.IsNotNull(notUsed, "XboxApplication constructor succeded with a null applicationDefinition, and did not throw an ArgumentNullException as expected.");
#pragma warning restore 168
        }

        /// <summary>
        /// Verifies that the XboxApplication class will throw an ArgumentNullException
        /// if given a null value for the package parameter.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxApplicationTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestXboxApplicationThrowsArgumentNullExceptionWithNullPackage()
        {
#pragma warning disable 168
            XboxApplication notUsed = new XboxApplication(this.xboxApplication.Definition, null, this.xboxConsole);
            Assert.IsNotNull(notUsed, "XboxApplication constructor succeded with a null package, and did not throw an ArgumentNullException as expected.");
#pragma warning restore 168
        }

        /// <summary>
        /// Verifies that the XboxApplication class will throw an ArgumentNullException
        /// if given a null value for the console parameter.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxApplicationTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestXboxApplicationThrowsArgumentNullExceptionWithNullConsole()
        {
#pragma warning disable 168
            XboxApplication notUsed = new XboxApplication(this.xboxApplication.Definition, this.xboxPackage, null);
            Assert.IsNotNull(notUsed, "XboxApplication constructor succeded with a null console, and did not throw an ArgumentNullException as expected.");
#pragma warning restore 168
        }
    }
}
