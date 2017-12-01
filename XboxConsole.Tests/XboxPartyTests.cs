//------------------------------------------------------------------------------
// <copyright file="XboxPartyTests.cs" company="Microsoft">
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
    using Microsoft.Internal.GamesTest.Xbox.Input;
    using Microsoft.QualityTools.Testing.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Represents tests for the XboxParty type.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "This is test code and the object is disposed in the TestCleanup method.")]
    [TestClass]
    public class XboxPartyTests
    {
        private const string XboxPartyTestCategory = "XboxConsole.XboxParty";
        private const string ConsoleAddress = "10.124.151.244"; // The actual IP address used here is irrelevant.     
        private const string TestXuidString = "111111111111111";
        private const string TestXuidString2 = "222222222222222";
        private const uint TestTitleId = 12345;
        private const string TestPartyId = "1234";
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
                    SystemIpAddressGet = () => IPAddress.Parse(XboxPartyTests.ConsoleAddress),
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
        /// Verifies that the constructors of the Xboxparty class correctly set the properties.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxPartyTestCategory)]
        public void TestConstructorsCorrectlySetProperties()
        {
            XboxParty party = new XboxParty(this.xboxConsole, TestTitleId);
            Assert.AreSame(this.xboxConsole, party.Console);
            Assert.AreEqual(TestTitleId, party.TitleId);

            party = XboxParty.FromTitleId(this.xboxConsole, TestTitleId);
            Assert.AreSame(this.xboxConsole, party.Console);
            Assert.AreEqual(TestTitleId, party.TitleId);
        }

        /// <summary>
        /// Verifies that PartyId property calls Adapter GetPartyId and passes the correct return value and arguments.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxPartyTestCategory)]
        public void TestPartyIdCallsAdapterGetPartyId()
        {
            bool isCorrectMethodCalled = false;

            ShimXboxConsoleAdapterBase.AllInstances.GetPartyIdStringUInt32 = (adapter, systemIpAddress, titleId) =>
            {
                isCorrectMethodCalled = true;

                Assert.AreEqual(ConsoleAddress, systemIpAddress);
                Assert.AreEqual(TestTitleId, titleId);

                return TestPartyId;
            };

            XboxParty party = new XboxParty(this.xboxConsole, TestTitleId);
            Assert.AreEqual(TestPartyId, party.PartyId);

            Assert.IsTrue(isCorrectMethodCalled, "XboxParty PartyId property did not call Adapter GetPartyId.");
        }

        /// <summary>
        /// Verifies that Members property calls Adapter GetPartyMembers and passes the correct return value and arguments.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxPartyTestCategory)]
        public void TestMembersCallsAdapterGetPartyMembers()
        {
            bool isCorrectMethodCalled = false;

            // Overload GetUsers() to fake TestXuidString as the only local user (therefore TestXuidString2 will be assumed to be a remote user)
            ShimXboxConsoleAdapterBase.AllInstances.GetUsersString = (adapter, systemIpAddress) =>
            {
                return new XboxUserDefinition[] { new XboxUserDefinition(0, string.Empty, string.Empty, true, false, TestXuidString) };
            };

            // Verify parameters are correct and adapter method is called.
            ShimXboxConsoleAdapterBase.AllInstances.GetPartyMembersStringUInt32 = (adapter, systemIpAddress, titleId) =>
            {
                isCorrectMethodCalled = true;

                Assert.AreEqual(TestTitleId, titleId, "Incorrect Title ID passed into GetPartyMembers.");

                return new string[] { TestXuidString, TestXuidString2 };
            };

            XboxParty party = new XboxParty(this.xboxConsole, TestTitleId);
            XboxUserBase[] returnValue = party.Members.ToArray();

            Assert.IsTrue(isCorrectMethodCalled, "XboxParty Members property did not call Adapter GetPartyMembers.");

            // Verify return value was passed correctly, TestXuidString was converted into XboxUser, and TestXuidString2 into XboxRemoteUser
            Assert.AreEqual(2, returnValue.Length);
            Assert.AreEqual(returnValue[0].Xuid, TestXuidString);
            Assert.AreEqual(returnValue[1].Xuid, TestXuidString2);
            Assert.IsInstanceOfType(returnValue[0], typeof(XboxUser));
            Assert.IsInstanceOfType(returnValue[1], typeof(XboxRemoteUser));
        }
    }
}
