//------------------------------------------------------------------------------
// <copyright file="XboxConsoleAdapter.User.Tests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Adapter.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Represents tests for the XboxConsole adapter types.
    /// </summary>
    public partial class XboxConsoleAdapterTests
    {
        private const string AdapterUserTestCategory = "Adapter.User";
        private const string TestEmailAddressString = "TestEmailAddress@Test.test";
        private const string TestXuidString = "1212121212121212";
        private const string TestGamerTag = "TestGamerTag";
        private const string TestPartyId = "1234567";
        private const uint TestTitleId = 0x12345;

        private const int InvalidEmailErrorCode = unchecked((int)0x80048823);
        private const int PasswordNotStoredErrorCode = unchecked((int)0x8004882E);
        private const int InvalidPasswordErrorCode = unchecked((int)0x80048821);
        private const int SignedInElsewhereErrorCode = unchecked((int)0x8015DC16);
        private const int InvalidXuidErrorCode = unchecked((int)0x80004003);
        private const int NoLocalUsersErrorCode = unchecked((int)0x87CC0007);
        private const int InvalidPartyInviteErrorCode = unchecked((int)0x8019019c);

        /// <summary>
        /// Verifies that if the XDK's GetUsers method throws a COMException, then the
        /// XboxConsoleAdapter will wrap that into an XboxConsoleException with the COMException as its inner exception.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestGetUsersTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.GetUsersFunc = _ =>
            {
                throw new COMException();
            };

            try
            {
                this.adapter.GetUsers(ConsoleAddress);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The adapter GetUsers method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The adapter GetUsers method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that an ObjectDisposedException is thrown if the GetUsers()
        /// method is called after the object has been disposed.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestGetUsersThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.GetUsers(ConsoleAddress);
        }

        /// <summary>
        /// Verifies that the adapter's GetUsers() method
        /// calls the XboxXdk's GetUsers method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestGetUsersCallsXdkGetUsers()
        {
            bool isCorrectFunctionCalled = false;
            this.fakeXboxXdk.GetUsersFunc = ipAddress =>
            {
                isCorrectFunctionCalled = true;
                return Enumerable.Empty<XboxUserDefinition>();
            };

            this.adapter.GetUsers(ConsoleAddress);

            Assert.IsTrue(isCorrectFunctionCalled, "The adapter's GetUsers function failed to call the XboxXdk's GetUsers function.");

            this.VerifyThrows<ArgumentNullException>(() => this.adapter.GetUsers(null));
        }

        /// <summary>
        /// Verifies that the adapter correctly passes on the enumerable from the XDK.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestGetUsers()
        {
            var expectedDefinition = new XboxUserDefinition(0, "Test email address", "Test gamertag", true, false, TestXuidString);
            var expectedDefinitionCollection = new XboxUserDefinition[] { expectedDefinition };

            this.fakeXboxXdk.GetUsersFunc = ipAddress =>
            {
                return expectedDefinitionCollection;
            };

            IEnumerable<XboxUserDefinition> users = this.adapter.GetUsers(ConsoleAddress);
            Assert.IsNotNull(users, "GetUsers returned a null value.");
            Assert.AreSame(expectedDefinitionCollection, users, "The returned user collection was not the same object as the expected one.");
            Assert.IsTrue(users.Count() == 1, "The call to GetUsers packages should have returned exactly one packages.");

            foreach (XboxUserDefinition user in users)
            {
                Assert.AreSame(expectedDefinition, user, "User collection contained an unexpected user.");
            }
        }

        /// <summary>
        /// Verifies that if the XDK's PairControllerToUser method throws a COMException, then the
        /// XboxConsoleAdapter will wrap that into an XboxConsoleException with the COMException as its inner exception.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestPairGamepadToUserTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.PairControllerToUserFunc = (ipAddress, userId, controllerId) =>
            {
                throw new COMException();
            };

            try
            {
                this.adapter.PairGamepadToUser(ConsoleAddress, 0, 0);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The adapter PairGamepadToUser method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The adapter PairGamepadToUser method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that an ObjectDisposedException is thrown if the PairGamepadToUser()
        /// method is called after the object has been disposed.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestPairGamepadToUserThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.PairGamepadToUser(ConsoleAddress, 0, 0);
        }

        /// <summary>
        /// Verifies that the adapter's PairGamepadToUser() method
        /// calls the XboxXdk's PairControllerToUser method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestPairGamepadToUserCallsXdkPairControllerToUser()
        {
            bool isCorrectFunctionCalled = false;
            this.fakeXboxXdk.PairControllerToUserFunc = (ipAddress, userId, controllerId) =>
            {
                isCorrectFunctionCalled = true;
            };

            this.adapter.PairGamepadToUser(ConsoleAddress, 0, 0);

            Assert.IsTrue(isCorrectFunctionCalled, "The adapter's PairGamepadToUser function failed to call the XboxXdk's PairControllerToUser function.");
        }

        /// <summary>
        /// Verifies that the adapter's PairGamepadToUser method correctly handles passing on arguments.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestPairGamepadToUserArguments()
        {
            const uint ExpectedUserId = 12345;
            const ulong ExpectedControllerId = 9876543210;

            this.VerifyThrows<ArgumentNullException>(() => this.adapter.PairGamepadToUser(null, ExpectedControllerId, ExpectedUserId));

            this.fakeXboxXdk.PairControllerToUserFunc = (ipAddress, userId, controllerId) =>
            {
                Assert.AreEqual(ExpectedUserId, userId, "Adapter did not pass on the expected user id.");
                Assert.AreEqual(ExpectedControllerId, controllerId, "Adapter did not pass on the expected controller id.");
            };

            this.adapter.PairGamepadToUser(ConsoleAddress, ExpectedControllerId, ExpectedUserId);
        }

        /// <summary>
        /// Verifies that if the XDK's PairControllerToUserExclusive method throws a COMException, then the
        /// XboxConsoleAdapter will wrap that into an XboxConsoleException with the COMException as its inner exception.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestPairGamepadToUserExclusiveTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.PairControllerToUserExclusiveFunc = (ipAddress, userId, controllerId) =>
            {
                throw new COMException();
            };

            try
            {
                this.adapter.PairGamepadToUserExclusive(ConsoleAddress, 0, 0);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The adapter PairGamepadToUserExclusive method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The adapter PairGamepadToUserExclusive method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that an ObjectDisposedException is thrown if the PairGamepadToUserExclusive()
        /// method is called after the object has been disposed.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestPairGamepadToUserExclusiveThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.PairGamepadToUserExclusive(ConsoleAddress, 0, 0);
        }

        /// <summary>
        /// Verifies that the adapter's PairGamepadToUserExclusive() method
        /// calls the XboxXdk's PairControllerToUserExclusive method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestPairGamepadToUserExclusiveCallsXdkPairControllerToUser()
        {
            bool isCorrectFunctionCalled = false;
            this.fakeXboxXdk.PairControllerToUserExclusiveFunc = (ipAddress, userId, controllerId) =>
            {
                isCorrectFunctionCalled = true;
            };

            this.adapter.PairGamepadToUserExclusive(ConsoleAddress, 0, 0);

            Assert.IsTrue(isCorrectFunctionCalled, "The adapter's PairGamepadToUserExclusive function failed to call the XboxXdk's PairControllerToUserExclusive function.");
        }

        /// <summary>
        /// Verifies that the adapter's PairGamepadToUserExclusive method correctly handles passing on arguments.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestPairGamepadToUserExclusiveArguments()
        {
            const uint ExpectedUserId = 12345;
            const ulong ExpectedControllerId = 9876543210;

            this.VerifyThrows<ArgumentNullException>(() => this.adapter.PairGamepadToUserExclusive(null, ExpectedControllerId, ExpectedUserId));

            this.fakeXboxXdk.PairControllerToUserExclusiveFunc = (ipAddress, userId, controllerId) =>
            {
                Assert.AreEqual(ExpectedUserId, userId, "Adapter did not pass on the expected user id.");
                Assert.AreEqual(ExpectedControllerId, controllerId, "Adapter did not pass on the expected controller id.");
            };

            this.adapter.PairGamepadToUserExclusive(ConsoleAddress, ExpectedControllerId, ExpectedUserId);
        }

        /// <summary>
        /// Verifies that if the XDK's AddGuestUser method throws a COMException, then the
        /// XboxConsoleAdapter will wrap that into an XboxConsoleException with the COMException as its inner exception.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestAddGuestUserTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.AddGuestUserFunc = ipAddress =>
            {
                throw new COMException();
            };

            try
            {
                this.adapter.AddGuestUser(ConsoleAddress);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The adapter AddGuestUser method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The adapter AddGuestUser method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that an ObjectDisposedException is thrown if the AddGuestUser()
        /// method is called after the object has been disposed.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestAddGuestUserThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.AddGuestUser(ConsoleAddress);
        }

        /// <summary>
        /// Verifies that the adapter's AddGuestUser() method
        /// calls the XboxXdk's AddGuestUser method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestAddGuestUserCallsXdkAddGuestUser()
        {
            bool isCorrectFunctionCalled = false;
            this.fakeXboxXdk.AddGuestUserFunc = ipAddress =>
            {
                isCorrectFunctionCalled = true;
                return 0;
            };

            this.adapter.AddGuestUser(ConsoleAddress);

            Assert.IsTrue(isCorrectFunctionCalled, "The adapter's AddGuestUser function failed to call the XboxXdk's AddGuestUser function.");

            this.VerifyThrows<ArgumentNullException>(() => this.adapter.AddGuestUser(null));
        }

        /// <summary>
        /// Verifies that the adapter correctly passes on the enumerable from the XDK.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestAddGuestUser()
        {
            uint expectedUserId = 42;

            this.fakeXboxXdk.AddGuestUserFunc = ipAddress =>
            {
                return expectedUserId;
            };

            var userId = this.adapter.AddGuestUser(ConsoleAddress);
            Assert.AreEqual(expectedUserId, userId, "The returned user id was not equal to the expected one.");
        }

        /// <summary>
        /// Verifies that if the XDK's AddUser method throws a COMException, then the
        /// XboxConsoleAdapter will wrap that into an XboxConsoleException with the COMException as its inner exception.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestAddUserTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.AddUserFunc = (ipAddress, emailAddress) =>
            {
                throw new COMException();
            };

            try
            {
                this.adapter.AddUser(ConsoleAddress, TestEmailAddressString);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The adapter AddUser method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The adapter AddUser method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that an ObjectDisposedException is thrown if the AddUser()
        /// method is called after the object has been disposed.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestAddUserThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.AddUser(ConsoleAddress, TestEmailAddressString);
        }

        /// <summary>
        /// Verifies that an ArgumentException is thrown if the AddUser()
        /// method is passed an invalid email address.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestAddUserInvalidEmailThrowsArgumentException()
        {
            this.VerifyThrows<ArgumentException>(() => this.adapter.AddUser(ConsoleAddress, null));
            this.VerifyThrows<ArgumentException>(() => this.adapter.AddUser(ConsoleAddress, string.Empty));
            this.VerifyThrows<ArgumentException>(() => this.adapter.AddUser(ConsoleAddress, "  "));
        }

        /// <summary>
        /// Verifies that an XboxConsoleException is thrown if the Xdk
        /// method returns null.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        [ExpectedException(typeof(XboxConsoleException))]
        public void TestAddUserThrowsXboxConsoleException()
        {
            this.fakeXboxXdk.AddUserFunc = (ipAddress, emailAddress) => null;

            this.adapter.AddUser(ConsoleAddress, TestEmailAddressString);
        }

        /// <summary>
        /// Verifies that the adapter's AddUser() method
        /// calls the XboxXdk's AddUser method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestAddUserCallsXdkAddUser()
        {
            bool isCorrectFunctionCalled = false;
            this.fakeXboxXdk.AddUserFunc = (ipAddress, emailAddress) =>
            {
                isCorrectFunctionCalled = true;
                return new XboxUserDefinition(0, null, null, false);
            };

            this.adapter.AddUser(ConsoleAddress, TestEmailAddressString);

            Assert.IsTrue(isCorrectFunctionCalled, "The adapter's AddUser function failed to call the XboxXdk's AddUser function.");
        }

        /// <summary>
        /// Verifies that the adapter's AddUser method correctly handles passing on arguments.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestAddUserArguments()
        {
            const string ExpectedEmailAddress = TestEmailAddressString;

            this.VerifyThrows<ArgumentNullException>(() => this.adapter.AddUser(null, ExpectedEmailAddress));

            this.fakeXboxXdk.AddUserFunc = (ipAddress, emailAddress) =>
            {
                Assert.AreEqual(ExpectedEmailAddress, emailAddress, "Adapter did not pass on the expected email address.");
                return new XboxUserDefinition(0, null, null, false);
            };

            this.adapter.AddUser(ConsoleAddress, ExpectedEmailAddress);
        }

        /// <summary>
        /// Verifies that if the XDK's DeleteAllUsers method throws a COMException, then the
        /// XboxConsoleAdapter will wrap that into an XboxConsoleException with the COMException as its inner exception.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestDeleteAllUsersTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.DeleteAllUsersAction = (ipAddress) =>
            {
                throw new COMException();
            };

            try
            {
                this.adapter.DeleteAllUsers(ConsoleAddress);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The adapter DeleteAllUsers method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The adapter DeleteAllUsers method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that an ObjectDisposedException is thrown if the DeleteAllUsers()
        /// method is called after the object has been disposed.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestDeleteAllUsersThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.DeleteAllUsers(ConsoleAddress);
        }

        /// <summary>
        /// Verifies that an ArgumentNullException is thrown if the DeleteAllUsers()
        /// method with null ip address.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestDeleteAllUsersThrowsArgumentNullException()
        {
            this.adapter.DeleteAllUsers(null);
        }

        /// <summary>
        /// Verifies that the adapter's DeleteAllUsers() method
        /// calls the XboxXdk's DeleteAllUsers method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestDeleteAllUsersCallsXdkDeleteAllUsers()
        {
            bool isCorrectFunctionCalled = false;
            this.fakeXboxXdk.DeleteAllUsersAction = (ipAddress) =>
            {
                isCorrectFunctionCalled = true;
            };

            this.adapter.DeleteAllUsers(ConsoleAddress);

            Assert.IsTrue(isCorrectFunctionCalled, "The adapter's DeleteAllUsers function failed to call the XboxXdk's DeleteAllUsers function.");
        }

        /// <summary>
        /// Verifies that if the XDK's DeleteUser method throws a COMException, then the
        /// XboxConsoleAdapter will wrap that into an XboxConsoleException with the COMException as its inner exception.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestDeleteUserTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.DeleteUserAction = (ipAddress, user) =>
            {
                throw new COMException();
            };

            try
            {
                this.adapter.DeleteUser(ConsoleAddress, null);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The adapter DeleteUser method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The adapter DeleteUser method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that an ObjectDisposedException is thrown if the DeleteUser()
        /// method is called after the object has been disposed.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestDeleteUserThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.DeleteUser(ConsoleAddress, null);
        }

        /// <summary>
        /// Verifies that an ArgumentNullException is thrown if the DeleteUser()
        /// method with null ip address.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestDeleteUserThrowsArgumentNullException()
        {
            this.adapter.DeleteUser(null, new XboxUserDefinition(0, null, null, false));
        }

        /// <summary>
        /// Verifies that the adapter's DeleteUser() method
        /// calls the XboxXdk's DeleteUser method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestDeleteUserCallsXdkDeleteUser()
        {
            XboxUserDefinition expectedUser = new XboxUserDefinition(1, TestEmailAddressString, TestGamerTag, false);

            bool isCorrectFunctionCalled = false;

            this.fakeXboxXdk.DeleteUserAction = (ipAddress, user) =>
            {
                isCorrectFunctionCalled = true;
                Assert.AreSame(expectedUser, user, "Adapter did not pass on the expected user.");
            };

            this.adapter.DeleteUser(ConsoleAddress, expectedUser);

            Assert.IsTrue(isCorrectFunctionCalled, "The adapter's DeleteUser function failed to call the XboxXdk's DeleteUser function.");
        }

        /// <summary>
        /// Verifies that if the XDK's SignOutUser method throws a COMException, then the
        /// XboxConsoleAdapter will wrap that into an XboxConsoleException with the COMException as its inner exception.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestSignOutUserTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.SignOutUserFunc = (ipAddress, user) =>
            {
                throw new COMException();
            };

            try
            {
                this.adapter.SignOutUser(ConsoleAddress, null);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The adapter SignOutUser method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The adapter SignOutUser method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that an ObjectDisposedException is thrown if the SignOutUser()
        /// method is called after the object has been disposed.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestSignOutUserThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.SignOutUser(ConsoleAddress, null);
        }

        /// <summary>
        /// Verifies that an ArgumentNullException is thrown if the SignOutUser()
        /// method with null ip address.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSignOutUserThrowsArgumentNullException()
        {
            this.adapter.SignOutUser(null, new XboxUserDefinition(0, null, null, false));
        }

        /// <summary>
        /// Verifies that the adapter's SignOutUser() method
        /// calls the XboxXdk's SignOutUser method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestSignOutUserCallsXdkSignOutUser()
        {
            XboxUserDefinition expectedUser = new XboxUserDefinition(1, TestEmailAddressString, TestGamerTag, false);

            bool isCorrectFunctionCalled = false;

            this.fakeXboxXdk.SignOutUserFunc = (ipAddress, user) =>
            {
                isCorrectFunctionCalled = true;
                Assert.AreSame(expectedUser, user, "Adapter did not pass on the expected user.");
                return null;
            };

            this.adapter.SignOutUser(ConsoleAddress, expectedUser);

            Assert.IsTrue(isCorrectFunctionCalled, "The adapter's SignOutUser function failed to call the XboxXdk's SignOutUser function.");
        }

        /// <summary>
        /// Verifies that if the XDK's SignInUser method throws a COMException, then the
        /// XboxConsoleAdapter will wrap that into an XboxConsoleException with the COMException as its inner exception.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestSignInUserTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.SignInUserFunc = (ipAddress, user, password, storePassword) =>
            {
                throw new COMException();
            };

            try
            {
                this.adapter.SignInUser(ConsoleAddress, new XboxUserDefinition(0, null, null, false), null, false);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The adapter SignInUser method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The adapter SignInUser method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that an ObjectDisposedException is thrown if the SignInUser()
        /// method is called after the object has been disposed.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestSignInUserThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.SignInUser(ConsoleAddress, null, null, false);
        }

        /// <summary>
        /// Verifies that the adapter's SignInUser() method
        /// calls the XboxXdk's SignInUser method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestSignInUserCallsXdkSignInUser()
        {
            bool isCorrectFunctionCalled = false;
            this.fakeXboxXdk.SignInUserFunc = (ipAddress, user, password, storePassword) =>
            {
                isCorrectFunctionCalled = true;
                return new XboxUserDefinition(0, null, null, false);
            };

            this.adapter.SignInUser(ConsoleAddress, new XboxUserDefinition(0, null, null, false), null, false);

            Assert.IsTrue(isCorrectFunctionCalled, "The adapter's SignInUser function failed to call the XboxXdk's SignInUser function.");
        }

        /// <summary>
        /// Verifies that the adapter's SignInUser method correctly handles passing on arguments.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestSignInUserArguments()
        {
            const string ExpectedPassword = "TestPassword";

            XboxUserDefinition expectedUser = new XboxUserDefinition(1, "TestEmailAddress@test.test", TestGamerTag, false);
            bool expectedStorePassword = false;

            this.VerifyThrows<ArgumentNullException>(() => this.adapter.SignInUser(null, expectedUser, ExpectedPassword, expectedStorePassword));

            this.fakeXboxXdk.SignInUserFunc = (ipAddress, user, password, storePassword) =>
            {
                Assert.AreSame(expectedUser, user, "Adapter did not pass on the expected user.");
                Assert.AreEqual(ExpectedPassword, password, "Adapter did not pass on expected password.");
                Assert.AreEqual(expectedStorePassword, storePassword, "Adapter did not pass on expected store password value.");
                return new XboxUserDefinition(0, null, null, false);
            };

            this.adapter.SignInUser(ConsoleAddress, expectedUser, ExpectedPassword, expectedStorePassword);

            expectedStorePassword = true;

            this.adapter.SignInUser(ConsoleAddress, expectedUser, ExpectedPassword, expectedStorePassword);
        }

        /// <summary>
        /// Verifies that an ArgumentNullException is thrown if the SignInUser()
        /// method with null user.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSignInUserThrowsArgumentNullException()
        {
            this.adapter.SignInUser(ConsoleAddress, null, null, false);
        }

        /// <summary>
        /// Verifies that if the XDK's SignInUser method throws a COMException, then the
        /// XboxConsoleAdapter will wrap that into an XboxConsoleException with the COMException as its inner exception.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestSignInUserTurnsSpecificComExceptionIntoXboxSignInExceptions()
        {
            Action adapterCall = () => { this.adapter.SignInUser(ConsoleAddress, new XboxUserDefinition(0, TestEmailAddressString, TestGamerTag, false, false, TestXuidString), null, false); };

            COMException invalidEmailException = new COMException(null, InvalidEmailErrorCode);
            this.fakeXboxXdk.SignInUserFunc = (ipAddress, userDef, password, savePassword) => { throw invalidEmailException; };
            this.VerifyFunctionHandlesSpecificComExceptions(
                invalidEmailException,
                typeof(XboxConsoleException),
                adapterCall);

            COMException passwordNotStoredException = new COMException(null, PasswordNotStoredErrorCode);
            this.fakeXboxXdk.SignInUserFunc = (ipAddress, userDef, password, savePassword) => { throw passwordNotStoredException; };
            this.VerifyFunctionHandlesSpecificComExceptions(
                passwordNotStoredException,
                typeof(XboxConsoleException),
                adapterCall);

            COMException invalidPasswordException = new COMException(null, InvalidPasswordErrorCode);
            this.fakeXboxXdk.SignInUserFunc = (ipAddress, userDef, password, savePassword) => { throw invalidPasswordException; };
            this.VerifyFunctionHandlesSpecificComExceptions(
                invalidPasswordException,
                typeof(XboxConsoleException),
                adapterCall);

            COMException signedInElsewhereException = new COMException(null, SignedInElsewhereErrorCode);
            this.fakeXboxXdk.SignInUserFunc = (ipAddress, userDef, password, savePassword) => { throw signedInElsewhereException; };
            this.VerifyFunctionHandlesSpecificComExceptions(
                signedInElsewhereException,
                typeof(XboxConsoleException),
                adapterCall);
        }

        /// <summary>
        /// Verifies that if the XDK's AddLocalUsersToParty method throws a COMException, then the
        /// XboxConsoleAdapter will wrap that into an XboxConsoleException with the COMException as its inner exception.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestAddLocalUsersToPartyTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.AddLocalUsersToPartyFunc = (ipAddress, titleId, actingUserXuid, localUserXuidsToAdd) =>
            {
                throw new COMException();
            };

            try
            {
                this.adapter.AddLocalUsersToParty(ConsoleAddress, TestTitleId, string.Empty, new string[0]);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The adapter AddLocalUsersToParty method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The adapter AddLocalUsersToParty method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that an ObjectDisposedException is thrown if the AddLocalUsersToParty()
        /// method is called after the object has been disposed.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestAddLocalUsersToPartyThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.AddLocalUsersToParty(ConsoleAddress, TestTitleId, string.Empty, new string[0]);
        }

        /// <summary>
        /// Verifies that the adapter's AddLocalUsersToParty() method
        /// calls the XboxXdk's AddLocalUsersToParty method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestAddLocalUsersToPartyCallsXdkAddLocalUsersToParty()
        {
            bool isCorrectFunctionCalled = false;
            this.fakeXboxXdk.AddLocalUsersToPartyFunc = (ipAddress, titleId, actingUserXuid, localUserXuids) =>
            {
                isCorrectFunctionCalled = true;
            };

            this.adapter.AddLocalUsersToParty(ConsoleAddress, TestTitleId, string.Empty, new string[0]);

            Assert.IsTrue(isCorrectFunctionCalled, "The adapter's AddLocalUsersToParty function failed to call the XboxXdk's AddLocalUsersToParty function.");
        }

        /// <summary>
        /// Verifies that the adapter's AddLocalUsersToParty method correctly handles passing on arguments.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestAddLocalUsersToPartyArguments()
        {
            this.fakeXboxXdk.AddLocalUsersToPartyFunc = (ipAddress, titleId, actingUserXuid, localUserXuids) =>
            {
                Assert.AreEqual(ConsoleAddress, ipAddress, "Adapter did not pass on the expected Console IP.");
                Assert.AreEqual(TestTitleId, titleId, "Adapter did not pass on expected Title ID.");
                Assert.AreEqual(TestXuidString, actingUserXuid, "Adapter did not pass on expected acting user XUID.");
                Assert.AreEqual(1, localUserXuids.Length, "Adapter did not pass on expected number of local user XUIDs.");
                Assert.AreEqual(TestXuidString, localUserXuids[0], "Adapter did not pass on expected local user XUID.");
            };

            this.adapter.AddLocalUsersToParty(ConsoleAddress, TestTitleId, TestXuidString, new string[] { TestXuidString });
        }

        /// <summary>
        /// Verifies that an ArgumentNullException is thrown if the AddLocalUsersToParty()
        /// method with null console.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestAddLocalUsersToPartyThrowsArgumentNullExceptionOnConsole()
        {
            this.adapter.AddLocalUsersToParty(null, TestTitleId, TestXuidString, new string[1] { TestXuidString });
        }

        /// <summary>
        /// Verifies that if the XDK's AddLocalUsersToParty method throws a COMException, then the
        /// XboxConsoleAdapter will wrap that into an XboxConsoleException with the COMException as its inner exception.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestAddLocalUsersToPartyTurnsSpecificComExceptionsIntoXboxConsoleExceptions()
        {
            Action adapterCall = () => { this.adapter.AddLocalUsersToParty(ConsoleAddress, TestTitleId, TestXuidString, new string[] { TestXuidString }); };

            COMException invalidXuidException = new COMException(null, InvalidXuidErrorCode);
            this.fakeXboxXdk.AddLocalUsersToPartyFunc = (ipAddress, titleId, actingUserXuid, addXuids) => { throw invalidXuidException; };
            this.VerifyFunctionHandlesSpecificComExceptions(
                invalidXuidException,
                typeof(InvalidXuidException),
                adapterCall);
        }

        /// <summary>
        /// Verifies that if the XDK's InviteToParty method throws a COMException, then the
        /// XboxConsoleAdapter will wrap that into an XboxConsoleException with the COMException as its inner exception.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestInviteToPartyTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.InviteToPartyFunc = (ipAddress, titleId, actingUserXuid, remoteUserXuidsToInvite) =>
            {
                throw new COMException();
            };

            try
            {
                this.adapter.InviteToParty(ConsoleAddress, TestTitleId, string.Empty, new string[0]);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The adapter InviteToParty method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The adapter InviteToParty method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that an ObjectDisposedException is thrown if the InviteToParty()
        /// method is called after the object has been disposed.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestInviteToPartyThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.InviteToParty(ConsoleAddress, TestTitleId, string.Empty, new string[0]);
        }

        /// <summary>
        /// Verifies that the adapter's InviteToParty() method
        /// calls the XboxXdk's InviteToParty method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestInviteToPartyCallsXdkInviteToParty()
        {
            bool isCorrectFunctionCalled = false;
            this.fakeXboxXdk.InviteToPartyFunc = (ipAddress, titleId, actingUserXuid, remoteUserXuids) =>
            {
                isCorrectFunctionCalled = true;
            };

            this.adapter.InviteToParty(ConsoleAddress, TestTitleId, string.Empty, new string[0]);

            Assert.IsTrue(isCorrectFunctionCalled, "The adapter's InviteToParty function failed to call the XboxXdk's InviteToParty function.");
        }

        /// <summary>
        /// Verifies that the adapter's InviteToParty method correctly handles passing on arguments.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestInviteToPartyArguments()
        {
            this.fakeXboxXdk.InviteToPartyFunc = (ipAddress, titleId, actingUserXuid, remoteUserXuids) =>
            {
                Assert.AreEqual(ConsoleAddress, ipAddress, "Adapter did not pass on the expected Console IP.");
                Assert.AreEqual(TestTitleId, titleId, "Adapter did not pass on expected Title ID.");
                Assert.AreEqual(TestXuidString, actingUserXuid, "Adapter did not pass on expected acting user XUID.");
                Assert.AreEqual(1, remoteUserXuids.Length, "Adapter did not pass on expected number of local user XUIDs.");
                Assert.AreEqual(TestXuidString, remoteUserXuids[0], "Adapter did not pass on expected local user XUID.");
            };

            this.adapter.InviteToParty(ConsoleAddress, TestTitleId, TestXuidString, new string[] { TestXuidString });
        }

        /// <summary>
        /// Verifies that an ArgumentNullException is thrown if the InviteToParty()
        /// method with null console.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestInviteToPartyThrowsArgumentNullExceptionOnConsole()
        {
            this.adapter.InviteToParty(null, TestTitleId, TestXuidString, new string[1] { TestXuidString });
        }

        /// <summary>
        /// Verifies that if the XDK's InviteToParty method throws a COMException, then the
        /// XboxConsoleAdapter will wrap that into an XboxConsoleException with the COMException as its inner exception.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestInviteToPartyTurnsSpecificComExceptionsIntoXboxConsoleExceptions()
        {
            Action adapterCall = () => { this.adapter.InviteToParty(ConsoleAddress, TestTitleId, TestXuidString, new string[] { TestXuidString }); };

            COMException invalidXuidException = new COMException(null, InvalidXuidErrorCode);
            this.fakeXboxXdk.InviteToPartyFunc = (ipAddress, titleId, actingUserXuid, partyId) => { throw invalidXuidException; };
            this.VerifyFunctionHandlesSpecificComExceptions(
                invalidXuidException,
                typeof(InvalidXuidException),
                adapterCall);
        }

        /// <summary>
        /// Verifies that if the XDK's RemoveLocalUsersFromParty method throws a COMException, then the
        /// XboxConsoleAdapter will wrap that into an XboxConsoleException with the COMException as its inner exception.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestRemoveLocalUsersFromPartyTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.RemoveLocalUsersFromPartyFunc = (ipAddress, titleId, remoteUserXuidsToInvite) =>
            {
                throw new COMException();
            };

            try
            {
                this.adapter.RemoveLocalUsersFromParty(ConsoleAddress, TestTitleId, new string[0]);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The adapter RemoveLocalUsersFromParty method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The adapter RemoveLocalUsersFromParty method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that an ObjectDisposedException is thrown if the RemoveLocalUsersFromParty()
        /// method is called after the object has been disposed.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestRemoveLocalUsersFromPartyThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.RemoveLocalUsersFromParty(ConsoleAddress, TestTitleId, new string[0]);
        }

        /// <summary>
        /// Verifies that the adapter's RemoveLocalUsersFromParty() method
        /// calls the XboxXdk's RemoveLocalUsersFromParty method and a null argument exception is thrown if Xbox IP is null.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestRemoveLocalUsersFromPartyCallsXdkRemoveLocalUsersFromParty()
        {
            bool isCorrectFunctionCalled = false;
            this.fakeXboxXdk.RemoveLocalUsersFromPartyFunc = (ipAddress, titleId, remoteUserXuids) =>
            {
                isCorrectFunctionCalled = true;
            };

            this.adapter.RemoveLocalUsersFromParty(ConsoleAddress, TestTitleId, new string[0]);

            Assert.IsTrue(isCorrectFunctionCalled, "The adapter's RemoveLocalUsersFromParty function failed to call the XboxXdk's RemoveLocalUsersFromParty function.");

            this.VerifyThrows<ArgumentNullException>(() => this.adapter.RemoveLocalUsersFromParty(null, TestTitleId, new string[0]));
        }

        /// <summary>
        /// Verifies that the adapter's RemoveLocalUsersFromParty method correctly handles passing on arguments.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestRemoveLocalUsersFromPartyArguments()
        {
            this.fakeXboxXdk.RemoveLocalUsersFromPartyFunc = (ipAddress, titleId, removeUserXuids) =>
            {
                Assert.AreEqual(ConsoleAddress, ipAddress, "Adapter did not pass on the expected Console IP.");
                Assert.AreEqual(TestTitleId, titleId, "Adapter did not pass on expected Title ID.");
                Assert.AreEqual(1, removeUserXuids.Length, "Adapter did not pass on expected number of local user XUIDs.");
                Assert.AreEqual(TestXuidString, removeUserXuids[0], "Adapter did not pass on expected local user XUID.");
            };

            this.adapter.RemoveLocalUsersFromParty(ConsoleAddress, TestTitleId, new string[] { TestXuidString });
        }

        /// <summary>
        /// Verifies that an ArgumentNullException is thrown if the RemoveLocalUsersFromParty()
        /// method with null console.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestRemoveLocalUsersFromPartyThrowsArgumentNullExceptionOnConsole()
        {
            this.adapter.RemoveLocalUsersFromParty(null, TestTitleId, new string[1] { TestXuidString });
        }

        /// <summary>
        /// Verifies that if the XDK's RemoveLocalUsersFromParty method throws a COMException, then the
        /// XboxConsoleAdapter will wrap that into an XboxConsoleException with the COMException as its inner exception.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestRemoveLocalUsersFromPartyTurnsSpecificComExceptionsIntoXboxConsoleExceptions()
        {
            Action adapterCall = () => { this.adapter.RemoveLocalUsersFromParty(ConsoleAddress, TestTitleId, new string[] { TestXuidString }); };

            COMException invalidXuidException = new COMException(null, InvalidXuidErrorCode);
            this.fakeXboxXdk.RemoveLocalUsersFromPartyFunc = (ipAddress, titleId, removeXuids) => { throw invalidXuidException; };
            this.VerifyFunctionHandlesSpecificComExceptions(
                invalidXuidException,
                typeof(InvalidXuidException),
                adapterCall);

            COMException partyDoesntExistException = new COMException(null, NoLocalUsersErrorCode);
            this.fakeXboxXdk.RemoveLocalUsersFromPartyFunc = (ipAddress, titleId, removeXuids) => { throw partyDoesntExistException; };
            this.VerifyFunctionHandlesSpecificComExceptions(
                partyDoesntExistException,
                typeof(NoLocalSignedInUsersInPartyException),
                adapterCall);
        }

        /// <summary>
        /// Verifies that if the XDK's GetPartyId method throws a COMException, then the
        /// XboxConsoleAdapter will wrap that into an XboxConsoleException with the COMException as its inner exception.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestGetPartyIdTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.GetPartyIdFunc = (ipAddress, titleId) =>
            {
                throw new COMException();
            };

            try
            {
                this.adapter.GetPartyId(ConsoleAddress, 0);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The adapter GetPartyId method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The adapter GetPartyId method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that an ObjectDisposedException is thrown if the GetPartyId()
        /// method is called after the object has been disposed.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestGetPartyIdThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.GetPartyId(ConsoleAddress, 0);
        }

        /// <summary>
        /// Verifies that the adapter's GetPartyId() method
        /// calls the XboxXdk's GetPartyId method and a null argument exception is thrown if Xbox IP is null.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestGetPartyIdCallsXdkGetPartyId()
        {
            bool isCorrectFunctionCalled = false;
            this.fakeXboxXdk.GetPartyIdFunc = (ipAddress, titleId) =>
            {
                isCorrectFunctionCalled = true;
                return string.Empty;
            };

            this.adapter.GetPartyId(ConsoleAddress, 0);

            Assert.IsTrue(isCorrectFunctionCalled, "The adapter's GetPartyId function failed to call the XboxXdk's GetPartyId function.");
        }

        /// <summary>
        /// Verifies that the adapter's GetPartyId method correctly handles passing on arguments.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestGetPartyIdArguments()
        {
            this.fakeXboxXdk.GetPartyIdFunc = (ipAddress, titleId) =>
            {
                Assert.AreEqual(ConsoleAddress, ipAddress, "Adapter did not pass on the expected Console IP.");
                Assert.AreEqual(TestTitleId, titleId, "Adapter did not pass on expected Title ID.");
                return TestPartyId;
            };

            string returnPartyId = this.adapter.GetPartyId(ConsoleAddress, TestTitleId);

            Assert.AreEqual(TestPartyId, returnPartyId, "Adapter did not return the expected Party ID.");
        }

        /// <summary>
        /// Verifies that an ArgumentNullException is thrown if the GetPartyId()
        /// method with null console.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetPartyIdThrowsArgumentNullExceptionOnConsole()
        {
            this.adapter.GetPartyId(null, 0);
        }

        /// <summary>
        /// Verifies that if the XDK's GetPartyId method throws a COMException, then the
        /// XboxConsoleAdapter will wrap that into an XboxConsoleException with the COMException as its inner exception.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestGetPartyIdTurnsSpecificComExceptionsIntoXboxConsoleExceptions()
        {
            Action adapterCall = () => { this.adapter.GetPartyId(ConsoleAddress, TestTitleId); };

            COMException partyDoesntExistException = new COMException(null, NoLocalUsersErrorCode);
            this.fakeXboxXdk.GetPartyIdFunc = (ipAddress, titleId) => { throw partyDoesntExistException; };
            this.VerifyFunctionHandlesSpecificComExceptions(
                partyDoesntExistException,
                typeof(NoLocalSignedInUsersInPartyException),
                adapterCall);
        }

        /// <summary>
        /// Verifies that if the XDK's GetPartyMembers method throws a COMException, then the
        /// XboxConsoleAdapter will wrap that into an XboxConsoleException with the COMException as its inner exception.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestGetPartyMembersTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.GetPartyMembersFunc = (ipAddress, titleId) =>
            {
                throw new COMException();
            };

            try
            {
                this.adapter.GetPartyMembers(ConsoleAddress, 0);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The adapter GetPartyMembers method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The adapter GetPartyMembers method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that an ObjectDisposedException is thrown if the GetPartyMembers()
        /// method is called after the object has been disposed.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestGetPartyMembersThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.GetPartyMembers(ConsoleAddress, 0);
        }

        /// <summary>
        /// Verifies that the adapter's GetPartyMembers() method
        /// calls the XboxXdk's GetPartyMembers method and a null argument exception is thrown if Xbox IP is null.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestGetPartyMembersCallsXdkGetPartyMembers()
        {
            bool isCorrectFunctionCalled = false;
            this.fakeXboxXdk.GetPartyMembersFunc = (ipAddress, titleId) =>
            {
                isCorrectFunctionCalled = true;
                return new string[0];
            };

            this.adapter.GetPartyMembers(ConsoleAddress, 0);

            Assert.IsTrue(isCorrectFunctionCalled, "The adapter's GetPartyMembers function failed to call the XboxXdk's GetPartyMembers function.");
        }

        /// <summary>
        /// Verifies that the adapter's GetPartyMembers method correctly handles passing on arguments.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestGetPartyMembersArguments()
        {
            this.fakeXboxXdk.GetPartyMembersFunc = (ipAddress, titleId) =>
            {
                Assert.AreEqual(ConsoleAddress, ipAddress, "Adapter did not pass on the expected Console IP.");
                Assert.AreEqual(TestTitleId, titleId, "Adapter did not pass on expected Title ID.");
                return new string[] { TestXuidString };
            };

            string[] returnMembers = this.adapter.GetPartyMembers(ConsoleAddress, TestTitleId);

            Assert.AreEqual(1, returnMembers.Length, "Adapter did not return the correct number of party members.");
            Assert.AreEqual(TestXuidString, returnMembers[0], "Adapter did not return the right party members.");
        }

        /// <summary>
        /// Verifies that an ArgumentNullException is thrown if the GetPartyMembers()
        /// method with null console.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetPartyMembersThrowsArgumentNullExceptionOnConsole()
        {
            this.adapter.GetPartyMembers(null, 0);
        }

        /// <summary>
        /// Verifies that if the XDK's GetPartyMembers method throws a COMException, then the
        /// XboxConsoleAdapter will wrap that into an XboxConsoleException with the COMException as its inner exception.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestGetPartyMembersTurnsSpecificComExceptionsIntoXboxConsoleExceptions()
        {
            Action adapterCall = () => { this.adapter.GetPartyMembers(ConsoleAddress, TestTitleId); };

            COMException partyDoesntExistException = new COMException(null, NoLocalUsersErrorCode);
            this.fakeXboxXdk.GetPartyMembersFunc = (ipAddress, titleId) => { throw partyDoesntExistException; };
            this.VerifyFunctionHandlesSpecificComExceptions(
                partyDoesntExistException,
                typeof(NoLocalSignedInUsersInPartyException),
                adapterCall);
        }

        /// <summary>
        /// Verifies that if the XDK's AcceptInviteToParty method throws a COMException, then the
        /// XboxConsoleAdapter will wrap that into an XboxConsoleException with the COMException as its inner exception.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestAcceptInviteToPartyTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.AcceptInviteToPartyFunc = (ipAddress, actingUserXuid, partyId) =>
            {
                throw new COMException();
            };

            try
            {
                this.adapter.AcceptInviteToParty(ConsoleAddress, string.Empty, string.Empty);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The adapter AcceptInviteToParty method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The adapter AcceptInviteToParty method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that an ObjectDisposedException is thrown if the AcceptInviteToParty()
        /// method is called after the object has been disposed.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestAcceptInviteToPartyThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.AcceptInviteToParty(ConsoleAddress, string.Empty, string.Empty);
        }

        /// <summary>
        /// Verifies that the adapter's AcceptInviteToParty() method
        /// calls the XboxXdk's AcceptInviteToParty method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestAcceptInviteToPartyCallsXdkAcceptInviteToParty()
        {
            bool isCorrectFunctionCalled = false;
            this.fakeXboxXdk.AcceptInviteToPartyFunc = (ipAddress, actingUserXuid, partyId) =>
            {
                isCorrectFunctionCalled = true;
            };

            this.adapter.AcceptInviteToParty(ConsoleAddress, string.Empty, string.Empty);

            Assert.IsTrue(isCorrectFunctionCalled, "The adapter's AcceptInviteToParty function failed to call the XboxXdk's AcceptInviteToParty function.");
        }

        /// <summary>
        /// Verifies that the adapter's AcceptInviteToParty method correctly handles passing on arguments.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestAcceptInviteToPartyArguments()
        {
            this.fakeXboxXdk.AcceptInviteToPartyFunc = (ipAddress, actingUserXuid, partyId) =>
            {
                Assert.AreEqual(ConsoleAddress, ipAddress, "Adapter did not pass on the expected Console IP.");
                Assert.AreEqual(TestXuidString, actingUserXuid, "Adapter did not pass on expected Acting User XUID.");
                Assert.AreEqual(TestPartyId, partyId, "Adapter did not pass on expected Party ID.");
            };

            this.adapter.AcceptInviteToParty(ConsoleAddress, TestXuidString, TestPartyId);
        }

        /// <summary>
        /// Verifies that an ArgumentNullException is thrown if the AcceptInviteToParty()
        /// method with null console.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestAcceptInviteToPartyThrowsArgumentNullExceptionOnConsole()
        {
            this.adapter.AcceptInviteToParty(null, TestXuidString, TestXuidString);
        }

        /// <summary>
        /// Verifies that if the XDK's AcceptInviteToParty method throws a COMException, then the
        /// XboxConsoleAdapter will wrap that into an XboxConsoleException with the COMException as its inner exception.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestAcceptInviteToPartyTurnsSpecificComExceptionsIntoXboxConsoleExceptions()
        {
            Action adapterCall = () => { this.adapter.AcceptInviteToParty(ConsoleAddress, TestXuidString, TestPartyId); };

            COMException invalidXuidException = new COMException(null, InvalidXuidErrorCode);
            this.fakeXboxXdk.AcceptInviteToPartyFunc = (ipAddress, actingUserXuid, partyId) => { throw invalidXuidException; };
            this.VerifyFunctionHandlesSpecificComExceptions(
                invalidXuidException,
                typeof(InvalidXuidException),
                adapterCall);

            COMException invalidPartyInviteException = new COMException(null, InvalidPartyInviteErrorCode);
            this.fakeXboxXdk.AcceptInviteToPartyFunc = (ipAddress, actingUserXuid, partyId) => { throw invalidPartyInviteException; };
            this.VerifyFunctionHandlesSpecificComExceptions(
                invalidPartyInviteException,
                typeof(InvalidPartyInviteException),
                adapterCall);
        }

        /// <summary>
        /// Verifies that if the XDK's DeclineInviteToParty method throws a COMException, then the
        /// XboxConsoleAdapter will wrap that into an XboxConsoleException with the COMException as its inner exception.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestDeclineInviteToPartyTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.DeclineInviteToPartyFunc = (ipAddress, actingUserXuid, partyId) =>
            {
                throw new COMException();
            };

            try
            {
                this.adapter.DeclineInviteToParty(ConsoleAddress, string.Empty, string.Empty);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The adapter DeclineInviteToParty method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The adapter DeclineInviteToParty method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that an ObjectDisposedException is thrown if the DeclineInviteToParty()
        /// method is called after the object has been disposed.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestDeclineInviteToPartyThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.DeclineInviteToParty(ConsoleAddress, string.Empty, string.Empty);
        }

        /// <summary>
        /// Verifies that the adapter's DeclineInviteToParty() method
        /// calls the XboxXdk's DeclineInviteToParty method and a null argument exception is thrown if Xbox IP is null.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestDeclineInviteToPartyCallsXdkDeclineInviteToParty()
        {
            bool isCorrectFunctionCalled = false;
            this.fakeXboxXdk.DeclineInviteToPartyFunc = (ipAddress, actingUserXuid, partyId) =>
            {
                isCorrectFunctionCalled = true;
            };

            this.adapter.DeclineInviteToParty(ConsoleAddress, string.Empty, string.Empty);

            Assert.IsTrue(isCorrectFunctionCalled, "The adapter's DeclineInviteToParty function failed to call the XboxXdk's DeclineInviteToParty function.");
        }

        /// <summary>
        /// Verifies that the adapter's DeclineInviteToParty method correctly handles passing on arguments.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestDeclineInviteToPartyArguments()
        {
            this.fakeXboxXdk.DeclineInviteToPartyFunc = (ipAddress, actingUserXuid, partyId) =>
            {
                Assert.AreEqual(ConsoleAddress, ipAddress, "Adapter did not pass on the expected Console IP.");
                Assert.AreEqual(TestXuidString, actingUserXuid, "Adapter did not pass on expected Acting User XUID.");
                Assert.AreEqual(TestPartyId, partyId, "Adapter did not pass on expected Party ID.");
            };

            this.adapter.DeclineInviteToParty(ConsoleAddress, TestXuidString, TestPartyId);
        }

        /// <summary>
        /// Verifies that an ArgumentNullException is thrown if the DeclineInviteToParty()
        /// method with null console.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestDeclineInviteToPartyThrowsArgumentNullExceptionOnConsole()
        {
            this.adapter.DeclineInviteToParty(null, TestXuidString, TestXuidString);
        }

        /// <summary>
        /// Verifies that if the XDK's DeclineInviteToParty method throws a COMException, then the
        /// XboxConsoleAdapter will wrap that into an XboxConsoleException with the COMException as its inner exception.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterUserTestCategory)]
        public void TestDeclineInviteToPartyTurnsSpecificComExceptionsIntoXboxConsoleExceptions()
        {
            Action adapterCall = () => { this.adapter.DeclineInviteToParty(ConsoleAddress, TestXuidString, TestPartyId); };

            COMException invalidXuidException = new COMException(null, InvalidXuidErrorCode);
            this.fakeXboxXdk.DeclineInviteToPartyFunc = (ipAddress, actingUserXuid, partyId) => { throw invalidXuidException; };
            this.VerifyFunctionHandlesSpecificComExceptions(
                invalidXuidException,
                typeof(InvalidXuidException),
                adapterCall);

            COMException invalidPartyInviteException = new COMException(null, InvalidPartyInviteErrorCode);
            this.fakeXboxXdk.DeclineInviteToPartyFunc = (ipAddress, actingUserXuid, partyId) => { throw invalidPartyInviteException; };
            this.VerifyFunctionHandlesSpecificComExceptions(
                invalidPartyInviteException,
                typeof(InvalidPartyInviteException),
                adapterCall);
        }

        /// <summary>
        /// Verifies that DeclineInviteToParty correctly recognizes expected COMExceptions
        /// and wraps them in a XboxConsoleException-derived exception.
        /// </summary>
        /// <param name="expectedCOMException">The COMException to be thrown and caught.</param>
        /// <param name="expectedType">Expected type of the compound exception.</param>
        /// <param name="callAction">Action called to cause the exception to be thrown.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        private void VerifyFunctionHandlesSpecificComExceptions(COMException expectedCOMException, Type expectedType, Action callAction)
        {
            try
            {
                if (callAction != null)
                {
                    callAction();
                }
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, expectedType, "The adapter method did not convert an expected COMException into an XboxConsole exception.");
                Assert.AreSame(
                    expectedCOMException,
                    ex.InnerException,
                    "The adapter method did not include the expected COMException with error code {0} as the inner exception of the XboxConsoleException that it threw.",
                    expectedCOMException.ErrorCode);
            }
        }
    }
}