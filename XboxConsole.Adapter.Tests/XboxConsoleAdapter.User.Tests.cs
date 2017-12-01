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

        private const int InvalidEmailErrorCode = unchecked((int)0x80048823);
        private const int PasswordNotStoredErrorCode = unchecked((int)0x8004882E);
        private const int InvalidPasswordErrorCode = unchecked((int)0x80048821);
        private const int SignedInElsewhereErrorCode = unchecked((int)0x8015DC16);

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
            var expectedDefinition = new XboxUserDefinition(0, "Test email address", "Test gamertag", true);
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
            XboxUserDefinition expectedUser = new XboxUserDefinition(1, TestEmailAddressString, "TestGamerTag", false);

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
            XboxUserDefinition expectedUser = new XboxUserDefinition(1, TestEmailAddressString, "TestGamerTag", false);

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

            XboxUserDefinition expectedUser = new XboxUserDefinition(1, "TestEmailAddress@test.test", "TestGamerTag", false);
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
            COMException invalidEmailException = new COMException(null, InvalidEmailErrorCode);
            COMException passwordNotStoredException = new COMException(null, PasswordNotStoredErrorCode);
            COMException invalidPasswordException = new COMException(null, InvalidPasswordErrorCode);
            COMException signedInElsewhereException = new COMException(null, SignedInElsewhereErrorCode);

            this.VerifySignInHandlesSpecificComExceptions(invalidEmailException);
            this.VerifySignInHandlesSpecificComExceptions(passwordNotStoredException);
            this.VerifySignInHandlesSpecificComExceptions(invalidPasswordException);
            this.VerifySignInHandlesSpecificComExceptions(signedInElsewhereException);
        }

        /// <summary>
        /// Verifies that SignInUser correctly recognizes expected COMExceptions
        ///  and wraps them in a XboxSignInException.
        /// </summary>
        /// <param name="expectedCOMException">The COMException to be thrown and caught.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        private void VerifySignInHandlesSpecificComExceptions(COMException expectedCOMException)
        {
            this.fakeXboxXdk.SignInUserFunc = (ipAddress, user, password, storePassword) =>
            {
                throw expectedCOMException;
            };

            try
            {
                this.adapter.SignInUser(ConsoleAddress, new XboxUserDefinition(0, null, null, false), null, false);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxSignInException), "The adapter SignInUser method did not convert an expected COMException into an XboxSignInException.");
                Assert.AreSame(
                    expectedCOMException,
                    ex.InnerException,
                    "The adapter SignInUser method did not include the expected COMException with error code {0} as the inner exception of the XboxSignInException that it threw.",
                    expectedCOMException.ErrorCode);
            }
        }
    }
}
