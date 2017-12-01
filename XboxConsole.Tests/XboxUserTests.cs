//------------------------------------------------------------------------------
// <copyright file="XboxUserTests.cs" company="Microsoft">
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
    /// Represents tests for the XboxUser type.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "This is test code and the object is disposed in the TestCleanup method.")]
    [TestClass]
    public class XboxUserTests
    {
        private const string XboxUserTestCategory = "XboxConsole.XboxUser";
        private const string ConsoleAddress = "10.124.151.244"; // The actual IP address used here is irrelevant.
        private const string TestGamertag = "Test Gamertag";
        private const string TestEmailAddress = "TestAddress@test.test";
        private const string TestXuidString = "1111111111111111";
        private const uint TestUserId = 12345;
        private const string TestPartyId = "1234";
        private const uint TestTitleId = 12255;

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
                    SystemIpAddressGet = () => IPAddress.Parse(XboxUserTests.ConsoleAddress),
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
        /// Verifies that the PairWith methods call the adapter's PairGamepadToUser method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxUserTestCategory)]
        public void TestPairWithMethodsCallAdapterPairGamepadToUser()
        {
            var gamepad = new XboxGamepad(this.xboxConsole);
            gamepad.Connect();

            XboxUser user = this.CreateTestUser();

            bool isCorrectMethodCalled = false;

            ShimXboxConsoleAdapterBase.AllInstances.PairGamepadToUserStringUInt64UInt32 = (adapter, systemIpAddress, gamepadId, userId) =>
            {
                isCorrectMethodCalled = true;
            };

            user.PairWithPhysicalController(0);
            Assert.IsTrue(isCorrectMethodCalled, "The PairWithPhysicalController method didn't call the adapter's PairGamepadToUser(systemIpAddress, gamepadId, userId).");

            isCorrectMethodCalled = false;

            user.PairWithVirtualController(gamepad);
            Assert.IsTrue(isCorrectMethodCalled, "The PairWithVirtualController method didn't call the adapter's PairGamepadToUser(systemIpAddress, gamepadId, userId).");
        }

        /// <summary>
        /// Verifies that the PairWith methods call the adapter's PairGamepadToUser method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxUserTestCategory)]
        public void TestPairWithMethodsPassOnIds()
        {
            const uint ExpectedUserId = TestUserId;
            const ulong ExpectedGamepadId = 9876543210;
            var gamepad = new XboxGamepad(this.xboxConsole);

            XboxUser user = this.CreateTestUser();

            ShimXboxConsoleAdapterBase.AllInstances.ConnectXboxGamepadString = (_, __) => ExpectedGamepadId;

            gamepad.Connect();

            ShimXboxConsoleAdapterBase.AllInstances.PairGamepadToUserStringUInt64UInt32 = (adapter, systemIpAddress, gamepadId, userId) =>
            {
                Assert.AreEqual(ExpectedGamepadId, gamepadId, "The XboxUser did not pass in the expected id parameter");
                Assert.AreEqual(ExpectedUserId, userId, "The XboxUser did not pass in the expected id parameter");
            };

            user.PairWithPhysicalController(ExpectedGamepadId);

            user.PairWithVirtualController(gamepad);
        }

        /// <summary>
        /// Verifies that the PairWithVirtualController method throws an ArgumentNullException when a null gamepad is passed in.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxUserTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestPairWithVirtualControllerThrowsArgumentNullExceptions()
        {
            XboxUser user = this.CreateTestUser();

            ShimXboxConsoleAdapterBase.AllInstances.PairGamepadToUserStringUInt64UInt32 = (adapter, systemIpAddress, gamepadId, userId) =>
            {
                Assert.Fail("Test should never have gotten to the adapters method.");
            };

            user.PairWithVirtualController(null);
        }

        /// <summary>
        /// Verifies that the PairWithVirtualController method throws an ArgumentException when a gamepad with a null id is passed in.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxUserTestCategory)]
        [ExpectedException(typeof(ArgumentException))]
        public void TestPairWithVirtualControllerThrowsArgumentExceptions()
        {
            XboxGamepad gamepad = new XboxGamepad(this.xboxConsole);
            XboxUser user = this.CreateTestUser();

            ShimXboxConsoleAdapterBase.AllInstances.PairGamepadToUserStringUInt64UInt32 = (adapter, systemIpAddress, gamepadId, userId) =>
            {
                Assert.Fail("Test should never have gotten to the adapters method.");
            };

            user.PairWithVirtualController(gamepad);
        }

        /// <summary>
        /// Verifies that the constructors of the XboxUser class correctly set the properties.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxUserTestCategory)]
        public void TestConstructorsCorrectlySetProperties()
        {
            const uint ExpectedId = TestUserId;
            const string ExpectedEmail = TestEmailAddress;
            const string ExpectedGamerTag = TestGamertag;
            const string ExpectedXuidValue = TestXuidString;
            
            bool expectedAutoSignInValue = true;
            bool expectedIsSignedInValue = false;

            this.TestXboxUserProperties(
                ExpectedId,
                ExpectedEmail,
                ExpectedGamerTag,
                expectedIsSignedInValue,
                () => new XboxUser(this.xboxConsole, ExpectedId, ExpectedEmail, ExpectedGamerTag, false));

            this.TestXboxUserProperties(
                ExpectedId,
                ExpectedEmail,
                ExpectedGamerTag,
                expectedIsSignedInValue,
                () => new XboxUser(this.xboxConsole, ExpectedId, ExpectedEmail, ExpectedGamerTag, expectedIsSignedInValue));

            this.TestXboxUserProperties(
                ExpectedId,
                ExpectedEmail,
                ExpectedGamerTag,
                expectedIsSignedInValue,
                expectedAutoSignInValue,
                ExpectedXuidValue,
                () => new XboxUser(this.xboxConsole, ExpectedId, ExpectedEmail, ExpectedGamerTag, expectedIsSignedInValue, expectedAutoSignInValue, ExpectedXuidValue));

            expectedIsSignedInValue = true;
            expectedAutoSignInValue = false;

            this.TestXboxUserProperties(
                ExpectedId,
                ExpectedEmail,
                ExpectedGamerTag,
                expectedIsSignedInValue,
                () => new XboxUser(this.xboxConsole, ExpectedId, ExpectedEmail, ExpectedGamerTag, expectedIsSignedInValue));

            this.TestXboxUserProperties(
                ExpectedId,
                ExpectedEmail,
                ExpectedGamerTag,
                expectedIsSignedInValue,
                expectedAutoSignInValue,
                ExpectedXuidValue,
                () => new XboxUser(this.xboxConsole, ExpectedId, ExpectedEmail, ExpectedGamerTag, expectedIsSignedInValue, expectedAutoSignInValue, ExpectedXuidValue));

            XboxUserDefinition definition = new XboxUserDefinition(ExpectedId, ExpectedEmail, ExpectedGamerTag, expectedIsSignedInValue);

            this.TestXboxUserProperties(
                ExpectedId,
                ExpectedEmail,
                ExpectedGamerTag,
                expectedIsSignedInValue,
                () => new XboxUser(this.xboxConsole, definition));
        }

        /// <summary>
        /// Verifies that the SignIn method calls the adapters SignInUser method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxUserTestCategory)]
        public void TestSignInCallsAdapterSignIn()
        {
            XboxUser user = this.CreateTestUser();

            bool isCorrectMethodCalled = false;

            ShimXboxConsoleAdapterBase.AllInstances.SignInUserStringXboxUserDefinitionStringBoolean = (adapter, systemIpAddress, userDefinition, password, storePassword) =>
            {
                isCorrectMethodCalled = true;
                return this.CreateTestUserDefinition();
            };

            user.SignIn(null, false);
            Assert.IsTrue(isCorrectMethodCalled, "The SignIn method didn't call the adapter's SignInUser(systemIpAddress, userDefinition, password, storePassword).");
        }

        /// <summary>
        /// Verifies that the SignIn method passes on its parameters correctly.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxUserTestCategory)]
        public void TestSignInPassesArguments()
        {
            const uint ExpectedUserId = TestUserId;
            const string ExpectedEmailAddress = TestEmailAddress;
            const string ExpectedGamerTag = TestGamertag;
            const string ExpectedPassword = "TestPassword";
            const bool ExpectedIsSignedInValue = true;

            bool expectedStorePassword = false;

            XboxUser user = new XboxUser(this.xboxConsole, ExpectedUserId, ExpectedEmailAddress, ExpectedGamerTag, !ExpectedIsSignedInValue);

            ShimXboxConsoleAdapterBase.AllInstances.SignInUserStringXboxUserDefinitionStringBoolean = (adapter, systemIpAddress, userDefinition, password, storePassword) =>
            {
                Assert.AreSame(user.Definition, userDefinition, "The passed in XboxUserDefinition was not the same as the expected user definition.");
                Assert.AreEqual(ExpectedPassword, password, "The passed in password string was not the same as the expected password string.");
                Assert.AreEqual(expectedStorePassword, storePassword, "The passed in storePassword value was not the same as the expected storePassword value.");

                return new XboxUserDefinition(userDefinition.UserId, userDefinition.EmailAddress, userDefinition.Gamertag, ExpectedIsSignedInValue);
            };

            user.SignIn(ExpectedPassword, expectedStorePassword);
            Assert.AreEqual(ExpectedIsSignedInValue, user.IsSignedIn, "The SignOut method didn't set the users IsSignedIn property based on return value.");

            expectedStorePassword = true;
            user.SignIn(ExpectedPassword, expectedStorePassword);
        }

        /// <summary>
        /// Verifies that the SignIn method handles a null return value.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxUserTestCategory)]
        [ExpectedException(typeof(XboxSignInException))]
        public void TestSignInHandlesNullReturn()
        {
            XboxUser user = this.CreateTestUser();

            ShimXboxConsoleAdapterBase.AllInstances.SignInUserStringXboxUserDefinitionStringBoolean = (adapter, systemIpAddress, userDefinition, password, storePassword) =>
            {
                return null;
            };

            user.SignIn(null, false);
        }

        /// <summary>
        /// Verifies that the SignIn method handles an incorrect return value.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxUserTestCategory)]
        [ExpectedException(typeof(XboxSignInException))]
        public void TestSignInHandlesIncorrectReturn()
        {
            XboxUser user = this.CreateTestUser();

            ShimXboxConsoleAdapterBase.AllInstances.SignInUserStringXboxUserDefinitionStringBoolean = (adapter, systemIpAddress, userDefinition, password, storePassword) =>
            {
                return new XboxUserDefinition(TestUserId, "DifferentEmail", null, false);
            };

            user.SignIn(null, false);
        }

        /// <summary>
        /// Verifies that the SignOut method calls the adapters SignOutUser method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxUserTestCategory)]
        public void TestSignOutInvokesAdapterSignOut()
        {
            XboxUser user = this.CreateTestUser();

            bool isCorrectMethodCalled = false;

            ShimXboxConsoleAdapterBase.AllInstances.SignOutUserStringXboxUserDefinition = (adapter, systemIpAddress, userDefinition) =>
            {
                isCorrectMethodCalled = true;
                return this.CreateTestUserDefinition();
            };

            user.SignOut();
            Assert.IsTrue(isCorrectMethodCalled, "The SignOut method didn't call the adapter's SignOutUser(systemIpAddress, userDefinition).");
        }

        /// <summary>
        /// Verifies that the SignOut method passes on its parameters correctly.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxUserTestCategory)]
        public void TestSignOutPassesArguments()
        {
            const uint ExpectedUserId = TestUserId;
            const string ExpectedEmailAddress = TestEmailAddress;
            const string ExpectedGamerTag = TestGamertag;

            const bool ExpectedIsSignedInValue = false;

            XboxUser user = new XboxUser(this.xboxConsole, ExpectedUserId, ExpectedEmailAddress, ExpectedGamerTag, !ExpectedIsSignedInValue);

            ShimXboxConsoleAdapterBase.AllInstances.SignOutUserStringXboxUserDefinition = (adapter, systemIpAddress, userDefinition) =>
            {
                Assert.AreSame(user.Definition, userDefinition, "The passed in XboxUserDefinition was not the same as the expected user definition.");

                return new XboxUserDefinition(userDefinition.UserId, userDefinition.EmailAddress, userDefinition.Gamertag, ExpectedIsSignedInValue);
            };

            user.SignOut();
            Assert.AreEqual(ExpectedIsSignedInValue, user.IsSignedIn, "The SignOut method didn't set the users IsSignedIn property based on return value.");
        }

        /// <summary>
        /// Verifies that the SignOut method handles a null return value.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxUserTestCategory)]
        [ExpectedException(typeof(XboxSignInException))]
        public void TestSignOutHandlesNullReturn()
        {
            XboxUser user = this.CreateTestUser();

            ShimXboxConsoleAdapterBase.AllInstances.SignOutUserStringXboxUserDefinition = (adapter, systemIpAddress, userDefinition) =>
            {
                return null;
            };

            user.SignOut();
        }

        /// <summary>
        /// Verifies that the SignOut methods handles an incorrect return value.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxUserTestCategory)]
        [ExpectedException(typeof(XboxSignInException))]
        public void TestSignOutHandlesIncorrectReturn()
        {
            XboxUser user = this.CreateTestUser();

            ShimXboxConsoleAdapterBase.AllInstances.SignOutUserStringXboxUserDefinition = (adapter, systemIpAddress, userDefinition) =>
            {
                return new XboxUserDefinition(TestUserId, "DifferentEmail", null, false);
            };

            user.SignOut();
        }

        /// <summary>
        /// Verifies that this method calls the corresponding method on the adapter and passes the correct parameters.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxUserTestCategory)]
        public void TestAddLocalUsersToPartyInvokesAdapterAddLocalUsersToParty()
        {
            bool isCorrectMethodCalled = false;

            ShimXboxConsoleAdapterBase.AllInstances.AddLocalUsersToPartyStringUInt32StringStringArray = (adapter, systemIpAddress, titleId, actingUserXuid, addUserXuids) =>
            {
                isCorrectMethodCalled = true;

                Assert.AreEqual(TestTitleId, titleId);
                Assert.AreEqual(TestXuidString, actingUserXuid);
                Assert.IsNotNull(addUserXuids);
                Assert.AreEqual(1, addUserXuids.Length);
                Assert.AreEqual(TestXuidString, addUserXuids[0]);
            };

            this.CreateTestUser().AddLocalUsersToParty(XboxParty.FromTitleId(this.xboxConsole, TestTitleId), new XboxUser[] { this.CreateTestUser() });

            Assert.IsTrue(isCorrectMethodCalled, "XboxUser method did not call the correct Adapter method.");
        }

        /// <summary>
        /// Verifies that this method calls the corresponding method on the adapter and passes the correct parameters.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxUserTestCategory)]
        public void TestInviteToPartyInvokesAdapterInviteToParty()
        {
            bool isCorrectMethodCalled = false;

            ShimXboxConsoleAdapterBase.AllInstances.InviteToPartyStringUInt32StringStringArray = (adapter, systemIpAddress, titleId, actingUserXuid, inviteUserXuids) =>
            {
                isCorrectMethodCalled = true;

                Assert.AreEqual(TestTitleId, titleId);
                Assert.AreEqual(TestXuidString, actingUserXuid);
                Assert.IsNotNull(inviteUserXuids);
                Assert.AreEqual(1, inviteUserXuids.Length);
                Assert.AreEqual(TestXuidString, inviteUserXuids[0]);
            };

            this.CreateTestUser().InviteToParty(XboxParty.FromTitleId(this.xboxConsole, TestTitleId), new XboxRemoteUser[] { this.CreateTestRemoteUser() });

            Assert.IsTrue(isCorrectMethodCalled, "XboxUser method did not call the correct Adapter method.");
        }

        /// <summary>
        /// Verifies that this method calls the corresponding method on the adapter and passes the correct parameters.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxUserTestCategory)]
        public void TestAcceptInviteToPartyInvokesAdapterAcceptInviteToParty()
        {
            bool isCorrectMethodCalled = false;

            ShimXboxConsoleAdapterBase.AllInstances.AcceptInviteToPartyStringStringString = (adapter, systemIp, actingUserXuid, partyId) =>
            {
                isCorrectMethodCalled = true;

                Assert.AreEqual(TestXuidString, actingUserXuid);
                Assert.AreEqual(TestPartyId, partyId);
            };

            this.CreateTestUser().AcceptInviteToParty(XboxRemoteParty.FromPartyId(this.xboxConsole, TestPartyId));

            Assert.IsTrue(isCorrectMethodCalled, "XboxUser method did not call the correct Adapter method.");
        }

        /// <summary>
        /// Verifies that this method calls the corresponding method on the adapter and passes the correct parameters.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxUserTestCategory)]
        public void TestDeclineInviteToPartyInvokesAdapterDeclineInviteToParty()
        {
            bool isCorrectMethodCalled = false;

            ShimXboxConsoleAdapterBase.AllInstances.DeclineInviteToPartyStringStringString = (adapter, systemIp, actingUserXuid, partyId) =>
            {
                isCorrectMethodCalled = true;

                Assert.AreEqual(TestXuidString, actingUserXuid);
                Assert.AreEqual(TestPartyId, partyId);
            };

            this.CreateTestUser().DeclineInviteToParty(XboxRemoteParty.FromPartyId(this.xboxConsole, TestPartyId));

            Assert.IsTrue(isCorrectMethodCalled, "XboxUser method did not call the correct Adapter method.");
        }        

        private void TestXboxUserProperties(uint expectedId, string expectedEmail, string expectedGamerTag, bool expectedIsSignedInValue, Func<XboxUser> constructorFunc)
        {
            XboxUser user = constructorFunc();

            Assert.AreEqual(expectedId, user.UserId, "UserId Property did not match expected value");
            Assert.AreEqual(expectedEmail, user.EmailAddress, "EmailAddress Property did not match expected value");
            Assert.AreEqual(expectedGamerTag, user.GamerTag, "GamerTag Property did not match expected value");
            Assert.AreEqual(expectedIsSignedInValue, user.IsSignedIn, "IsSignedIn Property did not match expected value");
        }

        private void TestXboxUserProperties(uint expectedId, string expectedEmail, string expectedGamerTag, bool expectedIsSignedInValue, bool expectedAutoSignInValue, string expectedXuidValue, Func<XboxUser> constructorFunc)
        {
            XboxUser user = constructorFunc();

            Assert.AreEqual(expectedId, user.UserId, "UserId Property did not match expected value");
            Assert.AreEqual(expectedEmail, user.EmailAddress, "EmailAddress Property did not match expected value");
            Assert.AreEqual(expectedGamerTag, user.GamerTag, "GamerTag Property did not match expected value");
            Assert.AreEqual(expectedIsSignedInValue, user.IsSignedIn, "IsSignedIn Property did not match expected value");
            Assert.AreEqual(expectedAutoSignInValue, user.AutoSignIn, "AutoSignIn Property did not match the expected value");
            Assert.AreEqual(expectedXuidValue, user.Xuid, "Xuid Property did not match the expected value");
        }

        private XboxUserDefinition CreateTestUserDefinition()
        {
            return new XboxUserDefinition(TestUserId, TestEmailAddress, TestGamertag, false, false, TestXuidString);
        }

        private XboxUser CreateTestUser()
        {
            return new XboxUser(this.xboxConsole, this.CreateTestUserDefinition());
        }

        private XboxRemoteUser CreateTestRemoteUser()
        {
            return new XboxRemoteUser(this.xboxConsole, TestXuidString);
        }
    }
}
