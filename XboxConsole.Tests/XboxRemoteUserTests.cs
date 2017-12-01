//------------------------------------------------------------------------------
// <copyright file="XboxRemoteUserTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Microsoft.Internal.GamesTest.Xbox.Fakes;
    using Microsoft.Internal.GamesTest.Xbox.Input;
    using Microsoft.QualityTools.Testing.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Represents tests for the XboxRemoteUser type.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "This is test code and the object is disposed in the TestCleanup method.")]
    [TestClass]
    public class XboxRemoteUserTests
    {
        private const string XboxRemoteUserTestCategory = "XboxConsole.XboxRemoteUser";
        private const string ConsoleAddress = "10.124.151.244"; // The actual IP address used here is irrelevant.     
        private const string TestXuidString = "1111111111111111";
        private IDisposable shimsContext;
        private XboxConsole xboxConsole;

        /// <summary>
        /// Called once before each test to setup shim and stub objects.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.shimsContext = ShimsContext.Create();

            ShimXboxConsole.ConstructorIPAddress = (console, address) =>
            {
                var myShim = new ShimXboxConsole(console)
                {
                    AdapterGet = () => new StubXboxConsoleAdapterBase(null),
                    SystemIpAddressGet = () => IPAddress.Parse(XboxRemoteUserTests.ConsoleAddress),
                    SystemIpAddressStringGet = () => this.xboxConsole.SystemIpAddress.ToString(),
                    XboxGamepadsGet = () => new List<Input.XboxGamepad>(),
                };
            };

            ShimXboxConsoleAdapterBase.ConstructorXboxXdkBase = (adapter, xboxXdk) =>
            {
            };

            this.xboxConsole = new XboxConsole((IPAddress)null);
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
        /// Verifies that the constructors of the XboxUser class set the properties correctly.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxRemoteUserTestCategory)]
        public void TestConstructorsCorrectlySetProperties()
        {
            const string ExpectedXuidValue = TestXuidString;

            XboxRemoteUser remoteUser = new XboxRemoteUser(this.xboxConsole, ExpectedXuidValue);
            Assert.AreSame(this.xboxConsole, remoteUser.Console);
            Assert.AreEqual(ExpectedXuidValue, remoteUser.Xuid);

            remoteUser = XboxRemoteUser.FromXuid(this.xboxConsole, ExpectedXuidValue);
            Assert.AreSame(this.xboxConsole, remoteUser.Console);
            Assert.AreEqual(ExpectedXuidValue, remoteUser.Xuid);
        }
    }
}
