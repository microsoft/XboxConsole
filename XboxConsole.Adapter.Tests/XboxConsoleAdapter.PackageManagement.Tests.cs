//------------------------------------------------------------------------------
// <copyright file="XboxConsoleAdapter.PackageManagement.Tests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Adapter.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using Microsoft.Internal.GamesTest.Xbox.Deployment;
    using Microsoft.Internal.GamesTest.Xbox.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Represents tests for the XboxConsole adapter types.
    /// </summary>
    public partial class XboxConsoleAdapterTests
    {
        private const string AdapterPackageManagementTestCategory = "Adapter.PackageManagement";
        private const string PackagesParsingExceptionMessage = "Failed to parse installed packages string.";

        private const string PackageFamilyName = "NuiView.ERA_8wekyb3d8bbwe";
        private const string PackageFullName = "NuiView.ERA_1.0.0.0_neutral__8wekyb3d8bbwe";
        private const string ApplicationId = "NuiView.ERA";
        private const string Aumid = PackageFamilyName + "!" + ApplicationId;
        private const string TestArguments = "firstargument secondArgument";
        private static readonly string[] Aumids = { Aumid };

        /// <summary>
        /// Verifies that if the XDK's GetInstalledPackages method throws a COMException, then the
        /// XboxConsoleAdapter will wrap that into an XboxConsoleException with the COMException as its inner exception.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        public void TestInstalledPackagesTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.GetInstalledPackagesFunc = _ =>
            {
                throw new COMException();
            };

#pragma warning disable 168 // Unused local
            try
            {
                var notUsed = this.adapter.GetInstalledPackages(ConsoleAddress);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The XboxConsole InstalledApplications property did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The XboxConsole InstalledApplications property did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
#pragma warning restore 168
        }

        /// <summary>
        /// Verifies that an ObjectDisposedException is thrown if the GetInstalledPackages()
        /// method is called after the object has been disposed.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestGetInstalledPackagesThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
#pragma warning disable 168
            var notUsed = this.adapter.GetInstalledPackages(ConsoleAddress);
#pragma warning restore 168
        }

        /// <summary>
        /// Verifies that the adapter's GetInstalledApplications() method
        /// calls the XboxXdk's GetInstalledApplications method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        public void TestGetInstalledApplicationsCallsXdkGetInstalledApplications()
        {
            bool isCorrectFunctionCalled = false;
            this.fakeXboxXdk.GetInstalledPackagesFunc = ipAddress =>
            {
                isCorrectFunctionCalled = true;
                return string.Empty;
            };

            // we don't care about parsing errors in this test (caused by empty string):
            try
            {
                this.adapter.GetInstalledPackages(ConsoleAddress);
            }
            catch (XboxConsoleException)
            {
            }

            Assert.IsTrue(isCorrectFunctionCalled, "The adapter's GetInstalledPackages function failed to call the XboxXdk's GetInstalledPackages function.");

            this.VerifyThrows<ArgumentNullException>(() => this.adapter.GetInstalledPackages(null));
        }

        /// <summary>
        /// Verifies that the adapter throws an exception if the text it receives from the XDK is null.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        public void TestGetInstalledPackagesThrowsWithNullOutput()
        {
            this.fakeXboxXdk.GetInstalledPackagesFunc = ipAddress => null;

            try
            {
                IEnumerable<XboxPackageDefinition> packages = this.adapter.GetInstalledPackages(ConsoleAddress);
                Assert.Fail("Null received from the XDK didn't throw an expected exception.");
            }
            catch (XboxConsoleException ex)
            {
                Assert.AreEqual(PackagesParsingExceptionMessage, ex.Message, "Null received from the XDK didn't throw an expected exception.");
            }
        }

        /// <summary>
        /// Verifies that the adapter throws an exception if the text it receives from the XDK is an empty string.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        public void TestGetInstalledPackagesThrowsWithEmptyStringOutput()
        {
            this.fakeXboxXdk.GetInstalledPackagesFunc = ipAddress => string.Empty;

            try
            {
                IEnumerable<XboxPackageDefinition> packages = this.adapter.GetInstalledPackages(ConsoleAddress);
                Assert.Fail("Empty string received from the XDK didn't throw an expected exception.");
            }
            catch (XboxConsoleException ex)
            {
                Assert.AreEqual(PackagesParsingExceptionMessage, ex.Message, "Empty string received from the XDK didn't throw an expected exception.");
            }
        }

        /// <summary>
        /// Verifies that the adapter throws an exception if the text it receives from the XDK is not in the expected format.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        public void TestGetInstalledPackagesThrowsWithImproperlyFormattedOutput()
        {
            this.fakeXboxXdk.GetInstalledPackagesFunc = ipAddress => "ImproperlyFormattedText";

            try
            {
                IEnumerable<XboxPackageDefinition> packages = this.adapter.GetInstalledPackages(ConsoleAddress);
                Assert.Fail("Incorrectly formatted text received from the XDK didn't throw an expected exception.");
            }
            catch (XboxConsoleException ex)
            {
                Assert.AreEqual(PackagesParsingExceptionMessage, ex.Message, "Incorrectly formatted text received from the XDK didn't throw an expected exception.");
            }
        }

        /// <summary>
        /// Verifies that the adapter correctly parses a valid string from the XDK.
        /// </summary>
        /// <remarks>
        /// This test changed significantly in transition from July 2013 XDK to August 2013 XDK. 
        /// The bug 698135 was filed to find a solution to be able to retain both old and new versions of unit tests when such a breaking change occurs, 
        /// so that the relevant version of a unit test runs when testing a corresponding adapter.</remarks>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        public void TestGetInstalledPackages()
        {
            this.fakeXboxXdk.GetInstalledPackagesFunc = ipAddress =>
                {
                    return "{\"Packages\":["
                    + "{\"FullName\":\"" + PackageFullName + "\",\"Applications\":[{\"Aumid\":\"" + Aumid + "\"}]},"
                    + "{\"FullName\":\"" + PackageFullName + "\",\"Applications\":[{\"Aumid\":\"" + Aumid + "\"}]},"
                    + "]}";
                };

            IEnumerable<XboxPackageDefinition> packages = this.adapter.GetInstalledPackages(ConsoleAddress);
            Assert.IsNotNull(packages, "GetInstalledPackages returned a null value.");
            Assert.IsTrue(packages.Count() == 2, "The call to GetInstalled packages should have returned exactly two packages.");

            foreach (XboxPackageDefinition package in packages)
            {
                Assert.AreEqual(PackageFamilyName, package.FamilyName, "Created package had the wrong Package Family Name.");
                Assert.AreEqual(PackageFullName, package.FullName, "Created package had the wrong Package Full Name.");
                Assert.AreEqual(1, package.ApplicationDefinitions.Count(), "Created package had the wrong number of ApplicationDefinitions");
                Assert.AreEqual(ApplicationId, package.ApplicationDefinitions.First().ApplicationId, "Created package had the wrong Application Id.");
            }
        }

        /// <summary>
        /// Verifies that the LaunchApplication(XboxApplicationDefinition) method throws an ObjectDisposedException.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestLaunchApplicationThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.LaunchApplication(ConsoleAddress, this.CreateXboxApplicationDefinition());
        }

        /// <summary>
        /// Verifies that the LaunchApplication(XboxApplicationDefinition) method calls the XDK's LaunchApplication method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        public void TestLaunchApplicationCallsXdkLaunchApplication()
        {
            XboxApplicationDefinition application = this.CreateXboxApplicationDefinition();

            bool isCorrectMethodCalled = false;
            this.fakeXboxXdk.LaunchApplicationAction = (ipAddress, aumid) =>
            {
                isCorrectMethodCalled = true;
                Assert.AreEqual(application.Aumid, aumid, "Aumid passed to XDK was different than value in XboxPackage parameter.");
            };

            this.adapter.LaunchApplication(ConsoleAddress, application);

            Assert.IsTrue(isCorrectMethodCalled, "The Adapter did not call the XDK's LaunchApplication method.");

            this.VerifyThrows<ArgumentNullException>(() => this.adapter.LaunchApplication(null, application));
        }

        /// <summary>
        /// Verifies that the LaunchApplication(XboxApplicationDefinition) method turns COMExceptions into XboxConsoleExceptions.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        public void TestLaunchApplicationTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.LaunchApplicationAction = (_, __) =>
            {
                throw new COMException();
            };

            try
            {
                this.adapter.LaunchApplication(ConsoleAddress, this.CreateXboxApplicationDefinition());
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The XboxConsole LaunchApplication method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The XboxConsole LaunchApplication method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that the LaunchApplication(XboxApplicationDefinition) method throws an ArgumentNullException.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestLaunchApplicationThrowsNullArgumentException()
        {
            this.adapter.LaunchApplication(ConsoleAddress, null);
        }

        /// <summary>
        /// Verifies that the LaunchApplication(XboxApplicationDefinition, arguments) method throws an ObjectDisposedException.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestLaunchApplicationWithArgumentsThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.LaunchApplication(ConsoleAddress, this.CreateXboxApplicationDefinition(), TestArguments);
        }

        /// <summary>
        /// Verifies that the LaunchApplication(XboxApplicationDefinition, arguments) method calls the XDK's LaunchApplication method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        public void TestLaunchApplicationWithArgumentsCallsXdkLaunchApplication()
        {
            XboxApplicationDefinition application = this.CreateXboxApplicationDefinition();

            bool isCorrectMethodCalled = false;
            this.fakeXboxXdk.LaunchApplicationAction = (ipAddress, aumid) =>
            {
                isCorrectMethodCalled = true;

                string[] aumidAndArguments = aumid.Split(' ');
                string[] testArgumentsArray = TestArguments.Split(' ');

                Assert.AreEqual(application.Aumid, aumidAndArguments[0], "Aumid passed to XDK was different than value in XboxPackage parameter.");

                for (int n = 0; n < testArgumentsArray.Length; n++)
                {
                    Assert.AreEqual(testArgumentsArray[n], aumidAndArguments[n + 1], "Argument index {0} was different than value in arguments passed.", n);
                }
            };

            this.adapter.LaunchApplication(ConsoleAddress, application, TestArguments);

            Assert.IsTrue(isCorrectMethodCalled, "The Adapter did not call the XDK's LaunchApplication (with arguments) method.");

            this.VerifyThrows<ArgumentNullException>(() => this.adapter.LaunchApplication(null, application));
        }

        /// <summary>
        /// Verifies that the LaunchApplication(XboxApplicationDefinition, arguments) method turns COMExceptions into XboxConsoleExceptions.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        public void TestLaunchApplicationWithArgumentsTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.LaunchApplicationAction = (_, __) =>
            {
                throw new COMException();
            };

            try
            {
                this.adapter.LaunchApplication(ConsoleAddress, this.CreateXboxApplicationDefinition(), TestArguments);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The XboxConsole LaunchApplication (with arguments) method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The XboxConsole LaunchApplication (with arguments) method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that the LaunchApplication(XboxApplicationDefinition, arguments) method throws an ArgumentNullException.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestLaunchApplicationWithArgumentsThrowsNullArgumentException()
        {
            this.adapter.LaunchApplication(ConsoleAddress, null);
        }

        /// <summary>
        /// Verifies that the TerminatePackage(XboxPackageDefinition) method throws an ArgumentNullException.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestTerminatePackageThrowsNullArgumentException()
        {
            this.adapter.TerminatePackage(ConsoleAddress, null);
        }

        /// <summary>
        /// Verifies that the TerminatePackage(XboxPackageDefinition) method throws an ObjectDisposedException.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestTerminatePackageThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.TerminatePackage(ConsoleAddress, this.CreateXboxPackageDefinition());
        }

        /// <summary>
        /// Verifies that the TerminatePackage(XboxPackageDefinition) method calls the XDK's TerminatePackage method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        public void TestTerminatePackageCallsXdkTerminatePackage()
        {
            XboxPackageDefinition package = new XboxPackageDefinition(PackageFullName, PackageFamilyName, Aumids);

            bool isCorrectMethodCalled = false;
            this.fakeXboxXdk.TerminatePackageAction = (ipAddress, packageFullName) =>
            {
                isCorrectMethodCalled = true;
                Assert.AreEqual(package.FullName, packageFullName, "Package Full Name passed to XDK was different than value in XboxPackage parameter.");
            };

            this.adapter.TerminatePackage(ConsoleAddress, package);

            Assert.IsTrue(isCorrectMethodCalled, "The Adapter did not call the XDK's TerminatePackage method.");

            this.VerifyThrows<ArgumentNullException>(() => this.adapter.TerminatePackage(null, package));
        }

        /// <summary>
        /// Verifies that the TerminatePackage(XboxPackageDefinition) method turns COMExceptions into XboxConsoleExceptions.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        public void TestTerminatePackageTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.TerminatePackageAction = (s, s1) =>
            {
                throw new COMException();
            };

            try
            {
                this.adapter.TerminatePackage(ConsoleAddress, this.CreateXboxPackageDefinition());
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The XboxConsole TerminatePackage method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The XboxConsole TerminatePackage method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that the ResumePackage(XboxPackageDefinition) method throws an ArgumentNullException.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestResumePackageThrowsNullArgumentException()
        {
            this.adapter.ResumePackage(ConsoleAddress, null);
        }

        /// <summary>
        /// Verifies that the ResumePackage(XboxPackageDefinition) method throws an ObjectDisposedException.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestResumePackageThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.ResumePackage(ConsoleAddress, this.CreateXboxPackageDefinition());
        }

        /// <summary>
        /// Verifies that the ResumePackage(XboxPackageDefinition) method calls the XDK's ResumePackage method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        public void TestResumePackageCallsXdkResumePackage()
        {
            XboxPackageDefinition package = this.CreateXboxPackageDefinition();

            bool isCorrectMethodCalled = false;
            this.fakeXboxXdk.ResumePackageAction = (ipAddress, packageFullName) =>
            {
                isCorrectMethodCalled = true;
                Assert.AreEqual(package.FullName, packageFullName, "Package Full Name passed to XDK was different than value in XboxPackage parameter.");
            };

            this.adapter.ResumePackage(ConsoleAddress, package);

            Assert.IsTrue(isCorrectMethodCalled, "The Adapter did not call the XDK's ResumePackage method.");

            this.VerifyThrows<ArgumentNullException>(() => this.adapter.ResumePackage(null, package));
        }

        /// <summary>
        /// Verifies that the ResumePackage(XboxPackageDefinition) method turns COMExceptions into XboxConsoleExceptions.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        public void TestResumePackageTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.ResumePackageAction = (s, s1) =>
            {
                throw new COMException();
            };

            try
            {
                this.adapter.ResumePackage(ConsoleAddress, this.CreateXboxPackageDefinition());
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The XboxConsole ResumePackage method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The XboxConsole ResumePackage method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that the SuspendPackage(XboxPackageDefinition) method throws an ArgumentNullException.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSuspendPackageThrowsNullArgumentException()
        {
            this.adapter.SuspendPackage(ConsoleAddress, null);
        }

        /// <summary>
        /// Verifies that the SuspendPackage(XboxPackageDefinition) method turns COMExceptions into XboxConsoleExceptions.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        public void TestSuspendPackageTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.SuspendPackageAction = (_, __) =>
            {
                throw new COMException();
            };

            try
            {
                this.adapter.SuspendPackage(ConsoleAddress, this.CreateXboxPackageDefinition());
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The XboxConsole SuspendPackage method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The XboxConsole SuspendPackage method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that the SuspendPackage(XboxPackageDefinition) method throws an ObjectDisposedException.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestSuspendPackageThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.SuspendPackage(ConsoleAddress, this.CreateXboxPackageDefinition());
        }

        /// <summary>
        /// Verifies that the SuspendPackage(XboxPackageDefinition) method calls the XDK's SuspendPackage method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        public void TestSuspendPackageCallsXdkSuspendPackage()
        {
            XboxPackageDefinition package = new XboxPackageDefinition(PackageFullName, PackageFamilyName, Aumids);

            bool isCorrectMethodCalled = false;
            this.fakeXboxXdk.SuspendPackageAction = (ipAddress, packageFullName) =>
            {
                isCorrectMethodCalled = true;
                Assert.AreEqual(package.FullName, packageFullName, "Package Full Name passed to XDK was different than value in XboxPackage parameter.");
            };
            this.fakeXboxXdk.QueryPackageExecutionStateFunc = (ipAddress, packageFullName) => (uint)PackageExecutionState.Suspended;

            this.adapter.SuspendPackage(ConsoleAddress, package);

            Assert.IsTrue(isCorrectMethodCalled, "The Adapter did not call the XDK's SuspendPackage method.");

            this.VerifyThrows<ArgumentNullException>(() => this.adapter.SuspendPackage(null, package));
        }

        /// <summary>
        /// Verifies that the ConstrainPackage(XboxPackageDefinition) method turns COMExceptions into XboxConsoleExceptions.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        public void TestConstrainPackageTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.ConstrainPackageAction = (_, __) =>
            {
                throw new COMException();
            };

            try
            {
                this.adapter.ConstrainPackage(ConsoleAddress, this.CreateXboxPackageDefinition());
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The ConstrainPackage method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The ConstrainPackage method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that the ConstrainPackage(XboxPackageDefinition) method calls the XDK's ConstrainPackage method.
        /// Verifies that the ConstrainPackage(XboxPackageDefinition) method throws an ArgumentNullException.
        /// Verifies that the ConstrainPackage(XboxPackageDefinition) method throws an ObjectDisposedException.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        public void TestConstrainPackageCallsXdkConstrainPackage()
        {
            XboxPackageDefinition package = new XboxPackageDefinition(PackageFullName, PackageFamilyName, Aumids);

            bool isCorrectMethodCalled = false;
            this.fakeXboxXdk.ConstrainPackageAction = (ipAddress, packageFullName) =>
            {
                isCorrectMethodCalled = true;
                Assert.AreEqual(package.FullName, packageFullName, "Package Full Name passed to XDK was different than value in XboxPackage parameter.");
            };
            this.fakeXboxXdk.QueryPackageExecutionStateFunc = (ipAddress, packageFullName) => (uint)PackageExecutionState.Constrained;

            this.adapter.ConstrainPackage(ConsoleAddress, package);

            Assert.IsTrue(isCorrectMethodCalled, "The Adapter did not call the XDK's ConstrainPackage method.");

            this.VerifyThrows<ArgumentNullException>(() => this.adapter.ConstrainPackage(null, package));
            this.VerifyThrows<ArgumentNullException>(() => this.adapter.ConstrainPackage(ConsoleAddress, null));
            this.adapter.Dispose();
            this.VerifyThrows<ObjectDisposedException>(() => this.adapter.ConstrainPackage(ConsoleAddress, package));
        }

        /// <summary>
        /// Verifies that the UnconstrainPackage(XboxPackageDefinition) method turns COMExceptions into XboxConsoleExceptions.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        public void TestUnconstrainPackageTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.UnconstrainPackageAction = (_, __) =>
            {
                throw new COMException();
            };

            try
            {
                this.adapter.UnconstrainPackage(ConsoleAddress, this.CreateXboxPackageDefinition());
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The UnconstrainPackage method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The UnconstrainPackage method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that the UnconstrainPackage(XboxPackageDefinition) method calls the XDK's UnconstrainPackage method.
        /// Verifies that the UnconstrainPackage(XboxPackageDefinition) method throws an ArgumentNullException.
        /// Verifies that the UnconstrainPackage(XboxPackageDefinition) method throws an ObjectDisposedException.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        public void TestUnconstrainPackageCallsXdkUnconstrainPackage()
        {
            XboxPackageDefinition package = new XboxPackageDefinition(PackageFullName, PackageFamilyName, Aumids);

            bool isCorrectMethodCalled = false;
            this.fakeXboxXdk.UnconstrainPackageAction = (ipAddress, packageFullName) =>
            {
                isCorrectMethodCalled = true;
                Assert.AreEqual(package.FullName, packageFullName, "Package Full Name passed to XDK was different than value in XboxPackage parameter.");
            };

            this.adapter.UnconstrainPackage(ConsoleAddress, package);

            Assert.IsTrue(isCorrectMethodCalled, "The Adapter did not call the XDK's UnconstrainPackage method.");

            this.VerifyThrows<ArgumentNullException>(() => this.adapter.UnconstrainPackage(null, package));
            this.VerifyThrows<ArgumentNullException>(() => this.adapter.UnconstrainPackage(ConsoleAddress, null));
            this.adapter.Dispose();
            this.VerifyThrows<ObjectDisposedException>(() => this.adapter.UnconstrainPackage(ConsoleAddress, package));
        }

        /// <summary>
        /// Verifies that the SnapApplication(XboxApplicationDefinition) method throws an ObjectDisposedException.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestSnapApplicationThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.SnapApplication(ConsoleAddress, this.CreateXboxApplicationDefinition());
        }

        /// <summary>
        /// Verifies that the SnapApplication(XboxApplicationDefinition) method calls the XDK's SnapApplication method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        public void TestSnapApplicationCallsXdkSnapPackage()
        {
            XboxApplicationDefinition application = this.CreateXboxApplicationDefinition();

            bool isCorrectMethodCalled = false;
            this.fakeXboxXdk.SnapApplicationAction = (ipAddress, aumid) =>
            {
                isCorrectMethodCalled = true;
                Assert.AreEqual(application.Aumid, aumid, "Aumid passed to XDK was different than value in XboxPackage parameter.");
            };

            this.adapter.SnapApplication(ConsoleAddress, application);

            Assert.IsTrue(isCorrectMethodCalled, "The Adapter did not call the XDK's SnapPackage method.");
        }

        /// <summary>
        /// Verifies that the SnapApplication(XboxApplicationDefinition) method turns COMExceptions into XboxConsoleExceptions.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        public void TestSnapApplicationTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.SnapApplicationAction = (_, __) =>
            {
                throw new COMException();
            };

            try
            {
                this.adapter.SnapApplication(ConsoleAddress, this.CreateXboxApplicationDefinition());
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The XboxConsole SnapApplication method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The XboxConsole SnapApplication method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
                return;
            }

            Assert.Fail("The XboxConsole SnapApplication method did not recieve a COMException.");
        }

        /// <summary>
        /// Verifies that the SnapApplication(XboxApplicationDefinition) method throws an ArgumentNullException.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSnapApplicationThrowsArgumentNullExceptionWithNullAumid()
        {
            this.adapter.SnapApplication(ConsoleAddress, null);
        }

        /// <summary>
        /// Verifies that the SnapApplication(XboxApplicationDefinition) method throws an ArgumentNullException.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSnapApplicationThrowsArgumentNullExceptionWithNullConsoleAddress()
        {
            this.adapter.SnapApplication(null, this.CreateXboxApplicationDefinition());
        }

        /// <summary>
        /// Verifies that the UnsnapApplication() method throws an ObjectDisposedException.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestUnsnapApplicationThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.UnsnapApplication(ConsoleAddress);
        }

        /// <summary>
        /// Verifies that the UnsnapApplication() method calls the XDK's UnsnapApplication method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        public void TestUnsnapApplicationCallsXdkUnsnapApplication()
        {
            XboxApplicationDefinition application = this.CreateXboxApplicationDefinition();

            bool isCorrectMethodCalled = false;
            this.fakeXboxXdk.UnsnapApplicationAction = (ipAddress) =>
            {
                isCorrectMethodCalled = true;
            };

            this.adapter.UnsnapApplication(ConsoleAddress);

            Assert.IsTrue(isCorrectMethodCalled, "The Adapter did not call the XDK's UnsnapApplication method.");
        }

        /// <summary>
        /// Verifies that the UnsnapApplication() method throws an ArgumentNullException.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestUnsnapApplicationThrowsArgumentNullException()
        {
            this.adapter.UnsnapApplication(null);
        }

        /// <summary>
        /// Verifies that the UnsnapApplication() method turns COMExceptions into XboxConsoleExceptions.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        public void TestUnsnapApplicationTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.UnsnapApplicationAction = (_) =>
            {
                throw new COMException();
            };

            try
            {
                this.adapter.UnsnapApplication(ConsoleAddress);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The XboxConsole UnsnapApplication method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The XboxConsole UnsnapApplication method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
                return;
            }

            Assert.Fail("The XboxConsole UnsnapApplication method did not recieve a COMException.");
        }

        /// <summary>
        /// Verifies that the QueryPackageExecutionState(XboxPackageDefinition) method turns COMExceptions into XboxConsoleExceptions.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        public void TestQueryPackageExecutionStateTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.QueryPackageExecutionStateFunc = (_, __) =>
            {
                throw new COMException();
            };

            try
            {
                this.adapter.QueryPackageExecutionState(ConsoleAddress, this.CreateXboxPackageDefinition());
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The XboxConsole QueryPackageExecutionState method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The XboxConsole QueryPackageExecutionState method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that the QueryPackageExecutionState(XboxPackageDefinition) method throws an ObjectDisposedException.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestQueryPackageExecutionStateThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.QueryPackageExecutionState(ConsoleAddress, this.CreateXboxPackageDefinition());
        }

        /// <summary>
        /// Verifies that the QueryPackageExecutionState(XboxPackageDefinition) method calls the XDK's QueryPackageExecutionState method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        public void TestQueryPackageExecutionStateCallsXdkQueryPackageExecutionState()
        {
            XboxPackageDefinition package = new XboxPackageDefinition(PackageFullName, PackageFamilyName, Aumids);

            bool isCorrectMethodCalled = false;
            this.fakeXboxXdk.QueryPackageExecutionStateFunc = (ipAddress, packageFullName) =>
            {
                isCorrectMethodCalled = true;
                Assert.AreEqual(package.FullName, packageFullName, "Package Full Name passed to XDK was different than value in XboxPackage parameter.");
                return 0;
            };

            this.adapter.QueryPackageExecutionState(ConsoleAddress, package);

            Assert.IsTrue(isCorrectMethodCalled, "The Adapter did not call the XDK's QueryPackageExecutionState method.");

            this.VerifyThrows<ArgumentNullException>(() => this.adapter.QueryPackageExecutionState(null, package));
        }

        /// <summary>
        /// Verifies that the QueryPackageExecutionState(XboxPackageDefinition) method throws an ArgumentNullException.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestQueryPackageExecutionStateThrowsNullArgumentException()
        {
            this.adapter.QueryPackageExecutionState(ConsoleAddress, null);
        }

        /// <summary>
        /// Verifies that the adapter's DeployPushAsync() method
        /// calls the XboxXdk's DeployPushAsync method.
        /// </summary>
        /// <returns>Returns a task for the running test.</returns>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        public async Task TestDeployPushAsyncCallsXdkDeployPushAsync()
        {
            bool isCorrectFunctionCalled = false;
            this.fakeXboxXdk.DeployPushAsyncFunc = (ipAddress, deployPath, removeExtraFiles, progressMetric, progressError, progressExtraFile) =>
            {
                isCorrectFunctionCalled = true;
                return Task.FromResult(string.Empty);
            };

            // we don't care about parsing errors in this test (caused by empty string):
            try
            {
                await this.adapter.DeployPushAsync(ConsoleAddress, string.Empty, true, null, null, null).WithTimeout(TimeSpan.FromSeconds(30));
            }
            catch (XboxConsoleException ex)
            {
                Assert.AreEqual(PackagesParsingExceptionMessage, ex.Message, "Empty string received from the XDK didn't throw an expected exception.");
            }

            Assert.IsTrue(isCorrectFunctionCalled, "The adapter's DeployPushAsync function failed to call the XboxXdk's DeployPushAsync function.");
        }

        /// <summary>
        /// Verifies that the adapter's DeployPushAsync() method
        /// handles null parameters.
        /// </summary>
        /// <returns>Returns a task for the running test.</returns>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        public async Task TestDeployPushAsyncNullParameters()
        {
            this.fakeXboxXdk.DeployPushAsyncFunc = (ipAddress, deployPath, removeExtraFiles, progressMetric, progressError, progressExtraFile) =>
            {
                Assert.Fail("Should never have gotten to the XDK method");
                return Task.FromResult<string>(null);
            };

            try
            {
                await this.adapter.DeployPushAsync(null, string.Empty, true, null, null, null).WithTimeout(TimeSpan.FromSeconds(30));
                Assert.Fail("ArgumentNullException should have been thrown");
            }
            catch (ArgumentNullException)
            {
            }

            try
            {
                await this.adapter.DeployPushAsync(ConsoleAddress, null, true, null, null, null).WithTimeout(TimeSpan.FromSeconds(30));
                Assert.Fail("ArgumentNullException should have been thrown");
            }
            catch (ArgumentNullException)
            {
            }
        }

        /// <summary>
        /// Verifies that the adapter throws an exception if the text it receives from the XDK is null.
        /// </summary>
        /// <returns>Returns a task for the running test.</returns>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        public async Task TestDeployPushAsyncThrowsWithNullOutput()
        {
            this.fakeXboxXdk.DeployPushAsyncFunc = (ipAddress, deployPath, removeExtraFiles, progressMetric, progressError, progressExtraFile) =>
            {
                return Task.FromResult<string>(null);
            };

            try
            {
                XboxPackageDefinition package = await this.adapter.DeployPushAsync(ConsoleAddress, string.Empty, true, null, null, null).WithTimeout(TimeSpan.FromSeconds(30));
                Assert.Fail("Null received from the XDK didn't throw an expected exception.");
            }
            catch (XboxConsoleException ex)
            {
                Assert.AreEqual(PackagesParsingExceptionMessage, ex.Message, "Null received from the XDK didn't throw an expected exception.");
            }
        }

        /// <summary>
        /// Verifies that the adapter throws an exception if the text it receives from the XDK is an empty string.
        /// </summary>
        /// <returns>Returns a task for the running test.</returns>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        public async Task TestDeployPushAsyncThrowsWithEmptyStringOutput()
        {
            this.fakeXboxXdk.DeployPushAsyncFunc = (ipAddress, deployPath, removeExtraFiles, progressMetric, progressError, progressExtraFile) =>
            {
                return Task.FromResult(string.Empty);
            };

            try
            {
                XboxPackageDefinition package = await this.adapter.DeployPushAsync(ConsoleAddress, string.Empty, true, null, null, null).WithTimeout(TimeSpan.FromSeconds(30));
                Assert.Fail("Empty string received from the XDK didn't throw an expected exception.");
            }
            catch (XboxConsoleException ex)
            {
                Assert.AreEqual(PackagesParsingExceptionMessage, ex.Message, "Empty string received from the XDK didn't throw an expected exception.");
            }
        }

        /// <summary>
        /// Verifies that the adapter throws an exception if the text it receives from the XDK is not in the expected format.
        /// </summary>
        /// <returns>Returns a task for the running test.</returns>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        public async Task TestDeployPushAsyncThrowsWithImproperlyFormattedOutput()
        {
            this.fakeXboxXdk.DeployPushAsyncFunc = (ipAddress, deployPath, removeExtraFiles, progressMetric, progressError, progressExtraFile) =>
            {
                return Task.FromResult("ImproperlyFormattedText");
            };

            try
            {
                XboxPackageDefinition package = await this.adapter.DeployPushAsync(ConsoleAddress, string.Empty, true, null, null, null).WithTimeout(TimeSpan.FromSeconds(30));
                Assert.Fail("Incorrectly formatted text received from the XDK didn't throw an expected exception.");
            }
            catch (XboxConsoleException ex)
            {
                Assert.AreEqual(PackagesParsingExceptionMessage, ex.Message, "Incorrectly formatted text received from the XDK didn't throw an expected exception.");
            }
        }

        /// <summary>
        /// Verifies that the adapter correctly parses a valid string from the XDK.
        /// </summary>
        /// <returns>Returns a task for the running test.</returns>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        public async Task TestDeployPushAsync()
        {
            this.fakeXboxXdk.DeployPushAsyncFunc = (ipAddress, deployPath, removeExtraFiles, progressMetric, progressError, progressExtraFile) =>
            {
                return Task.FromResult("{\"Applications\":[\"" + Aumid + "\"],\"Identity\":{\"FullName\":\"" + PackageFullName + "\"}}");
            };

            XboxPackageDefinition package = await this.adapter.DeployPushAsync(ConsoleAddress, string.Empty, true, null, null, null).WithTimeout(TimeSpan.FromSeconds(30));
            Assert.IsNotNull(package, "DeployPushAsync returned a null value.");

            Assert.AreEqual(PackageFamilyName, package.FamilyName, "Created package had the wrong Package Family Name.");
            Assert.AreEqual(PackageFullName, package.FullName, "Created package had the wrong Package Full Name.");
            Assert.AreEqual(1, package.ApplicationDefinitions.Count(), "Created package had the wrong number of ApplicationDefinitions");
            Assert.AreEqual(ApplicationId, package.ApplicationDefinitions.First().ApplicationId, "Created package had the wrong Application Id.");
        }

        /// <summary>
        /// Verifies that the adapter correctly passes on the parameters to the Xdk.
        /// </summary>
        /// <returns>Returns a task for the running test.</returns>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        public async Task TestDeployPushAsyncPassesOnParametersToXdk()
        {
            Progress<XboxDeploymentMetric> progressMetricObject = new Progress<XboxDeploymentMetric>();
            Progress<XboxDeploymentError> progressErrorObject = new Progress<XboxDeploymentError>();
            Progress<XboxDeploymentExtraFile> progressExtraFileObject = new Progress<XboxDeploymentExtraFile>();

            bool expectedRemoveExtraFilesValue = false;
            string expectedDeployPathValue = "ExpectedDeployPathValue";

            this.fakeXboxXdk.DeployPushAsyncFunc = (ipAddress, deployPath, removeExtraFiles, progressMetric, progressError, progressExtraFile) =>
            {
                Assert.AreSame(progressMetricObject, progressMetric, "Adapter failed to pass in the same XboxDeploymentMetric object to the Xdk");
                Assert.AreSame(progressErrorObject, progressError, "Adapter failed to pass in the same XboxDeploymentError object to the Xdk");
                Assert.AreSame(progressExtraFileObject, progressExtraFile, "Adapter failed to pass in the same XboxDeploymentExtraFile object to the Xdk");

                Assert.AreEqual(expectedRemoveExtraFilesValue, removeExtraFiles, "Adapter failed to pass in the correct value for the removeExtraFiles parameter to the Xdk");
                Assert.AreEqual(expectedDeployPathValue, deployPath, "Adapter failed to pass in the correct value for the deployPath parameter to the Xdk");

                return Task.FromResult("{\"Applications\":[\"" + Aumid + "\"],\"Identity\":{\"FullName\":\"" + PackageFullName + "\"}}");
            };

            await this.adapter.DeployPushAsync(
                    ConsoleAddress,
                    expectedDeployPathValue,
                    expectedRemoveExtraFilesValue,
                    progressMetricObject,
                    progressErrorObject,
                    progressExtraFileObject).WithTimeout(TimeSpan.FromSeconds(30));

            expectedRemoveExtraFilesValue = true;

            await this.adapter.DeployPushAsync(
                    ConsoleAddress,
                    expectedDeployPathValue,
                    expectedRemoveExtraFilesValue,
                    progressMetricObject,
                    progressErrorObject,
                    progressExtraFileObject).WithTimeout(TimeSpan.FromSeconds(30));
        }

        /// <summary>
        /// Verifies that the UninstallPackage(XboxPackageDefinition) method throws an ObjectDisposedException.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestUninstallPackageThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.UninstallPackage(ConsoleAddress, this.CreateXboxPackageDefinition());
        }

        /// <summary>
        /// Verifies that the UninstallPackage(XboxPackageDefinition) method calls the XDK's UninstallPackage method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        public void TestUninstallPackageCallsXdkUninstallPackage()
        {
            XboxPackageDefinition package = this.CreateXboxPackageDefinition();

            bool isCorrectMethodCalled = false;
            this.fakeXboxXdk.UninstallPackageAction = (ipAddress, packageFullName) =>
            {
                isCorrectMethodCalled = true;
                Assert.AreEqual(package.FullName, packageFullName, "Package passed to XDK was different than value in package parameter.");
            };

            this.adapter.UninstallPackage(ConsoleAddress, package);

            Assert.IsTrue(isCorrectMethodCalled, "The Adapter did not call the XDK's UninstallPackage method.");

            this.VerifyThrows<ArgumentNullException>(() => this.adapter.UninstallPackage(null, package));
        }

        /// <summary>
        /// Verifies that the UninstallPackage(XboxPackageDefinition) method turns COMExceptions into XboxConsoleExceptions.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        public void TestUninstallPackageTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.UninstallPackageAction = (_, __) =>
            {
                throw new COMException();
            };

            try
            {
                this.adapter.UninstallPackage(ConsoleAddress, this.CreateXboxPackageDefinition());
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The XboxConsole UninstallPackage method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The XboxConsole UninstallPackage method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that the UninstallPackage(XboxPackageDefinition) method throws an ArgumentNullException.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterPackageManagementTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestUninstallPackageThrowsNullArgumentException()
        {
            this.adapter.UninstallPackage(ConsoleAddress, null);
        }

        private XboxPackageDefinition CreateXboxPackageDefinition()
        {
            return new XboxPackageDefinition(PackageFullName, PackageFamilyName, new string[] { PackageFamilyName + "!" + ApplicationId });
        }

        private XboxApplicationDefinition CreateXboxApplicationDefinition()
        {
            return new XboxApplicationDefinition(Aumid);
        }
    }
}
