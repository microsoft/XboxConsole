//------------------------------------------------------------------------------
// <copyright file="XboxConsoleTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using Microsoft.Internal.GamesTest.Xbox.Configuration;
    using Microsoft.Internal.GamesTest.Xbox.Configuration.Fakes;
    using Microsoft.Internal.GamesTest.Xbox.Fakes;
    using Microsoft.Internal.GamesTest.Xbox.Input;
    using Microsoft.Internal.GamesTest.Xbox.IO;
    using Microsoft.QualityTools.Testing.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
  /// Represents tests for the XboxConsole type.
  /// </summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "This is test code and the object is disposed in the TestCleanup method.")]
  [TestClass]
  public class XboxConsoleTests
  {
    private const string XboxConsoleTestCategory = "XboxConsole.XboxConsole";

    private const string ConsoleAddress = "10.124.151.244"; // The actual IP address used here is irrelevant.
    private const string ConsoleSystemIpAddress = "10.124.151.245"; // The actual IP address used here is irrelevant.
    private const string ConsoleHostName = "XBOXONETESTHOST";

    private const string ConsoleSessionKey = "TestSessionKey"; // The actual session key used here is irrelevant.

    private const string ConsoleCombinedIPAndSessionKey = "10.124.151.244+TestSessionKey";
    private const string ConsoleCombinedHostNameAndSessionKey = "XBOXONETESTHOST+TestSessionKey";

    private const string PackageFamilyName = "NuiView.ERA_8wekyb3d8bbwe";
    private const string PackageFullName = "NuiView.ERA_1.0.0.0_neutral__8wekyb3d8bbwe";
    private const string ApplicationId = "NuiView.ERA";
    private const string Aumid = PackageFamilyName + "!" + ApplicationId;

    private const string TestOutputPath = "TestClip.mp4";

    private IDisposable shimsContext;
    private StubXboxConsoleAdapterBase stubAdapter;
    private List<XboxGamepad> gamepads;
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
      this.gamepads = new List<XboxGamepad>();

      ShimXboxConsole.ConstructorIPAddress = (realConsole, address) =>
      {
        var myShim = new ShimXboxConsole(realConsole)
        {
          AdapterGet = () => this.stubAdapter,
          SystemIpAddressGet = () => IPAddress.Parse(XboxConsoleTests.ConsoleAddress),
          SystemIpAddressStringGet = () => this.console.SystemIpAddress.ToString(),
          XboxGamepadsGet = () => this.gamepads
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
    /// Verifies that the constructor throws an ArgumentNullException if the given IPAddress is null.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TestConstructorThrowsArgumentNullExceptionWithNullIpAddress()
    {
      ShimXboxConsole.ConstructorIPAddress = null;
      using (XboxConsole xboxConsole = new XboxConsole((IPAddress)null))
      {
      }
    }

    /// <summary>
    /// Verifies that the constructor correctly sets the value for the SystemIpAddress property.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestConstructorProperlySetsIpAddress()
    {
        ShimXboxConsole.ConstructorIPAddress = null;
        using (XboxConsole xboxConsole = new XboxConsole(IPAddress.Parse(ConsoleAddress)))
        {
            Assert.AreEqual(IPAddress.Parse(ConsoleAddress), xboxConsole.SystemIpAddress, "The XboxConsole did not return the proper IP Address.");
            Assert.IsNull(xboxConsole.SessionKey, "The XboxConsole did not return the proper session key.");
            Assert.AreEqual(ConsoleAddress, xboxConsole.SystemIpAddressAndSessionKeyCombined, "The XboxConsole did not return the proper combined IP Address and session key.");
        }
    }

    /// <summary>
    /// Verifies that the constructor correctly sets the values for the SystemIpAddress property and SessionKey property.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestConstructorProperlySetsIpAddressAndSessionKey()
    {
        ShimXboxConsole.ConstructorIPAddressString = null;
        using (XboxConsole xboxConsole = new XboxConsole(IPAddress.Parse(ConsoleAddress), ConsoleSessionKey))
        {
            Assert.AreEqual(IPAddress.Parse(ConsoleAddress), xboxConsole.SystemIpAddress, "The XboxConsole did not return the proper IP Address.");
            Assert.AreEqual(ConsoleSessionKey, xboxConsole.SessionKey, "The XboxConsole did not return the proper SessionKey.");
            Assert.AreEqual(ConsoleCombinedIPAndSessionKey, xboxConsole.SystemIpAddressAndSessionKeyCombined, "The XboxConsole did not return the proper combined IP Address and session key.");
        }
    }

    /// <summary>
    /// Verifies that the IP Address and Session Key constructor correctly sets the value for the ConnectionString property.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestIPAddressAndSessionKeyConstructorProperlySetsConnectionString()
    {
        ShimXboxConsole.ConstructorIPAddressString = null;
        using (XboxConsole xboxConsole = new XboxConsole(IPAddress.Parse(ConsoleAddress), ConsoleSessionKey))
        {
            Assert.AreEqual(ConsoleCombinedIPAndSessionKey, xboxConsole.ConnectionString, "The XboxConsole connection string was not set properly by IP Address and Session Key constructor.");
        }
    }

    /// <summary>
    /// Verifies that the connection string constructor correctly sets the value for the ConnectionString property.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestConnectionStringConstructorWithIPAddressAndSessionKeyProperlySetsConnectionString()
    {
        ShimXboxConsole.ConstructorString = null;
        using (XboxConsole xboxConsole = new XboxConsole(ConsoleCombinedIPAndSessionKey))
        {
            Assert.AreEqual(ConsoleCombinedIPAndSessionKey, xboxConsole.ConnectionString, "The XboxConsole connection string was not set properly by Connection String constructor when initializing with IP Address and Session Key.");
        }
    }

    /// <summary>
    /// Verifies that the connection string constructor correctly sets the value for the SystemIpAddress property.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestConnectionStringConstructorSetsSystemIpAddressFromHostNameProperly()
    {
        ShimXboxConsole.ConstructorString = null;
        ShimXboxConsole.AllInstances.GetIPAddressFromConnectionStringString = (console, connectionString) =>
            {
                return IPAddress.Parse(ConsoleSystemIpAddress);
            };

        using (XboxConsole xboxConsole = new XboxConsole(ConsoleHostName))
        {
            Assert.AreEqual(xboxConsole.SystemIpAddress, IPAddress.Parse(ConsoleSystemIpAddress));
        }
    }

    /// <summary>
    /// Verifies that the connection string constructor with HostName only correctly sets the value for the ConnectionString property.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestConnectionStringConstructorWithHostNameProperlySetsConnectionString()
    {
        ShimXboxConsole.ConstructorString = null;
        ShimXboxConsole.AllInstances.GetIPAddressFromConnectionStringString = (console, connectionString) =>
        {
            return IPAddress.Parse(ConsoleSystemIpAddress);
        };

        using (XboxConsole xboxConsole = new XboxConsole(ConsoleHostName))
        {
            Assert.AreEqual(ConsoleHostName, xboxConsole.ConnectionString);
        }
    }

    /// <summary>
    /// Verifies that the connection string constructor with IP address only correctly sets the value for the ConnectionString property.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestConnectionStringConstructorWithIPAddressProperlySetsConnectionString()
    {
        ShimXboxConsole.ConstructorString = null;
        using (XboxConsole xboxConsole = new XboxConsole(ConsoleAddress))
        {
            Assert.AreEqual(ConsoleAddress, xboxConsole.ConnectionString, "The XboxConsole connection string was not set properly by Connection String constructor when initializing with IP Address.");
        }
    }

    /// <summary>
    /// Verifies that the default constructor correctly sets the value for the ConnectionString property.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestDefaultConstructorProperlySetsConnectionString()
    {
        ShimXboxConsole.DefaultConsoleGet = () => ConsoleCombinedIPAndSessionKey;
        ShimXboxConsole.Constructor = null;
        using (XboxConsole xboxConsole = new XboxConsole())
        {
            Assert.AreEqual(ConsoleCombinedIPAndSessionKey, xboxConsole.ConnectionString, "The XboxConsole connection string was not set properly by default constructor.");
        }
    }

    /// <summary>
    /// Verifies that the IP Address constructor correctly sets the value for the ConnectionString property.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestIPAddressConstructorProperlySetsConnectionString()
    {
        ShimXboxConsole.ConstructorIPAddress = null;
        using (XboxConsole xboxConsole = new XboxConsole(IPAddress.Parse(ConsoleAddress)))
        {
            Assert.AreEqual(ConsoleAddress, xboxConsole.ConnectionString, "The XboxConsole connection string was not set properly by IP Address constructor.");
        }
    }

    /// <summary>
    /// Verifies that the default constructor throws an XboxException if default console is not set.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    [ExpectedException(typeof(XboxException))]
    public void TestDefaultConstructorThrowsXboxExceptionWithDefaultConsoleNotSet()
    {
        ShimXboxConsole.DefaultConsoleGet = () => null;
        ShimXboxConsole.Constructor = null;
        using (XboxConsole xboxConsole = new XboxConsole())
        {
        }
    }

    /// <summary>
    /// Verifies that the default constructor correctly sets the value for the SystemIpAddress property using default console setting.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestDefaultConstructorProperlySetsIpAddress()
    {
        ShimXboxConsole.DefaultConsoleGet = () => ConsoleAddress;
        ShimXboxConsole.Constructor = null;
        using (XboxConsole xboxConsole = new XboxConsole())
        {
          Assert.AreEqual(IPAddress.Parse(ConsoleAddress), xboxConsole.SystemIpAddress, "The XboxConsole did not return the proper IP Address.");
          Assert.IsNull(xboxConsole.SessionKey, "The XboxConsole did not return the proper session key.");
          Assert.AreEqual(ConsoleAddress, xboxConsole.SystemIpAddressAndSessionKeyCombined, "The XboxConsole did not return the proper combined IP Address and session key.");
        }
    }

    /// <summary>
    /// Verifies that the default constructor correctly sets the value for the SystemIpAddress property using default console setting.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestDefaultConstructorProperlySetsIpAddressAndSessionKey()
    {
      ShimXboxConsole.DefaultConsoleGet = () => ConsoleCombinedIPAndSessionKey;
      ShimXboxConsole.Constructor = null;
      using (XboxConsole xboxConsole = new XboxConsole())
      {
        Assert.AreEqual(IPAddress.Parse(ConsoleAddress), xboxConsole.SystemIpAddress, "The XboxConsole did not return the proper IP Address.");
        Assert.AreEqual(ConsoleSessionKey, xboxConsole.SessionKey, "The XboxConsole did not return the proper IP Address.");
        Assert.AreEqual(ConsoleCombinedIPAndSessionKey, xboxConsole.SystemIpAddressAndSessionKeyCombined, "The XboxConsole did not return the proper combined IP Address and session key.");
      }
    }

    /// <summary>
    /// Verifies that the connection string constructor sets system IP correctly when IP is passed instead of host name.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestConnectionStringConstructorSetsSystemIpCorrectly()
    {
        ShimXboxConsole.ConstructorString = null;

        using (XboxConsole xboxConsole = new XboxConsole(ConsoleAddress))
        {
            Assert.AreEqual(ConsoleAddress, xboxConsole.SystemIpAddress.ToString(), "System IP address passed into connection string constructor was not set as the System IP address correctly.");
        }
    }

    /// <summary>
    /// Verifies that the connection string constructor with IP address correctly sets the value for the SessionKey property.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestConnectionStringConstructorWithIPAddressProperlySetsSessionKey()
    {
        ShimXboxConsole.ConstructorString = null;

        using (XboxConsole xboxConsole = new XboxConsole(ConsoleCombinedIPAndSessionKey))
        {
            Assert.AreEqual(ConsoleSessionKey, xboxConsole.SessionKey, "Connection string constructor with IP Address and Session Key did not set Session Key property correctly.");
        }
    }

    /// <summary>
    /// Verifies that the connection string constructor correctly sets connection string property.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestConnectionStringConstructorProperlySetsConnectionString()
    {
        ShimXboxConsole.ConstructorString = null;

        using (XboxConsole xboxConsole = new XboxConsole(ConsoleCombinedIPAndSessionKey))
        {
            Assert.AreEqual(ConsoleCombinedIPAndSessionKey, xboxConsole.ConnectionString, "Connection string constructor did not set Connection String property on the console correctly.");
        }
    }

    /// <summary>
    /// Verifies that the host name constructor throws an ArgumentNullException if the given host name is null.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TestConnectionStringConstructorThrowsArgumentNullExceptionWithNullString()
    {
        ShimXboxConsole.ConstructorString = null;

        using (XboxConsole xboxConsole = new XboxConsole((string)null))
        {
        }
    }

    /// <summary>
    /// Verifies that the adapter's Dispose() method is called when the XboxConsole is disposed.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestAdapterIsDisposedWhenXboxConsoleIsDisposed()
    {
      bool isDisposeCalled = false;
      this.stubAdapter.DisposeBoolean = _ => isDisposeCalled = true;

      this.console.Dispose();
      Assert.IsTrue(isDisposeCalled, "The XboxConsole Dispose method must call the adapter's Dispose method.");
    }

    /// <summary>
    /// Verifies that an ObjectDisposedException is thrown if the
    /// Configuration property is accessed after the XboxConsole object is disposed.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    [ExpectedException(typeof(ObjectDisposedException))]
    public void TestConfigurationThrowsObjectDisposedException()
    {
      this.stubAdapter.DisposeBoolean = _ => { };

      this.console.Dispose();
      var config = this.console.Configuration;
      Assert.Fail("ObjectDisposedException was not thrown.");
    }

    /// <summary>
    /// Verifies that an ObjectDisposedException is thrown if the
    /// SystemIpAddress is called after the XboxConsole object is disposed.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    [ExpectedException(typeof(ObjectDisposedException))]
    public void TestSystemIpAddressThrowsObjectDisposedException()
    {
      this.stubAdapter.DisposeBoolean = _ => { };

      ShimXboxConsole.ConstructorIPAddress = (realConsole, address) =>
      {
          var myShim = new ShimXboxConsole(realConsole)
          {
              AdapterGet = () => this.stubAdapter,
              SystemIpAddressStringGet = () => this.console.SystemIpAddress.ToString(),
              XboxGamepadsGet = () => this.gamepads
          };
      };

      this.console = new XboxConsole((IPAddress)null);
      this.console.Dispose();

      var systemIpAddress = this.console.SystemIpAddress;

      Assert.Fail("ObjectDisposedException was not thrown.");
    }

    /// <summary>
    /// Verifies that an ObjectDisposedException is thrown if the GetRunningProcesses(XboxOperatingSystem)
    /// method is called after the object has been disposed.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    [ExpectedException(typeof(ObjectDisposedException))]
    public void TestGetRunningProcessesThrowsObjectDisposedException()
    {
      this.stubAdapter.DisposeBoolean = _ => { };
      this.console.Dispose();
      var processes = this.console.GetRunningProcesses(XboxOperatingSystem.Title);
      Assert.Fail("ObjectDisposedException was not thrown.");
    }

    /// <summary>
    /// Verifies that the GetRunningProcesses(XboxOperatingSystem) method calls the adapter's GetRunningProcesses(XboxOperatingSystem) method.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestGetRunningProcessesCallsAdapterGetRunningProcesses()
    {
      bool isGetRunningProcessesCalled = false;
      ShimXboxConsoleAdapterBase.AllInstances.GetRunningProcessesStringXboxOperatingSystem = (adapter, systemIpAddress, operatingSystem) =>
      {
        isGetRunningProcessesCalled = true;
        return Enumerable.Empty<XboxProcessDefinition>();
      };

      var processes = this.console.GetRunningProcesses(XboxOperatingSystem.Title);
      Assert.IsTrue(isGetRunningProcessesCalled, "The XboxConsole GetRunningProcesses(XboxOperatingSystem) method must call the adapter's GetRunningProcesses(XboxOperatingSystem) method.");
      Assert.IsTrue(processes.Count() == 0, "The XboxConsole GetRunningProcesses(XboxOperatingSystem) didn't return enumeration of processes.");
    }

    /// <summary>
    /// Verifies that an ObjectDisposedException is thrown if the Reboot()
    /// method is called after the object has been disposed.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    [ExpectedException(typeof(ObjectDisposedException))]
    public void TestRebootThrowsObjectDisposedException()
    {
      this.stubAdapter.DisposeBoolean = _ => { };
      this.console.Dispose();
      this.console.Reboot();
    }

    /// <summary>
    /// Verifies that an ObjectDisposedException is thrown if the
    /// Reboot(TimeSpan) method is called after
    /// the XboxConsole object is disposed.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    [ExpectedException(typeof(ObjectDisposedException))]
    public void TestRebootTimeSpanThrowsObjectDisposedException()
    {
      this.stubAdapter.DisposeBoolean = _ => { };

      TimeSpan timeSpan = TimeSpan.FromSeconds(30.0);
      this.console.Dispose();
      this.console.Reboot(timeSpan);
    }

    /// <summary>
    /// Verifies that an ObjectDisposedException is thrown if the
    /// Shutdown() method is called after the XboxConsole object is disposed.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    [ExpectedException(typeof(ObjectDisposedException))]
    public void TestShutdownThrowsObjectDisposedException()
    {
      this.stubAdapter.DisposeBoolean = _ => { };

      this.console.Dispose();
      this.console.Shutdown();
    }

    /// <summary>
    /// Verifies that an ObjectDisposedException is thrown if the
    /// Shutdown(TimeSpan) is called after
    /// the XboxConsole object is disposed.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    [ExpectedException(typeof(ObjectDisposedException))]
    public void TestShutdownTimeSpanThrowsObjectDisposedException()
    {
      this.stubAdapter.DisposeBoolean = _ => { };

      TimeSpan timeSpan = TimeSpan.FromSeconds(30.0);
      this.console.Dispose();
      this.console.Shutdown(timeSpan);
    }

    /// <summary>
    /// Verifies that the DefaultConsole property throws ArgumentNullException if specified value is null.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TestDefaultConsoleThrowsArgumentNullException()
    {
      XboxConsole.DefaultConsole = null;
      Assert.Fail("ArgumentNullException was not thrown.");
    }

    /// <summary>
    /// Verifies that the DefaultConsole property calls the adapter's DefaultConsole property.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestDefaultConsoleCallsAdapterDefaultConsole()
    {
      bool isDefaultConsoleCalled = false;
      ShimXboxConsoleAdapterBase.AllInstances.DefaultConsoleGet = adapter =>
      {
        isDefaultConsoleCalled = true;
        return ConsoleAddress;
      };

      ShimXboxConsoleAdapterBase.AllInstances.DefaultConsoleSetString = (adapter, address) =>
      {
        isDefaultConsoleCalled = true;
        Assert.AreEqual(ConsoleAddress, address, "The XboxConsole DefaultConsole (set) property did not set correct value for the adapter's DefaultConsole (set) property.");
      };

      var systemIpAddress = XboxConsole.DefaultConsole;
      Assert.IsTrue(isDefaultConsoleCalled, "The XboxConsole DefaultConsole (get) property must call the adapter's DefaultConsole (get) property.");
      Assert.AreEqual(ConsoleAddress, systemIpAddress, "The XboxConsole DefaultConsole (get) property did not get correct value from the adapter's DefaultConsole (get) property.");

      isDefaultConsoleCalled = false;
      XboxConsole.DefaultConsole = ConsoleAddress;
      Assert.IsTrue(isDefaultConsoleCalled, "The XboxConsole DefaultConsole (set) property must call the adapter's DefaultConsole (set) property.");
    }

    /// <summary>
    /// Verifies that the Configuration property calls the ReadOnlyXboxConfiguration's constructor.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestConfigurationCallsReadOnlyXboxConfigurationConstructor()
    {
      bool isReadOnlyXboxConfigurationConstructorCalled = false;

      ShimReadOnlyXboxConfiguration.ConstructorFuncOfStringString = (console, getSettingValue) =>
      {
        isReadOnlyXboxConfigurationConstructorCalled = true;
      };

      using (XboxConsole c = new XboxConsole(IPAddress.Parse(ConsoleAddress)))
      {
        var config = c.Configuration;
      }

      Assert.IsTrue(isReadOnlyXboxConfigurationConstructorCalled, "XboxConsole Configurration property did not call the ReadOnlyXboxConfiguration's constructor.");
    }

    /// <summary>
    /// Verifies that the Configuration property calls the adapter's GetConfigValue function.
    /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Necessary for testing using the same logic as before when there were less settings.")]
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestConfigurationCallsAdapterGetConfigValue()
    {
      bool isGetConfigValueCalled = false;
      string testEnvironment = "Test Environment";
      string testSandboxId = "Test SandboxId";
      string testOOBECompleted = "false";
      string testProfilingMode = "legacy";
      string testPreferredLanguages = "ar-SA;th;en-US";
      string testGeographicRegion = "MX";
      string testTimeZone = TimeZoneInfo.Utc.Id;
      string testConnectedStorageForceOffline = "true";
      string testSimulateVersionSwitch = "true";
      string testEnableKernelDebugging = "true";
      string testSessionKey = "TestSessionKey";
      string testAccessoryFlags = "0";
      string testPowerMode = "EnergySaving";
      string testIdleShutdownTimeout = "360";
      string testDimTimeout = "6";
      string testHttpProxyHost = "10.161.24.254:8888";
      string testDisplayResolution = "HD";
      string testColorSpace = "RgbFull";
      string testColorDepth = "24";
      string testNetworkType = "Wireless";
      string testNetworkAddressMode = "Manual";
      string testDefaultUser = "user@xbox.com";
      string testDefaultUserPairing = "AnyPhysical";
      string testWirelessRadioSettings = "AccessoriesOff";
      string testHdmiAudio = "Bitstream";
      string testOpticalAudio = "Bitstream";
      string testAudioBitstreamFormat = "DolbyDigital";
      string testHostName = "TestHostName";
      ShimXboxConsole.ConstructorIPAddress = null;

      ShimXboxConsoleAdapterBase.AllInstances.GetConfigValueStringString = (adapter, systemIpAddress, key) =>
      {
        isGetConfigValueCalled = true;
        Assert.AreEqual(ConsoleAddress, systemIpAddress, "The XboxConsole Configuration (get) property did not pass correct system IP address to the adapter's GetConfigValue function.");

        // inelegant, but easy
        switch (key)
        {
          case "Environment": return testEnvironment;
          case "SandboxId": return testSandboxId;
          case "OOBECompleted": return testOOBECompleted;
          case "ProfilingMode": return testProfilingMode;
          case "PreferredLanguages": return testPreferredLanguages;
          case "GeographicRegion": return testGeographicRegion;
          case "TimeZone": return testTimeZone;
          case "ConnectedStorageForceOffline": return testConnectedStorageForceOffline;
          case "SimulateVersionSwitch": return testSimulateVersionSwitch;
          case "EnableKernelDebugging": return testEnableKernelDebugging;
          case "SessionKey": return testSessionKey;
          case "AccessoryFlags": return testAccessoryFlags;
          case "PowerMode": return testPowerMode;
          case "IdleShutdownTimeout": return testIdleShutdownTimeout;
          case "DimTimeout": return testDimTimeout;
          case "HttpProxyHost": return testHttpProxyHost;
          case "DisplayResolution": return testDisplayResolution;
          case "ColorSpace": return testColorSpace;
          case "ColorDepth": return testColorDepth;
          case "NetworkType": return testNetworkType;
          case "NetworkAddressMode": return testNetworkAddressMode;
          case "DefaultUser": return testDefaultUser;
          case "DefaultUserPairing": return testDefaultUserPairing;
          case "WirelessRadioSettings": return testWirelessRadioSettings;
          case "HDMIAudio": return testHdmiAudio;
          case "OpticalAudio": return testOpticalAudio;
          case "AudioBitstreamFormat": return testAudioBitstreamFormat;
          case "HostName": return testHostName;
          default: Assert.Fail("The XboxConsole Configuration (get) property did not return the proper key."); break;
        }

        return null;
      };

      using (XboxConsole xboxConsole = new XboxConsole(IPAddress.Parse(ConsoleAddress)))
      {
        string consoleSandboxId = xboxConsole.Configuration.SandboxId;
        Assert.IsTrue(isGetConfigValueCalled, "The XboxConsole Configuration (get) property must call the adapter's GetConfigValue function.");
        Assert.AreEqual(testSandboxId, consoleSandboxId, "The XboxConsole Configuration (get) property did not get correct value from the adapter's GetConfigValue function.");
      }
    }

    /// <summary>
    /// Verifies that the Reboot() function calls the adapter's Reboot(TimeSpan) function.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestRebootCallsAdapterReboot()
    {
      bool isRebootCalled = false;
      ShimXboxConsoleAdapterBase.AllInstances.RebootStringTimeSpan = (adapter, systemIpAddress, timeSpan) => isRebootCalled = true;

      this.console.Reboot();
      Assert.IsTrue(isRebootCalled, "The XboxConsole Reboot method must call the adapter's Reboot method.");
    }

    /// <summary>
    /// Verifies that the Reboot(XboxConfiguration) method calls the adapter's SetConfig method and then Adapter's reboot method. 
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestRebootXboxConfigurationSetsConfigurationAndCallsAdapterReboot()
    {
      this.TestRebootWithConfiguration(false);
    }

    /// <summary>
    /// Verifies that the Reboot(XboxConfiguration, TimeSpan) method calls the adapter's SetConfig method and then Adapter's Reboot(TimeSpan) method.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestRebootXboxConfigurationTimeSpanCallsAdapterSetConfigureThenReboot()
    {
      this.TestRebootWithConfiguration(true);
    }

    /// <summary>
    /// Verifies that the Reboot(TimeSpan) method calls the adapter's Reboot(TimeSpan) method.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestRebootTimeSpanCallsAdapterReboot()
    {
      bool isRebootCalled = false;
      ShimXboxConsoleAdapterBase.AllInstances.RebootStringTimeSpan = (adapter, systemIpAddress, timeSpan) => isRebootCalled = true;

      this.console.Reboot(TimeSpan.FromSeconds(30.0));
      Assert.IsTrue(isRebootCalled, "The XboxConsole Reboot method must call the adapter's Reboot method.");
    }

    /// <summary>
    /// Verifies that the Shutdown() method calls the adapter's Shutdown(TimeSpan) method.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestShutdownCallsAdapterShutdown()
    {
      bool isShutdownCalled = false;
      ShimXboxConsoleAdapterBase.AllInstances.ShutdownStringTimeSpan = (adapter, systemIpAddress, timeSpan) => isShutdownCalled = true;

      this.console.Shutdown();
      Assert.IsTrue(isShutdownCalled, "The XboxConsole Shutdown method must call the adapter's Shutdown method.");
    }

    /// <summary>
    /// Verifies that the Shutdown(TimeSpan) method calls the adapter's Shutdown(TimeSpan) method.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestShutdownTimeSpanCallsAdapterShutdown()
    {
      bool isShutdownCalled = false;
      ShimXboxConsoleAdapterBase.AllInstances.ShutdownStringTimeSpan = (adapter, systemIpAddress, timeSpan) => isShutdownCalled = true;

      this.console.Shutdown(TimeSpan.FromSeconds(30.0));
      Assert.IsTrue(isShutdownCalled, "The XboxConsole Shutdown method must call the adapter's Shutdown method.");
    }

    /// <summary>
    /// Verifies that the Reboot() calls the Reboot(TimeSpan) and passes
    /// it a value of Timeout.InfiniteTimeSpan.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestRebootCallsRebootTimeSpanWithInfiniteTimeout()
    {
      bool isFunctionCalled = false;
      bool isTimeSpanValid = false;
      ShimXboxConsole.AllInstances.RebootTimeSpan = (xboxConsole, timeout) =>
      {
        isFunctionCalled = true;

        if (timeout == Timeout.InfiniteTimeSpan)
        {
          isTimeSpanValid = true;
        }
      };

      this.console.Reboot();

      Assert.IsTrue(isFunctionCalled, "The Reboot method overload without a TimeSpan parameter did not call the Reboot method that takes a TimeSpan parameter.");
      Assert.IsTrue(isTimeSpanValid, "The Reboot method overload without a TimeSpan parameter did not pass Timeout.Infinite to the Reboot method overload that does take a TimeSpan parameter");
    }

    /// <summary>
    /// Verifies that the Shutdown() method calls the Shutdown(TimeSpan) and passes
    /// it a value of Timeout.InfiniteTimeSpan.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestShutdownCallsShutdownTimeSpanWithInfiniteTimeout()
    {
      bool isFunctionCalled = false;
      bool isTimeSpanValid = false;
      ShimXboxConsole.AllInstances.ShutdownTimeSpan = (xboxConsole, timeout) =>
      {
        isFunctionCalled = true;

        if (timeout == Timeout.InfiniteTimeSpan)
        {
          isTimeSpanValid = true;
        }
      };

      this.console.Shutdown();

      Assert.IsTrue(isFunctionCalled, "The Shutdown method overload without a TimeSpan parameter did not call the Shutdown method that takes a TimeSpan parameter.");
      Assert.IsTrue(isTimeSpanValid, "The Shutdown method overload without a TimeSpan parameter did not pass Timeout.Infinite to the Shutdown method overload that does take a TimeSpan parameter");
    }

    /// <summary>
    /// Verifies that if the adapter level returns a null value for the list of installed applications,
    /// then the XboxConsole level will return an empty list.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestInstalledPackagesReturnsEmptyIfAdapterReturnsNull()
    {
      ShimXboxConsoleAdapterBase.AllInstances.GetInstalledPackagesString = (adapter, systemIpAddress) => null;

#pragma warning disable 168 // Unused local
      var notUsed = this.console.InstalledPackages;
#pragma warning restore 168
      Assert.IsFalse(notUsed.Any(), "XboxConsole should have returned empty if the adatper returns null.");
    }

    /// <summary>
    /// Verifies that InstalledApplicationsGet calls the adapter's GetInstalledApplications(). 
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestInstalledPackagesCallsAdapterGetInstalledPackages()
    {
      bool isCorrectMethodCalled = false;
      ShimXboxConsoleAdapterBase.AllInstances.GetInstalledPackagesString = (adapter, systemIpAddress) =>
      {
        isCorrectMethodCalled = true;
        return new XboxPackageDefinition[] { };
      };

#pragma warning disable 168 // Unused local
      var notUsed = this.console.InstalledPackages;
#pragma warning restore 168
      Assert.IsTrue(isCorrectMethodCalled, "The XboxConsole object did not call the adapter's GetInstalledPackages method.");
    }

    /// <summary>
    /// Verifies that an ObjectDisposedException is thrown if the
    /// CreateGamepad is called after the XboxConsole object is disposed.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    [ExpectedException(typeof(ObjectDisposedException))]
    public void TestCreateGamepadThrowsObjectDisposedException()
    {
      this.stubAdapter.DisposeBoolean = _ => { };

      this.console.Dispose();
      var gamepad = this.console.CreateXboxGamepad();
    }

    /// <summary>
    /// Verifies that if the adapter level returns a null value for the list of users,
    /// then the XboxConsole level will return an empty list.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestUsersReturnsEmptyIfAdapterReturnsNull()
    {
        ShimXboxConsoleAdapterBase.AllInstances.GetUsersString = (adapter, systemIpAddress) => null;

        var users = this.console.Users;

        Assert.IsFalse(users.Any(), "XboxConsole should have returned empty if the adapter returns null.");
    }

    /// <summary>
    /// Verifies that Users get calls the adapter's GetUsers(). 
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestUsersCallsAdapterGetUsers()
    {
        bool isCorrectMethodCalled = false;
        ShimXboxConsoleAdapterBase.AllInstances.GetUsersString = (adapter, systemIpAddress) =>
        {
            isCorrectMethodCalled = true;
            return new XboxUserDefinition[] { };
        };

#pragma warning disable 168 // Unused local
        var notUsed = this.console.Users;
#pragma warning restore 168

        Assert.IsTrue(isCorrectMethodCalled, "The XboxConsole object did not call the adapter's GetUsers method.");
    }

    /// <summary>
    /// Verifies that AddGuestUser get calls the adapter's AddGuestUser(). 
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestAddGuestUserCallsAdaptersAddGuestUser()
    {
        bool isCorrectMethodCalled = false;
        ShimXboxConsoleAdapterBase.AllInstances.AddGuestUserString = (adapter, systemIpAddress) =>
        {
            isCorrectMethodCalled = true;
            return 0;
        };

        this.console.AddGuestUser();

        Assert.IsTrue(isCorrectMethodCalled, "The XboxConsole object did not call the adapter's AddGuestUser method.");
    }

    /// <summary>
    /// Verifies that AddUser get calls the adapter's AddUser(). 
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestAddUserCallsAdaptersAddUser()
    {
        bool isCorrectMethodCalled = false;
        ShimXboxConsoleAdapterBase.AllInstances.AddUserStringString = (adapter, systemIpAddress, emailAddress) =>
        {
            isCorrectMethodCalled = true;
            return new XboxUserDefinition(0, null, null, false);
        };

        this.console.AddUser(null);

        Assert.IsTrue(isCorrectMethodCalled, "The XboxConsole object did not call the adapter's AddUser method.");
    }

    /// <summary>
    /// Verifies that AddUser passes on its argument to the adapter level. 
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestAddUserPassesArguments()
    {
        const string ExpectedEmailAddress = "TestEmail@test.test";
        ShimXboxConsoleAdapterBase.AllInstances.AddUserStringString = (adapter, systemIpAddress, emailAddress) =>
        {
            Assert.AreEqual(ExpectedEmailAddress, emailAddress, "The email address passed in did not match the expected email");
            return new XboxUserDefinition(0, null, null, false);
        };

        this.console.AddUser(ExpectedEmailAddress);
    }

    /// <summary>
    /// Verifies that AddUser handles a null return value from the adapter.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    [ExpectedException(typeof(XboxConsoleException))]
    public void TestAddUserNullReturnValueThrowsException()
    {
        ShimXboxConsoleAdapterBase.AllInstances.AddUserStringString = (adapter, systemIpAddress, emailAddress) =>
        {
            return null;
        };

        this.console.AddUser(null);
    }

    /// <summary>
    /// Verifies that DeleteAllUsers get calls the adapter's DeleteAllUsers(). 
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestDeleteAllUsersCallsAdaptersDeleteAllUsers()
    {
        bool isCorrectMethodCalled = false;
        ShimXboxConsoleAdapterBase.AllInstances.DeleteAllUsersString = (adapter, systemIpAddress) =>
        {
            isCorrectMethodCalled = true;
        };

        this.console.DeleteAllUsers();

        Assert.IsTrue(isCorrectMethodCalled, "The XboxConsole object did not call the adapter's DeleteAllUsers method.");
    }

    /// <summary>
    /// Verifies that DeleteUser get calls the adapter's DeleteUser(). 
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestDeleteUserCallsAdaptersDeleteUsers()
    {
        bool isCorrectMethodCalled = false;
        ShimXboxConsoleAdapterBase.AllInstances.DeleteUserStringXboxUserDefinition = (adapter, systemIpAddress, userDefinition) =>
        {
            isCorrectMethodCalled = true;
        };

        XboxUser user = new XboxUser(this.console, 0, null, null, false);
        this.console.DeleteUser(user);

        Assert.IsTrue(isCorrectMethodCalled, "The XboxConsole object did not call the adapter's DeleteUser method.");
    }

    /// <summary>
    /// Verifies that DeleteUser passes on its argument to the adapter level. 
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestDeleteUserPassesArguments()
    {
        XboxUser user = new XboxUser(this.console, 0, null, null, false);

        ShimXboxConsoleAdapterBase.AllInstances.DeleteUserStringXboxUserDefinition = (adapter, systemIpAddress, userDefinition) =>
        {
            Assert.AreSame(user.Definition, userDefinition, "The user definition passed in did not match the expected definition.");
        };

        this.console.DeleteUser(user);
    }

    /// <summary>
    /// Verifies that DeleteUser throws an ArgumentNullException if null is passed in.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TestDeleteUserThrowsArgumentNullException()
    {
        ShimXboxConsoleAdapterBase.AllInstances.DeleteUserStringXboxUserDefinition = (adapter, systemIpAddress, userDefinition) =>
        {
            Assert.Fail("DeleteUser failed to throw exception before getting to adapter layer.");
        };

        this.console.DeleteUser(null);
    }

    /// <summary>
    /// Verifies that CaptureRecordedGameClip calls the adapters CaptureRecordedGameClip.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestCaptureRecordedGameClipCallsAdapterCaptureRecordedGameClip()
    {
        bool isCorrectMethodCalled = false;
        ShimXboxConsoleAdapterBase.AllInstances.CaptureRecordedGameClipStringStringUInt32 = (adapter, systemIpAddress, path, seconds) =>
        {
            isCorrectMethodCalled = true;
        };

        try
        {
            this.console.CaptureRecordedGameClip(TestOutputPath, 30);
        }
        catch (XboxConsoleException)
        {
            // We don't care what exception is thrown here.
        }

        Assert.IsTrue(isCorrectMethodCalled, "The XboxConsole object did not call the adapter's CaptureRecordedGameClip method.");
    }

    /// <summary>
    /// Verifies that CaptureRecordedGameClip throws an ArgumentException when called with null file path.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    [ExpectedException(typeof(ArgumentException))]
    public void TestCaptureRecordedGameClipThrowsArgumentExceptionOnNullPath()
    {
        ShimXboxConsoleAdapterBase.AllInstances.CaptureRecordedGameClipStringStringUInt32 = (adapter, systemIpAddress, path, seconds) =>
        {
        };

        this.console.CaptureRecordedGameClip(null, 30);
    }

    /// <summary>
    /// Verifies that CaptureRecordedGameClip throws an ArgumentException when called with an empty file path.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    [ExpectedException(typeof(ArgumentException))]
    public void TestCaptureRecordedGameClipThrowsArgumentExceptionOnEmptyPath()
    {
        ShimXboxConsoleAdapterBase.AllInstances.CaptureRecordedGameClipStringStringUInt32 = (adapter, systemIpAddress, path, seconds) =>
        {
        };

        this.console.CaptureRecordedGameClip(string.Empty, 30);
    }

    /// <summary>
    /// Verifies that CaptureScreenshot calls the adapters CaptureScreenshot.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestCaptureScreenshotCallsAdapterCaptureScreenshot()
    {
        bool isCorrectMethodCalled = false;
        ShimXboxConsoleAdapterBase.AllInstances.CaptureScreenshotString = (adapter, systemIpAddress) =>
        {
            isCorrectMethodCalled = true;
            return IntPtr.Zero;
        };

        try
        {
            this.console.CaptureScreenshot();
        }
        catch (XboxConsoleException)
        {
            // We don't care what exception is thrown here.
        }

        Assert.IsTrue(isCorrectMethodCalled, "The XboxConsole object did not call the adapter's CaptureScreenshot method.");
    }

    /// <summary>
    /// Verifies that CaptureScreenshot correctly creates a BitmapSource from an HBitmap.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestCaptureScreenshotExtractsHBitmap()
    {
        const int CreatedBitmapColor = unchecked((int)0xFFFF0000); // AKA Red

        ShimXboxConsoleAdapterBase.AllInstances.CaptureScreenshotString = (adapter, systemIpAddress) =>
        {
            return this.CreateBitmap(CreatedBitmapColor);
        };

        var bitmapSource = this.console.CaptureScreenshot();

        this.VerifyBitmapColor(CreatedBitmapColor, bitmapSource);
    }

    /// <summary>
    /// Verifies that ConsoleId get calls the adapter's GetConsoleInfo(). 
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestConsoleIdCallsAdapterGetConsoleInfo()
    {
        bool isCorrectMethodCalled = false;

        string expectedValue = "Expected Value";

        ShimXboxConsoleAdapterBase.AllInstances.GetConsoleInfoString = (adapter, systemIpAddress) =>
        {
            isCorrectMethodCalled = true;
            return new XboxConsoleInfo(null, null, null, expectedValue, XboxCertTypes.None, null, null);
        };

        var actualValue = this.console.ConsoleId;

        Assert.IsTrue(isCorrectMethodCalled, "The XboxConsole object did not call the adapter's GetConsoleInfo method.");
        Assert.AreEqual(expectedValue, actualValue, "The ConsoleId property did not return the expected value.");
    }

    /// <summary>
    /// Verifies that an ObjectDisposedException is thrown if the
    /// ConsoleId property is accessed after the XboxConsole object is disposed.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    [ExpectedException(typeof(ObjectDisposedException))]
    public void TestConsoleIdThrowsObjectDisposedException()
    {
        this.stubAdapter.DisposeBoolean = _ => { };

        this.console.Dispose();
        var notUsed = this.console.ConsoleId;
        Assert.Fail("ObjectDisposedException was not thrown.");
    }

    /// <summary>
    /// Verifies that DeviceId get calls the adapter's GetConsoleInfo(). 
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestDeviceIdCallsAdapterGetConsoleInfo()
    {
        bool isCorrectMethodCalled = false;

        string expectedValue = "Expected Value";

        ShimXboxConsoleAdapterBase.AllInstances.GetConsoleInfoString = (adapter, systemIpAddress) =>
        {
            isCorrectMethodCalled = true;
            return new XboxConsoleInfo(null, null, null, null, XboxCertTypes.None, null, expectedValue);
        };

        var actualValue = this.console.DeviceId;

        Assert.IsTrue(isCorrectMethodCalled, "The XboxConsole object did not call the adapter's GetConsoleInfo method.");
        Assert.AreEqual(expectedValue, actualValue, "The DeviceId property did not return the expected value.");
    }

    /// <summary>
    /// Verifies that an ObjectDisposedException is thrown if the
    /// DeviceId property is accessed after the XboxConsole object is disposed.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    [ExpectedException(typeof(ObjectDisposedException))]
    public void TestDeviceIdThrowsObjectDisposedException()
    {
        this.stubAdapter.DisposeBoolean = _ => { };

        this.console.Dispose();
        var notUsed = this.console.DeviceId;
        Assert.Fail("ObjectDisposedException was not thrown.");
    }

    /// <summary>
    /// Verifies that HostName get calls the adapter's GetConsoleInfo(). 
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestHostNameCallsAdapterGetConsoleInfo()
    {
        bool isCorrectMethodCalled = false;

        string expectedValue = "Expected Value";

        ShimXboxConsoleAdapterBase.AllInstances.GetConsoleInfoString = (adapter, systemIpAddress) =>
        {
            isCorrectMethodCalled = true;
            return new XboxConsoleInfo(null, null, null, null, XboxCertTypes.None, expectedValue, null);
        };

        var actualValue = this.console.HostName;

        Assert.IsTrue(isCorrectMethodCalled, "The XboxConsole object did not call the adapter's GetConsoleInfo method.");
        Assert.AreEqual(expectedValue, actualValue, "The HostName property did not return the expected value.");
    }

    /// <summary>
    /// Verifies that an ObjectDisposedException is thrown if the
    /// HostName property is accessed after the XboxConsole object is disposed.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    [ExpectedException(typeof(ObjectDisposedException))]
    public void TestHostNameThrowsObjectDisposedException()
    {
        this.stubAdapter.DisposeBoolean = _ => { };

        this.console.Dispose();
        var notUsed = this.console.HostName;
        Assert.Fail("ObjectDisposedException was not thrown.");
    }

    /// <summary>
    /// Verifies that CertType get calls the adapter's GetConsoleInfo(). 
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestCertTypeCallsAdapterGetConsoleInfo()
    {
        bool isCorrectMethodCalled = false;

        XboxCertTypes expectedValue = XboxCertTypes.EraTestKit | XboxCertTypes.Other;

        ShimXboxConsoleAdapterBase.AllInstances.GetConsoleInfoString = (adapter, systemIpAddress) =>
        {
            isCorrectMethodCalled = true;
            return new XboxConsoleInfo(null, null, null, null, expectedValue, null, null);
        };

        var actualValue = this.console.CertType;

        Assert.IsTrue(isCorrectMethodCalled, "The XboxConsole object did not call the adapter's GetConsoleInfo method.");
        Assert.AreEqual(expectedValue, actualValue, "The CertType property did not return the expected value.");
    }

    /// <summary>
    /// Verifies that an ObjectDisposedException is thrown if the
    /// CertType property is accessed after the XboxConsole object is disposed.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    [ExpectedException(typeof(ObjectDisposedException))]
    public void TestCertTypeThrowsObjectDisposedException()
    {
        this.stubAdapter.DisposeBoolean = _ => { };

        this.console.Dispose();
        var notUsed = this.console.CertType;
        Assert.Fail("ObjectDisposedException was not thrown.");
    }

    /// <summary>
    /// Verifies that RegisterPackage get calls the adapter's RegisterPackage(). 
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestRegisterPackageCallsAdapterRegisterPackage()
    {
        bool isCorrectMethodCalled = false;

        string expectedPath = "ExpectedPath";
        XboxPackageDefinition expectedValue = new XboxPackageDefinition(PackageFullName, new string[] { Aumid });

        ShimXboxConsoleAdapterBase.AllInstances.RegisterPackageStringString = (adapter, systemIpAddress, packagePath) =>
        {
            isCorrectMethodCalled = true;
            return expectedValue;
        };

        var actualValue = this.console.RegisterPackage(new XboxPath(expectedPath, XboxOperatingSystem.Title));

        Assert.IsTrue(isCorrectMethodCalled, "The XboxConsole object did not call the adapter's RegisterPackage method.");
        Assert.AreSame(expectedValue, actualValue.Definition, "The RegisterPackage method did not return the expected value.");
    }

    /// <summary>
    /// Verifies that an ObjectDisposedException is thrown if the
    /// RegisterPackage method is accessed after the XboxConsole object is disposed.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    [ExpectedException(typeof(ObjectDisposedException))]
    public void TestRegisterPackageThrowsObjectDisposedException()
    {
        this.stubAdapter.DisposeBoolean = _ => { };

        this.console.Dispose();
        var notUsed = this.console.RegisterPackage(new XboxPath("Doesn't matter", XboxOperatingSystem.Title));
        Assert.Fail("ObjectDisposedException was not thrown.");
    }

    /// <summary>
    /// Verifies that GetAvailableSpaceForAppInstallation get calls the adapter's GetAvailableSpaceForAppInstallation(). 
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    public void TestGetAvailableSpaceForAppInstallationCallsAdapterRegisterPackage()
    {
        bool isCorrectMethodCalled = false;

        ulong expectedValue = 1234;

        ShimXboxConsoleAdapterBase.AllInstances.GetAvailableSpaceForAppInstallationStringString = (adapter, systemIpAddress, storageName) =>
        {
            isCorrectMethodCalled = true;
            Assert.IsNull(storageName, "XboxConsole didn't pass null to adapter.");
            return expectedValue;
        };

        var actualValue = this.console.GetAvailableSpaceForAppInstallation();

        Assert.IsTrue(isCorrectMethodCalled, "The XboxConsole object did not call the adapter's GetAvailableSpaceForAppInstallation method.");
        Assert.AreEqual(expectedValue, actualValue, "The GetAvailableSpaceForAppInstallation method did not return the expected value.");
    }

    /// <summary>
    /// Verifies that an ObjectDisposedException is thrown if the
    /// GetAvailableSpaceForAppInstallation method is accessed after the XboxConsole object is disposed.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConsoleTestCategory)]
    [ExpectedException(typeof(ObjectDisposedException))]
    public void TestGetAvailableSpaceForAppInstallationThrowsObjectDisposedException()
    {
        this.stubAdapter.DisposeBoolean = _ => { };

        this.console.Dispose();
        var notUsed = this.console.GetAvailableSpaceForAppInstallation();
        Assert.Fail("ObjectDisposedException was not thrown.");
    }

    /// <summary>
    /// Verifies that the specified BitmapSource is all the specified color.
    /// </summary>
    /// <param name="bitmapColor">The color to check the bitmap against.</param>
    /// <param name="bitmapSource">The BitmapSource to check.</param>
    private void VerifyBitmapColor(int bitmapColor, System.Windows.Media.Imaging.BitmapSource bitmapSource)
    {
        int height = bitmapSource.PixelHeight;
        int width = bitmapSource.PixelWidth;
        int[] pixels = new int[height * width];

        var bytesPerPixel = (bitmapSource.Format.BitsPerPixel + 7) / 8;

        int a = pixels[0];
        bitmapSource.CopyPixels(pixels, bytesPerPixel * width, 0);

        // Lets search for any pixel from the expected color.
        Assert.IsFalse(pixels.Any(x => x != bitmapColor), "The screenshot did not match the expected one.");
    }

    /// <summary>
    /// Creates a bitmap with a specified color.
    /// </summary>
    /// <param name="bitmapColor">The Color to make the created Bitmap.</param>
    /// <returns>An IntPtr pointing to the created bitmap.</returns>
    private IntPtr CreateBitmap(int bitmapColor)
    {
        using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(16, 16))
        {
            for (int x = 0; x < bitmap.Height; ++x)
            {
                for (int y = 0; y < bitmap.Width; ++y)
                {
                    bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(bitmapColor));
                }
            }

            return bitmap.GetHbitmap();
        }
    }

    private void TestRebootWithConfiguration(bool withTimeSpan)
    {
      bool isConfigurationApplied = false;
      bool isRebootCalled = false;
      TimeSpan timeout = TimeSpan.FromSeconds(30.0);

      XboxConfiguration customConfiguration = new XboxConfiguration(this.console.Configuration);
      customConfiguration.SandboxId = "Test ID";

      // Fake for Adapter Reboot(TimeSpan) method. Use Shim to mock it 
      ShimXboxConsoleAdapterBase.AllInstances.RebootStringStringTimeSpan = (adapter, originalIpAddress, newIpAddress, timeSpan) =>
      {
        isRebootCalled = true;
        if (withTimeSpan == true)
        {
          Assert.AreEqual(timeout, timeSpan, "The reboot overload with Configuration and TimeSpan parameters did not pass the correct timeout value to the adapter's reboot function.");
        }
        else
        {
          Assert.AreEqual(Timeout.InfiniteTimeSpan, timeSpan, "The reboot overload with the Configuration parameter did not pass InfiniteTimeSpan as the parameter to the adapter's reboot function.");
        }
      };

      ShimXboxConfiguration.AllInstances.SetSettingValuesActionOfStringString = (config, setSettingValue) =>
      {
        // We want to make sure Reboot is called AFTER applying the config
        isRebootCalled = false;
        isConfigurationApplied = true;
      };

      // Call Reboot with the configuration
      if (withTimeSpan == true)
      {
        this.console.Reboot(customConfiguration, timeout);
      }
      else
      {
        this.console.Reboot(customConfiguration);
      }

      Assert.IsTrue(isConfigurationApplied, "The XboxConsole Reboot that takes a configuration must call the adapter's SetConfigValue method.");
      Assert.IsTrue(isRebootCalled, "The XboxConsole Reboot method that takes a configuration must call the adapter's Reboot method after setting the configuration.");
    }
  }
}
