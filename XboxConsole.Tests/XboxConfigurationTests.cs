//------------------------------------------------------------------------------
// <copyright file="XboxConfigurationTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Tests
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Linq;
  using System.Reflection;
  using Microsoft.Internal.GamesTest.Xbox.Configuration;
  using Microsoft.Internal.GamesTest.Xbox.Configuration.Fakes;
  using Microsoft.QualityTools.Testing.Fakes;
  using Microsoft.VisualStudio.TestTools.UnitTesting;

  /// <summary>
  /// Class to house tests for the XboxConfiguration class.
  /// </summary>
  [TestClass]
  public class XboxConfigurationTests
  {
    private const string XboxConfigurationTestCategory = "XboxConsole.XboxConfiguration";
    private bool callsConfigValueSet;
    private IDisposable shimsContext;

    /// <summary>
    /// Called once before each test to setup shim objects.
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
      this.shimsContext = ShimsContext.Create();
      this.CreateCustomKlingonCulture();
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
    /// Verifies that XboxConfiguration can be created with no parameters.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void XboxConfigurationConstructorTest()
    {
      var configuration = new XboxConfiguration();
      Assert.IsNotNull(configuration, "XboxConfiguration failed to initialize when created without any parameters");
    }

    /// <summary>
    /// Verifies that XboxConfiguration can be initialized using settings from a source configuration (copy). 
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void XboxConfigurationConstructorIXboxConfigurationTest()
    {
      XboxConfiguration sourceConfiguration = new XboxConfiguration();
      sourceConfiguration.SandboxId = "Sample ID";
      XboxConfiguration copyConfiguration = new XboxConfiguration(sourceConfiguration);

      Assert.IsNotNull(copyConfiguration, "XboxConfiguration failed to initialize when created with a source configuration");
      Assert.AreEqual(sourceConfiguration.SandboxId, copyConfiguration.SandboxId, "The new xboxconfiguration class's sandbox ID does not match the source's sandbox ID.");
    }

    /// <summary>
    /// Verifies that the XboxConfiguration constructor throws ArgumentNullException if the source parameter is null.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    [ExpectedException(typeof(ArgumentNullException), "XboxConfiguration's constructor did not throw an ArgumentNullException as expected.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Microsoft.Internal.GamesTest.Xbox.Configuration.XboxConfiguration", Justification = "This is to test that the constructor throws an exception when passed a null parameter. The instance is never created as the constructor should throw an exception.")]
    public void TestXboxConfigurationConstructorThrowsArgumentNullExceptionWithNullParameter()
    {
      XboxConfiguration notUsedConfig = new XboxConfiguration(null);
    }

    /// <summary>
    /// Verifies that the SetSettingValues throws ArgumentNullException if the parameter is null.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationSetSettingValuesThrowsArgumentNullExceptionWithNullParameter()
    {
      XboxConfiguration notUsedConfig = new XboxConfiguration();

      try
      {
        notUsedConfig.SetSettingValues(null);
        Assert.Fail("XboxConfiguration's SetSettinValues did not throw an ArgumentNullException as expected.");
      }
      catch (ArgumentNullException)
      {
      }
    }

    /// <summary>
    /// Verifies that the SetSettingValues calls the delegate and passes the correct parameters.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationSetSettingValuesCallsActionParameterCorrectly()
    {
      bool setSettingValueDelegateCalled = false;

      XboxConfiguration config = new XboxConfiguration();
      config.SandboxId = "Test ID";

      config.SetSettingValues(
          (key, value) =>
          {
            setSettingValueDelegateCalled = true;

            string stringValue = config.GetStringValueFromSettingKey(key);

            Assert.AreEqual(value, stringValue, "The SetSettingValues did not call the delegate with the correct key or was not provided the correct value.");
          });

      Assert.IsTrue(setSettingValueDelegateCalled, "The SetSettingValues did not call the delegate parameter.");
    }

    /// <summary>
    /// Verifies that the Environment property returns the correct value (calls get on XboxConfigurationSettings Value).
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationGetEnvironment()
    {
      this.TestGetter<string>(config => config.Environment, "Sample Environment", "Environment");
    }

    /// <summary>
    /// Verifies that the Environment property sets the value by setting XboxConfigurationSettings Value.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationSetEnvironment()
    {
      string testEnvironment = "Sample Environment";
      this.TestSetter<string>(testConfiguration => testConfiguration.Environment = testEnvironment, testEnvironment, "Environment");
    }

    /// <summary>
    /// Verifies that the Environment property gets the same value that it sets.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationEnvironment()
    {
      this.TestSetsAndGets<string>((testConfiguration, testValue) => { testConfiguration.Environment = testValue; return testConfiguration.Environment; }, "Test Environment", "Environment");
    }

    /// <summary>
    /// Verifies that the SandboxId property returns the correct value (calls get on XboxConfigurationSettings Value).
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationGetSandboxId()
    {
      this.TestGetter<string>(config => config.SandboxId, "Sample SandboxId", "SandboxId");
    }

    /// <summary>
    /// Verifies that the SandboxId property sets the value by setting XboxConfigurationSettings Value.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationSetSandboxId()
    {
      string testSandboxId = "Sample SandboxId";
      this.TestSetter<string>(testConfiguration => testConfiguration.SandboxId = testSandboxId, testSandboxId, "SandboxId");
    }

    /// <summary>
    /// Verifies that the SandboxId property gets the same value that it sets.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationSandboxId()
    {
      this.TestSetsAndGets<string>((testConfiguration, testValue) => { testConfiguration.SandboxId = testValue; return testConfiguration.SandboxId; }, "Test SandboxId", "SandboxId");
    }

    /// <summary>
    /// Verifies that the OOBECompleted property returns the correct boolean.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationGetOOBECompleted()
    {
      this.TestGetter<bool?>(config => config.OOBECompleted, false, "OOBECompleted");
    }

    /// <summary>
    /// Verifies that the OOBECompleted property sets the value by setting XboxConfigurationSettings Value.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationSetOOBECompleted()
    {
      this.TestSetter<bool?>(testConfiguration => testConfiguration.OOBECompleted = false, false, "OOBECompleted");
    }

    /// <summary>
    /// Verifies that the OOBECompleted property gets the same value that it sets.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationOOBECompleted()
    {
      this.TestSetsAndGets<bool?>((testConfiguration, testValue) => { testConfiguration.OOBECompleted = testValue; return testConfiguration.OOBECompleted; }, false, "OOBECompleted");
    }

    /// <summary>
    /// Verifies that the ProfilingMode property returns the correct boolean.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationGetProfilingMode()
    {
      this.TestGetter<ProfilingModeType>(config => config.ProfilingMode, ProfilingModeType.Legacy, "ProfilingMode");
    }

    /// <summary>
    /// Verifies that the ProfilingMode property sets the value by setting XboxConfigurationSettings Value.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationSetProfilingMode()
    {
        this.TestSetter<ProfilingModeType>(testConfiguration => testConfiguration.ProfilingMode = ProfilingModeType.Off, ProfilingModeType.Off, "ProfilingMode");
    }

    /// <summary>
    /// Verifies that the ProfilingMode property gets the same value that it sets.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationProfilingMode()
    {
      this.TestSetsAndGets<ProfilingModeType>((testConfiguration, testValue) => { testConfiguration.ProfilingMode = testValue; return testConfiguration.ProfilingMode; }, ProfilingModeType.Legacy, "ProfilingMode");
    }

    /// <summary>
    /// Verifies that the PreferredLanguages property throws the correct exception when receiving a list containing
    /// unsupported CultureInfo classes.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    [ExpectedException(typeof(ArgumentException))]
    public void TestXboxConfigurationSetGarbagePreferredLanguages()
    {
      XboxConfiguration testConfiguration = new XboxConfiguration();
      testConfiguration.PreferredLanguages = new[] { new CultureInfo("Klingon") };
    }

    /// <summary>
    /// Verifies that the PreferredLanguages property returns the correct collection of CultureInfo classes.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationGetPreferredLanguages()
    {
      this.TestGetter<CultureInfo>(config => config.PreferredLanguages, CultureInfo.GetCultures(CultureTypes.SpecificCultures).Where(culture => culture.ToString().Contains("en")), "PreferredLanguages");
    }

    /// <summary>
    /// Verifies that the PreferredLanguages property sets the value by setting XboxConfigurationSettings Value.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationSetPreferredLanguages()
    {
      IEnumerable<CultureInfo> testPreferredLanguages =
          CultureInfo.GetCultures(CultureTypes.SpecificCultures).Where(culture => culture.ToString().Contains("en"));
      this.TestSetter<CultureInfo>(testConfiguration => testConfiguration.PreferredLanguages = testPreferredLanguages, testPreferredLanguages, "PreferredLanguages");
    }

    /// <summary>
    /// Verifies that the PreferredLanguages property gets the same value that it sets.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationPreferredLanguages()
    {
      IEnumerable<CultureInfo> testPreferredLanguages =
          CultureInfo.GetCultures(CultureTypes.SpecificCultures).Where(culture => culture.ToString().Contains("es"));
      this.TestSetsAndGets<CultureInfo>((testConfiguration, testValue) => { testConfiguration.PreferredLanguages = testValue; return testConfiguration.PreferredLanguages; }, testPreferredLanguages, "PreferredLanguages");
    }

    /// <summary>
    /// Verifies that the GeographicRegion property returns the correct RegionInfo class.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationGetGeographicRegion()
    {
      this.TestGetter<RegionInfo>(config => config.GeographicRegion, new RegionInfo("MX"), "GeographicRegion");
    }

    /// <summary>
    /// Verifies that the GeographicRegion property sets the value by setting XboxConfigurationSettings Value.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationSetGeographicRegion()
    {
      RegionInfo testGeographicRegion = new RegionInfo("TH");
      this.TestSetter<RegionInfo>(testConfiguration => testConfiguration.GeographicRegion = testGeographicRegion, testGeographicRegion, "GeographicRegion");
    }

    /// <summary>
    /// Verifies that the GeographicRegion property gets the same value that it sets.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationGeographicRegion()
    {
      this.TestSetsAndGets<RegionInfo>((testConfiguration, testValue) => { testConfiguration.GeographicRegion = testValue; return testConfiguration.GeographicRegion; }, new RegionInfo("TH"), "GeographicRegion");
    }

    /// <summary>
    /// Verifies that the TimeZone property returns the correct TimeZoneInfo class.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationGetTimeZone()
    {
      this.TestGetter<TimeZoneInfo>(config => config.TimeZone, TimeZoneInfo.Utc, "TimeZone");
    }

    /// <summary>
    /// Verifies that the TimeZone property sets the value by setting XboxConfigurationSettings Value.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationSetTimeZone()
    {
      this.TestSetter<TimeZoneInfo>(testConfiguration => testConfiguration.TimeZone = TimeZoneInfo.Utc, TimeZoneInfo.Utc, "TimeZone");
    }

    /// <summary>
    /// Verifies that the TimeZone property gets the same value that it sets.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationTimeZone()
    {
      this.TestSetsAndGets<TimeZoneInfo>((testConfiguration, testValue) => { testConfiguration.TimeZone = testValue; return testConfiguration.TimeZone; }, TimeZoneInfo.Utc, "TimeZone");
    }

    /// <summary>
    /// Verifies that the ConnectedStorageForceOffline property returns the correct boolean.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationGetConnectedStorageForceOffline()
    {
        // Disable the "obsolete" warning when using ConnectedStorageForceOffline in this test.
#pragma warning disable 0618
      
      this.TestGetter<bool?>(config => config.ConnectedStorageForceOffline, true, "ConnectedStorageForceOffline");

#pragma warning restore 0618
    }

    /// <summary>
    /// Verifies that the ConnectedStorageForceOffline property sets the value by setting XboxConfigurationSettings Value.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationSetConnectedStorageForceOffline()
    {
        // Disable the "obsolete" warning when using ConnectedStorageForceOffline in this test.
#pragma warning disable 0618

      this.TestSetter<bool?>(testConfiguration => testConfiguration.ConnectedStorageForceOffline = true, true, "ConnectedStorageForceOffline");

#pragma warning restore 0618
    }

    /// <summary>
    /// Verifies that the ConnectedStorageForceOffline property gets the same value that it sets.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationConnectedStorageForceOffline()
    {
        // Disable the "obsolete" warning when using ConnectedStorageForceOffline in this test.
#pragma warning disable 0618

      this.TestSetsAndGets<bool?>((testConfiguration, testValue) => { testConfiguration.ConnectedStorageForceOffline = testValue; return testConfiguration.ConnectedStorageForceOffline; }, true, "ConnectedStorageForceOffline");

#pragma warning restore 0618
    }

    /// <summary>
    /// Verifies that the SimulateVersionSwitch property returns the correct boolean.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationGetSimulateVersionSwitch()
    {
      this.TestGetter<bool?>(config => config.SimulateVersionSwitch, true, "SimulateVersionSwitch");
    }

    /// <summary>
    /// Verifies that the SimulateVersionSwitch property sets the value by setting XboxConfigurationSettings Value.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationSetSimulateVersionSwitch()
    {
      this.TestSetter<bool?>(testConfiguration => testConfiguration.SimulateVersionSwitch = true, true, "SimulateVersionSwitch");
    }

    /// <summary>
    /// Verifies that the SimulateVersionSwitch property gets the same value that it sets.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationSimulateVersionSwitch()
    {
      this.TestSetsAndGets<bool?>((testConfiguration, testValue) => { testConfiguration.SimulateVersionSwitch = testValue; return testConfiguration.SimulateVersionSwitch; }, true, "SimulateVersionSwitch");
    }

    /// <summary>
    /// Verifies that the EnableKernelDebugging property returns the correct boolean.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationGetEnableKernelDebugging()
    {
      this.TestGetter<bool?>(config => config.EnableKernelDebugging, true, "EnableKernelDebugging");
    }

    /// <summary>
    /// Verifies that the EnableKernelDebugging property sets the value by setting XboxConfigurationSettings Value.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationSetEnableKernelDebugging()
    {
      this.TestSetter<bool?>(testConfig => testConfig.EnableKernelDebugging = true, true, "EnableKernelDebugging");
    }

    /// <summary>
    /// Verifies that the EnableKernelDebugging property gets the same value that it sets.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationEnableKernelDebugging()
    {
      this.TestSetsAndGets<bool?>((testConfiguration, testValue) => { testConfiguration.EnableKernelDebugging = testValue; return testConfiguration.EnableKernelDebugging; }, true, "EnableKernelDebugging");
    }

    /// <summary>
    /// Verifies that the SessionKey property returns the correct value (calls get on XboxConfigurationSettings Value).
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationGetSessionKey()
    {
        this.TestGetter<string>(config => config.SessionKey, "SampleSessionKey", "SessionKey");
    }

    /// <summary>
    /// Verifies that the SessionKey property sets the value by setting XboxConfigurationSettings Value.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationSetSessionKey()
    {
        string testSessionKey = "SampleSessionKey";
        this.TestSetter<string>(testConfiguration => testConfiguration.SessionKey = testSessionKey, testSessionKey, "SessionKey");
    }

    /// <summary>
    /// Verifies that the SessionKey property gets the same value that it sets.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationSessionKey()
    {
        this.TestSetsAndGets<string>((testConfiguration, testValue) => { testConfiguration.SessionKey = testValue; return testConfiguration.SessionKey; }, "TestSessionKey", "SessionKey");
        this.TestSetsAndGets<string>((testConfiguration, testValue) => { testConfiguration.SessionKey = testValue; return testConfiguration.SessionKey; }, string.Empty, "SessionKey");
        this.TestSetsAndGets<string>((testConfiguration, testValue) => { testConfiguration.SessionKey = testValue; return testConfiguration.SessionKey; }, null, "SessionKey");
    }

    /// <summary>
    /// Verifies that the SessionKey property gets the same value that it sets.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationSessionKeyThrowsArgumentException()
    {
        this.TestSetInvalidValueThrowsException<string, ArgumentException>((testConfiguration, testValue) => testConfiguration.SessionKey = testValue, "Non@lphanumeric", "SessionKey");
        this.TestSetInvalidValueThrowsException<string, ArgumentException>((testConfiguration, testValue) => testConfiguration.SessionKey = testValue, "Ano+her", "SessionKey");
        this.TestSetInvalidValueThrowsException<string, ArgumentException>((testConfiguration, testValue) => testConfiguration.SessionKey = testValue, "Lets try spaces", "SessionKey");
        this.TestSetInvalidValueThrowsException<string, ArgumentException>((testConfiguration, testValue) => testConfiguration.SessionKey = testValue, "ThisIsAReallyLongStringThatShouldBeLongerThanThirtyOneCharacters", "SessionKey");
    }

    /// <summary>
    /// Verifies that the HostName property returns the correct value (calls get on XboxConfigurationSettings Value).
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationGetHostName()
    {
        this.TestGetter<string>(config => config.HostName, "SampleHostName", "HostName");
    }

    /// <summary>
    /// Verifies that the HostName property sets the value by setting XboxConfigurationSettings Value.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationSetHostName()
    {
        string testHostName = "SampleHostName";
        this.TestSetter<string>(testConfiguration => testConfiguration.HostName = testHostName, testHostName, "HostName");
    }

    /// <summary>
    /// Verifies that the HostName property gets the same value that it sets.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationHostName()
    {
        this.TestSetsAndGets<string>((testConfiguration, testValue) => { testConfiguration.HostName = testValue; return testConfiguration.HostName; }, "TestHostName", "HostName");
        this.TestSetsAndGets<string>((testConfiguration, testValue) => { testConfiguration.HostName = testValue; return testConfiguration.HostName; }, string.Empty, "HostName");
        this.TestSetsAndGets<string>((testConfiguration, testValue) => { testConfiguration.HostName = testValue; return testConfiguration.HostName; }, null, "HostName");
    }

    /// <summary>
    /// Verifies that the HostName property throws exceptions on invalid value.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestXboxConfigurationHostNameThrowsArgumentException()
    {
        this.TestSetInvalidValueThrowsException<string, ArgumentException>((testConfiguration, testValue) => testConfiguration.HostName = testValue, "Non@lphanumeric", "HostName");
        this.TestSetInvalidValueThrowsException<string, ArgumentException>((testConfiguration, testValue) => testConfiguration.HostName = testValue, "Ano+her", "HostName");
        this.TestSetInvalidValueThrowsException<string, ArgumentException>((testConfiguration, testValue) => testConfiguration.HostName = testValue, "Lets try spaces", "HostName");
        this.TestSetInvalidValueThrowsException<string, ArgumentException>((testConfiguration, testValue) => testConfiguration.HostName = testValue, "ThisIsAReallyLongStringThatShouldBeLongerThanFifteenCharacters", "HostName");
        this.TestSetInvalidValueThrowsException<string, ArgumentException>((testConfiguration, testValue) => testConfiguration.HostName = testValue, "1eet", "HostName");
        this.TestSetInvalidValueThrowsException<string, ArgumentException>((testConfiguration, testValue) => testConfiguration.HostName = testValue, "justslightlylong", "HostName");
    }

    /// <summary>
    /// Generic function to test the setting of XboxConfigurationSetting objects.
    /// </summary>
    /// <typeparam name="T">The XboxConfigurationSetting represents strongly-typed data.</typeparam>
    /// <param name="propertySetter">The function used to set the value of the XboxConfigurationSetting object.</param>
    /// <param name="expectedValue">The value intended to be set to the XboxConfigurationSetting.</param>
    /// <param name="propertyName">The name of the XboxConfigurationSetting object being tested.</param>
    private void TestSetter<T>(Action<XboxConfiguration> propertySetter, T expectedValue, string propertyName)
    {
      this.callsConfigValueSet = false;

      ShimXboxConfigurationSetting<T>.AllInstances.ValueSetT0 = (configSetting, actualValue) =>
      {
        Assert.AreEqual(expectedValue, actualValue, string.Format("Setting the {0} property on XboxConfiguration called the right method but passed the wrong value.", propertyName));
        this.callsConfigValueSet = true;
      };

      XboxConfiguration testConfiguration = new XboxConfiguration();
      propertySetter(testConfiguration);

      Assert.IsTrue(this.callsConfigValueSet, string.Format("Setting the {0} of XboxConfiguation did not call the Value Set function of XboxConfigurationSetting.", propertyName));
    }

    /// <summary>
    /// Generic function to test the setting of XboxConfigurationSetting objects.
    /// </summary>
    /// <typeparam name="T">The XboxConfigurationSetting represents strongly-typed data.</typeparam>
    /// <param name="propertySetter">The function used to set the list of values of the XboxConfigurationSetting object.</param>
    /// <param name="expectedValue">A list of values intended to be set to the XboxConfigurationSetting.</param>
    /// <param name="propertyName">The name of the XboxConfigurationSetting object being tested.</param>
    private void TestSetter<T>(Action<XboxConfiguration> propertySetter, IEnumerable<T> expectedValue, string propertyName)
    {
      this.callsConfigValueSet = false;

      ShimXboxConfigurationSetting<IEnumerable<T>>.AllInstances.ValueSetT0 = (configSetting, actualValue) =>
      {
        CollectionAssert.AreEqual(expectedValue.ToList(), actualValue.ToList(), string.Format("Setting the {0} property on XboxConfiguration called the right method but passed the wrong value.", propertyName));
        this.callsConfigValueSet = true;
      };

      XboxConfiguration testConfiguration = new XboxConfiguration();
      propertySetter(testConfiguration);

      Assert.IsTrue(this.callsConfigValueSet, string.Format("Setting the {0} of XboxConfiguation did not call the Value Set function of XboxConfigurationSetting.", propertyName));
    }

    /// <summary>
    /// Generic function to test the getting of XboxConfigurationSetting objects.
    /// </summary>
    /// <typeparam name="T">The XboxConfigurationSetting represents strongly-typed data.</typeparam>
    /// <param name="propertyGetter">The function used to get the actual value of the XboxConfigurationSetting object.</param>
    /// <param name="expectedValue">The value expected to be returned by the XboxConfigurationSetting object.</param>
    /// <param name="propertyName">The name of the XboxConfigurationSetting object being tested.</param>
    private void TestGetter<T>(Func<XboxConfiguration, T> propertyGetter, T expectedValue, string propertyName)
    {
      ShimXboxConfigurationSetting<T>.AllInstances.ValueGet = (configSetting) =>
      {
        return expectedValue;
      };

      XboxConfiguration testConfiguration = new XboxConfiguration();
      T actualValue = propertyGetter(testConfiguration);
      Assert.AreEqual(expectedValue, actualValue, string.Format(CultureInfo.InvariantCulture, "The {0} property returned from XboxConfiguration is different from the {0} returned by the XboxConfigurationSetting class.", propertyName));
    }

    /// <summary>
    /// Generic function to test the getting of XboxConfigurationSetting objects.
    /// </summary>
    /// <typeparam name="T">The XboxConfigurationSetting represents strongly-typed data.</typeparam>
    /// <param name="propertyGetter">The function used to get the actual list of values of the XboxConfigurationSetting object.</param>
    /// <param name="expectedValue">The list of values expected to be returned by the XboxConfigurationSetting object.</param>
    /// <param name="propertyName">The name of the XboxConfigurationSetting object being tested.</param>
    private void TestGetter<T>(Func<XboxConfiguration, IEnumerable<T>> propertyGetter, IEnumerable<T> expectedValue, string propertyName)
    {
      ShimXboxConfigurationSetting<IEnumerable<T>>.AllInstances.ValueGet = (configSetting) =>
      {
        return expectedValue;
      };

      XboxConfiguration testConfiguration = new XboxConfiguration();
      IEnumerable<T> actualValue = propertyGetter(testConfiguration);
      CollectionAssert.AreEqual(expectedValue.ToList(), actualValue.ToList(), string.Format(CultureInfo.InvariantCulture, "The {0} property returned from XboxConfiguration is different from the {0} returned by the XboxConfigurationSetting class.", propertyName));
    }

    /// <summary>
    /// Generic function to test that XboxConfigurationSetting objects return the values assigned to them.
    /// </summary>
    /// <typeparam name="T">The XboxConfigurationSetting represents strongly-typed data.</typeparam>
    /// <param name="propertySetterAndGetter">The function used to set and get the value of the XboxConfigurationSetting object.</param>
    /// <param name="assignedValue">The value to be assigned to the XboxConfigurationSetting object.</param>
    /// <param name="propertyName">THe name of the XboxConfigurationObject being tested.</param>
    private void TestSetsAndGets<T>(Func<XboxConfiguration, T, T> propertySetterAndGetter, T assignedValue, string propertyName)
    {
      XboxConfiguration testConfiguration = new XboxConfiguration();
      T actualValue = propertySetterAndGetter(testConfiguration, assignedValue);
      Assert.AreEqual(assignedValue, actualValue, string.Format("The {0} that was returned is not the same as the {0} that was set", propertyName));
    }

    /// <summary>
    /// Generic function to test that setting XboxConfigurationSetting objects throws exception.
    /// </summary>
    /// <typeparam name="TProperty">The XboxConfigurationSetting represents strongly-typed data.</typeparam>
    /// <typeparam name="TException">The type of the exception to be thrown.</typeparam>
    /// <param name="propertySetter">The action used to set the value of the XboxConfigurationSetting object.</param>
    /// <param name="assignedValue">The value to be assigned to the XboxConfigurationSetting object.</param>
    /// <param name="propertyName">THe name of the XboxConfigurationObject being tested.</param>
    private void TestSetInvalidValueThrowsException<TProperty, TException>(Action<XboxConfiguration, TProperty> propertySetter, TProperty assignedValue, string propertyName) where TException : Exception
    {
        try
        {
            XboxConfiguration testConfiguration = new XboxConfiguration();
            propertySetter(testConfiguration, assignedValue);
            Assert.Fail(string.Format("Setting {0} to value {1} failed to throw exception of type {2}.", propertyName, assignedValue, typeof(TException)));
        }
        catch (TException)
        {
        }
    }

    /// <summary>
    /// Generic function to test that XboxConfigurationSetting objects return the values assigned to them.
    /// </summary>
    /// <typeparam name="T">The XboxConfigurationSetting represents strongly-typed data.</typeparam>
    /// <param name="propertySetterAndGetter">The function used to set and get the list of values of the XboxConfigurationSetting object.</param>
    /// <param name="assignedValue">The list of values to be assigned to the XboxConfigurationSetting object.</param>
    /// <param name="propertyName">THe name of the XboxConfigurationObject being tested.</param>
    private void TestSetsAndGets<T>(Func<XboxConfiguration, IEnumerable<T>, IEnumerable<T>> propertySetterAndGetter, IEnumerable<T> assignedValue, string propertyName)
    {
      XboxConfiguration testConfiguration = new XboxConfiguration();
      IEnumerable<T> actualValue = propertySetterAndGetter(testConfiguration, assignedValue);
      CollectionAssert.AreEqual(assignedValue.ToList(), actualValue.ToList(), string.Format("The {0} that was returned is not the same as the {0} that was set", propertyName));
    }

    /// <summary>
    /// Creates a user-defined CultureInfo and RegionInfo definition.
    /// </summary>
    private void CreateCustomKlingonCulture()
    {
        try
        {
            CultureAndRegionInfoBuilder builder = new CultureAndRegionInfoBuilder("Klingon", CultureAndRegionModifiers.None);
            builder.AvailableCalendars = new[] { CultureInfo.CurrentCulture.Calendar };
            builder.NumberFormat = new NumberFormatInfo();
            builder.GregorianDateTimeFormat = new DateTimeFormatInfo();
            builder.RegionEnglishName = "Kronos";
            builder.RegionNativeName = "Qo'noS";
            builder.ThreeLetterISOLanguageName = "kgn";
            builder.ThreeLetterISORegionName = "QOS";
            builder.TwoLetterISOLanguageName = "kn";
            builder.TwoLetterISORegionName = "QO";
            builder.ThreeLetterWindowsLanguageName = "kgn";
            builder.ThreeLetterWindowsRegionName = "KGN";
            builder.ISOCurrencySymbol = "DAR";
            builder.CurrencyNativeName = "Darsek";
            builder.CurrencyEnglishName = "Darsek";
            builder.TextInfo = TextInfo.ReadOnly(CultureInfo.CurrentCulture.TextInfo);
            builder.CompareInfo = CompareInfo.GetCompareInfo("en-US");
            builder.Parent = CultureInfo.InvariantCulture;
            builder.CultureNativeName = "Klingon";
            builder.CultureEnglishName = "Klingon";
            builder.Register();
        }
        catch (InvalidOperationException)
        {
            // this is thrown either if the culture already exists, or if one of the properties has not been assigned a value
            // since we know that all of the properties have been assigned a valid value, the only reason we'd catch this exception
            // is if the culture already exists; and that's fine
        }
    }
  }
}