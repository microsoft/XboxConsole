//------------------------------------------------------------------------------
// <copyright file="XboxConsoleAdapter.Tests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Adapter.Tests
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using Microsoft.Internal.GamesTest.Xbox.Fakes;
    using Microsoft.QualityTools.Testing.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Win32;
    using Microsoft.Win32.Fakes;

    /// <summary>
    /// Represents tests for the XboxConsole adapter types.
    /// </summary>
    [TestClass]
    public partial class XboxConsoleAdapterTests
    {
        private const string XboxConsoleAdapterTestCategory = "Adapter.XboxConsoleAdapterBase";

        private const string ConsoleAddress = "10.124.151.244"; // The actual IP address used here is irrelevant.
        private const string ConsoleSystemIpAddress = "10.124.151.245"; // The actual IP address used here is irrelevant.

        private IDisposable shimsContext;
        private XboxConsoleAdapterBase adapter;
        private FakeXboxXdk fakeXboxXdk;

        /// <summary>
        /// Called once before each test to setup shim and stub objects.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.shimsContext = ShimsContext.Create();
            this.fakeXboxXdk = new FakeXboxXdk();
            ShimXboxConsoleAdapterBase.ConstructorXboxXdkBase = (consoleAdapter, xboxXdk) =>
                {
                    var shimXboxConsoleAdapter = new ShimXboxConsoleAdapterBase(consoleAdapter)
                        {
                            XboxXdkGet = () => this.fakeXboxXdk,
                        };
                };

            this.adapter = this.CreateXboxConsoleAdapter();
        }

        /// <summary>
        /// Called once after each test to clean up shim and stub objects.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            this.shimsContext.Dispose();
            this.adapter.Dispose();
        }

        /// <summary>
        /// Verifies that the constructor throws an exception when Durango XDK directory environment variable is not set.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxConsoleAdapterTestCategory)]
        [ExpectedException(typeof(XdkNotFoundException))]
        public void TestConstructorThrowsXdkNotFoundExceptionXdkDirectoryIsNotSet()
        {
            string storeValue = Environment.GetEnvironmentVariable("DurangoXdk");
            const string ExpectedXdkRegistryKeyName = "InstallPath";

            Environment.SetEnvironmentVariable("DurangoXdk", null);

            ShimRegistryKey.AllInstances.GetValueString = (instance, xdkRegistryKeyName) =>
            {
                Assert.IsTrue(ExpectedXdkRegistryKeyName.Equals(xdkRegistryKeyName, StringComparison.InvariantCultureIgnoreCase));
                return null;
            };

            try
            {
                ShimXboxConsoleAdapterBase.ConstructorXboxXdkBase = null;

                // we need to run real (not shimmed) constructor
                this.adapter = this.CreateXboxConsoleAdapter();
            }
            finally
            {
                Environment.SetEnvironmentVariable("DurangoXdk", storeValue);
            }

            Assert.Fail("The constructor must throw an exception when Durango XDK directory environment variable is not set.");
        }

        /// <summary>
        /// Verifies that the DefaultConsole (get) property throws an ObjectDisposedException if
        /// the adapter has been disposed.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxConsoleAdapterTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestDefaultConsoleGetThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            var defaultConsole = this.adapter.DefaultConsole;
            Assert.Fail("ObjectDisposedException was not thrown.");
        }

        /// <summary>
        /// Verifies that the DefaultConsole (set) property throws an ObjectDisposedException if
        /// the adapter has been disposed.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxConsoleAdapterTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestDefaultConsoleSetThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.DefaultConsole = ConsoleAddress;
            Assert.Fail("ObjectDisposedException was not thrown.");
        }

        /// <summary>
        /// Verifies that the DefaultConsole property converts COMExceptions thrown by the XDK
        /// into XboxExceptions.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxConsoleAdapterTestCategory)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        public void TestDefaultConsoleTurnsComExceptionIntoXboxException()
        {
            this.fakeXboxXdk.DefaultConsoleGetFunc = () =>
            {
                throw new COMException();
            };

            this.fakeXboxXdk.DefaultConsoleSetAction = address =>
            {
                throw new COMException();
            };

            try
            {
                var defaultConsole = this.adapter.DefaultConsole;
                Assert.Fail("The XDK should have thrown a COMException");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxException), "The XboxConsoleAdapter DefaultConsole (get) property did not convert a COMException into an XboxException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The XboxConsoleAdapter DefaultConsole (get) property did not include the COMException as the inner exception of the XboxException that it threw.");
            }

            try
            {
                this.adapter.DefaultConsole = ConsoleAddress;
                Assert.Fail("The XDK should have thrown a COMException");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxException), "The XboxConsoleAdapter DefaultConsole (set) property did not convert a COMException into an XboxException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The XboxConsoleAdapter DefaultConsole (set) property did not include the COMException as the inner exception of the XboxException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that the DefaultConsole property passes the correct parameters to the Xdk.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxConsoleAdapterTestCategory)]
        public void TestDefaultConsoleCallsXdkDefaultConsole()
        {
            bool isCorrectPropertyCalled = false;
            this.fakeXboxXdk.DefaultConsoleGetFunc = () =>
            {
                isCorrectPropertyCalled = true;
                return ConsoleAddress;
            };

            this.fakeXboxXdk.DefaultConsoleSetAction = address =>
            {
                isCorrectPropertyCalled = true;
                Assert.AreEqual(ConsoleAddress, address, "DefaultConsole (set) property did not pass the correct IP address to the Xdk.");
            };

            var defaultConsole = this.adapter.DefaultConsole;
            Assert.IsTrue(isCorrectPropertyCalled, "The DefaultConsole (get) property did not call the Xdk's DefaultConsole (get) property.");
            Assert.AreEqual(ConsoleAddress, defaultConsole, "DefaultConsole (get) property did not receive the correct IP address from the Xdk.");

            isCorrectPropertyCalled = false;
            this.adapter.DefaultConsole = ConsoleAddress;
            Assert.IsTrue(isCorrectPropertyCalled, "The DefaultConsole (set) property did not call the Xdk's DefaultConsole (set) property.");
        }

        /// <summary>
        /// Verifies that the GetConfigValue method throws an ObjectDisposedException if
        /// the adapter has been disposed.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxConsoleAdapterTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestGetConfigValueThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            var value = this.adapter.GetConfigValue(ConsoleAddress, "SettingKey");
            Assert.Fail("ObjectDisposedException was not thrown.");
        }

        /// <summary>
        /// Verifies that the GetConfigValue method converts COMExceptions thrown by the XDK
        /// into XboxConsoleExceptions.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxConsoleAdapterTestCategory)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        public void TestGetConfigValueTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.GetConfigValueFunc = (address, key) =>
            {
                throw new COMException();
            };

            try
            {
                var value = this.adapter.GetConfigValue(ConsoleAddress, "SettingKey");
                Assert.Fail("The XDK should have thrown a COMException");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The XboxConsoleAdapter GetConfigValue method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The XboxConsoleAdapter GetConfigValue method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that the GetConfigValue method passes the correct parameters to the Xdk.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxConsoleAdapterTestCategory)]
        public void TestGetConfigValueCallsXdkGetConfigValue()
        {
            bool isCorrectMethodCalled = false;
            string correctKey = "SettingKey";
            string correctValue = "SettingValue";
            this.fakeXboxXdk.GetConfigValueFunc = (address, key) =>
            {
                isCorrectMethodCalled = true;
                Assert.AreEqual(ConsoleAddress, address, "GetConfigValue method did not pass the correct system IP address to the Xdk.");
                Assert.AreEqual(correctKey, key, "GetConfigValue method did not pass the correct setting key to the Xdk.");
                return correctValue;
            };

            var value = this.adapter.GetConfigValue(ConsoleAddress, correctKey);
            Assert.IsTrue(isCorrectMethodCalled, "The GetConfigValue method did not call the Xdk's GetConfigValue method.");
            Assert.AreEqual(correctValue, value, "GetConfigValue method did not receive the correct setting value from the Xdk.");

            this.VerifyThrows<ArgumentNullException>(() => this.adapter.GetConfigValue(null, correctKey));
            this.VerifyThrows<ArgumentNullException>(() => this.adapter.GetConfigValue(ConsoleAddress, null));
        }

        /// <summary>
        /// Verifies that the SetConfigValue method throws an ObjectDisposedException if
        /// the adapter has been disposed.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxConsoleAdapterTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestSetConfigValueThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.SetConfigValue(ConsoleAddress, "SettingKey", "SettingValue");
            Assert.Fail("ObjectDisposedException was not thrown.");
        }

        /// <summary>
        /// Verifies that the SetConfigValue method converts COMExceptions thrown by the XDK
        /// into XboxConsoleExceptions.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxConsoleAdapterTestCategory)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        public void TestSetConfigValueTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.SetConfigValueAction = (address, key, value) =>
            {
                throw new COMException();
            };

            try
            {
                this.adapter.SetConfigValue(ConsoleAddress, "SettingKey", "SettingValue");
                Assert.Fail("The XDK should have thrown a COMException");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The XboxConsoleAdapter SetConfigValue method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The XboxConsoleAdapter SetConfigValue method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that the SetConfigValue method passes the correct parameters to the Xdk.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxConsoleAdapterTestCategory)]
        public void TestSetConfigValueCallsXdkSetConfigValue()
        {
            bool isCorrectMethodCalled = false;
            string correctKey = "SettingKey";
            string correctValue = "SettingValue";
            this.fakeXboxXdk.SetConfigValueAction = (address, key, value) =>
            {
                isCorrectMethodCalled = true;
                Assert.AreEqual(ConsoleAddress, address, "SetConfigValue method did not pass the correct system IP address to the Xdk.");
                Assert.AreEqual(correctKey, key, "SetConfigValue method did not pass the correct setting key to the Xdk.");
                Assert.AreEqual(correctValue, value, "SetConfigValue method did not pass the correct setting value to the Xdk.");
            };

            this.adapter.SetConfigValue(ConsoleAddress, correctKey, correctValue);
            Assert.IsTrue(isCorrectMethodCalled, "The SetConfigValue method did not call the Xdk's SetConfigValue method.");

            this.VerifyThrows<ArgumentNullException>(() => this.adapter.SetConfigValue(null, correctKey, correctValue));
            this.VerifyThrows<ArgumentNullException>(() => this.adapter.SetConfigValue(ConsoleAddress, null, correctValue));
            this.VerifyThrows<ArgumentNullException>(() => this.adapter.SetConfigValue(ConsoleAddress, correctKey, null));
        }

        /// <summary>
        /// Verifies that the GetConsoleInfo method throws an ObjectDisposedException if
        /// the adapter has been disposed.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxConsoleAdapterTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestGetConsoleInfoThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            var value = this.adapter.GetConsoleInfo(ConsoleAddress);
            Assert.Fail("ObjectDisposedException was not thrown.");
        }

        /// <summary>
        /// Verifies that the GetConsoleInfo method converts COMExceptions thrown by the XDK
        /// into XboxConsoleExceptions.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxConsoleAdapterTestCategory)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        public void TestGetConsoleInfoTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.GetConsoleInfoFunc = (address) =>
            {
                throw new COMException();
            };

            try
            {
                var value = this.adapter.GetConsoleInfo(ConsoleAddress);
                Assert.Fail("The XDK should have thrown a COMException");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The XboxConsoleAdapter GetConsoleInfo method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The XboxConsoleAdapter GetConsoleInfo method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that the GetConsoleInfo method passes the correct parameters to the Xdk.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxConsoleAdapterTestCategory)]
        public void TestGetConsoleInfoCallsXdkGetConsoleInfo()
        {
            bool isCorrectMethodCalled = false;

            string expectedToolsIp = "Expected Tools Ip";
            string expectedSystemIp = "Expected System Ip";
            string expectedAccessKey = "Expected Access Key";
            string expectedConsoleId = "Expected Console Id";
            XboxCertTypes expectedCertType = XboxCertTypes.EraDevKit | XboxCertTypes.Other;
            string expectedHostName = "Expected Host Name";
            string expectedDeviceId = "Expected Device Id";

            XboxConsoleInfo expectedValue = new XboxConsoleInfo(expectedToolsIp, expectedSystemIp, expectedAccessKey, expectedConsoleId, expectedCertType, expectedHostName, expectedDeviceId);
            this.fakeXboxXdk.GetConsoleInfoFunc = (address) =>
            {
                isCorrectMethodCalled = true;
                Assert.AreEqual(ConsoleAddress, address, "GetConsoleInfo method did not pass the correct system IP address to the Xdk.");
                return expectedValue;
            };

            var value = this.adapter.GetConsoleInfo(ConsoleAddress);
            Assert.IsTrue(isCorrectMethodCalled, "The GetConsoleInfo method did not call the Xdk's GetConsoleInfo method.");

            Assert.AreEqual(expectedToolsIp, value.ToolsIpAddress, "GetConsoleInfo method did not receive the correct tools ip value in the returned XboxConsoleInfo object from the xdk.");
            Assert.AreEqual(expectedSystemIp, value.ConsoleIPAddress, "etConsoleInfo method did not receive the correct console ip value in the returned XboxConsoleInfo object from the xdk.");
            Assert.AreEqual(expectedAccessKey, value.AccessKey, "GetConsoleInfo method did not receive the correct access key value in the returned XboxConsoleInfo object from the xdk.");
            Assert.AreEqual(expectedConsoleId, value.ConsoleId, "GetConsoleInfo method did not receive the correct console id value in the returned XboxConsoleInfo object from the xdk.");
            Assert.AreEqual(expectedCertType, value.CertType, "GetConsoleInfo method did not receive the correct cert type value in the returned XboxConsoleInfo object from the xdk.");
            Assert.AreEqual(expectedHostName, value.HostName, "GetConsoleInfo method did not receive the correct host name value in the returned XboxConsoleInfo object from the xdk.");
            Assert.AreEqual(expectedDeviceId, value.DeviceId, "GetConsoleInfo method did not receive the correct device id value in the returned XboxConsoleInfo object from the xdk.");

            this.VerifyThrows<ArgumentNullException>(() => this.adapter.GetConsoleInfo(null));
        }

        /// <summary>
        /// Verifies that Xtf assemblies from ManagedXtf are embedded into the currently loaded adapter.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Xtf", Justification = "Xtf is the name of embedded assemblies used by XboxConsole"), TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxConsoleAdapterTestCategory)]
        public void TestXtfAssembliesAreEmbedded()
        {
            // load assembly of the currently used adapter
            Assembly adapterAssembly = this.adapter.GetType().Assembly;

            // get a string expressing the release version of an adapter, like "August2013QFE3"
            string adapterRelease = Path.GetExtension(adapterAssembly.GetName().Name);
            if (!adapterRelease.StartsWith("."))
            {
                Assert.Fail("The currently loaded adapter doesn't have a conventional prefix in the full assembly name");
            }

            adapterRelease = adapterRelease.Substring(1);

            // build a collection of full Xtf assembly file names as they are stored in adapter resources
            var xtfAssemblyFileNames = new string[] 
            {
                "Application.dll",
                "ConsoleControl.dll",
                "ConsoleManager.dll",
                "DebugMonitor.dll",
                "dll",
                "FileIO.dll",
                "Input.dll",
                "RemoteRun.dll",
                "User.dll"
            }.Select(fileName => string.Format(CultureInfo.InvariantCulture, "Microsoft.Internal.GamesTest.Xbox.Adapter.{0}.Microsoft.Xbox.Xtf.{1}", adapterRelease, fileName));

            var resourceNames = adapterAssembly.GetManifestResourceNames();
            var notFoundAssemblyFileNames = xtfAssemblyFileNames.Except(resourceNames, StringComparer.OrdinalIgnoreCase);
            if (notFoundAssemblyFileNames.Any())
            {
                Assert.Fail("The following files are not found in embedded resources of the currently loaded adapter: {0}", string.Join(", ", notFoundAssemblyFileNames));
            }
        }

        private XboxConsoleAdapterBase CreateXboxConsoleAdapter()
        {
            // The ConstructionHelpers class can be changed to any version of the adapter to test that specific adapter against a desired version of fake XDK.
            return ConstructionHelpers.NewXboxConsoleAdapter(this.fakeXboxXdk);
        }

        private void VerifyThrows<T>(Action action) where T : Exception
        {
            try
            {
                action();
                Assert.Fail(string.Format(CultureInfo.CurrentUICulture, "{0} should have been thrown", typeof(T).Name));
            }
            catch (T)
            {
            }
        }
    }
}