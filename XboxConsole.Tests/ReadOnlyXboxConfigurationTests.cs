//------------------------------------------------------------------------------
// <copyright file="ReadOnlyXboxConfigurationTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Tests
{
  using System;
  using System.Globalization;
  using System.Linq;
  using Microsoft.Internal.GamesTest.Xbox.Configuration;
  using Microsoft.VisualStudio.TestTools.UnitTesting;

  /// <summary>
  /// Class to house tests for the ReadOnlyXboxConfiguration class.
  /// </summary>
  [TestClass]
  public class ReadOnlyXboxConfigurationTests
  {
    private const string XboxConfigurationTestCategory = "XboxConsole.XboxConfiguration";
    private const string ConsoleAddress = "10.124.151.244"; // The actual IP address used here is irrelevant.

    /// <summary>
    /// Verifies that the ReadOnlyXboxConfiguration constructor throws ArgumentNullException if the getSettingValue parameter is null.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    [ExpectedException(typeof(ArgumentNullException), "ReadOnlyXboxConfiguration constructor did not throw an ArgumentNullException as expected.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Microsoft.Internal.GamesTest.Xbox.Configuration.ReadOnlyXboxConfiguration", Justification = "This is to test that the constructor throws an exception when passed a null parameter. The instance is never created as the constructor should throw an exception.")]
    public void TestReadOnlyXboxConfigurationConstructorThrowsArgumentNullExceptionWithNullGetSettingValueFunc()
    {
      ReadOnlyXboxConfiguration notUsedConfig = new ReadOnlyXboxConfiguration(null);
    }

    /// <summary>
    /// Verifies that the SandboxId property get functions correctly and the constructor calls the getSettingValue Func (parameter) to set the SandboxId.
    /// </summary>
    [TestMethod]
    [TestCategory("UnitTest")]
    [TestCategory(XboxConfigurationTestCategory)]
    public void TestReadOnlyXboxConfigurationGetsSandboxIdPropertyCorrectlyAndConstructorSetsSandboxIdProperty()
    {
      string sandboxIdValue = "Sample ID";
      bool isGetSettingValueFuncParameterCalled = false;

      // since this action will be called for ALL settings, and we're only testing whether or not
      // the SandboxId is set correctly, double check the configKey value
      ReadOnlyXboxConfiguration readOnlyConfig = new ReadOnlyXboxConfiguration(
          configKey =>
          {
            if (configKey == "SandboxId")
            {
              isGetSettingValueFuncParameterCalled = true;
              return sandboxIdValue;
            }
            else
            {
              return null;
            }
          });

      Assert.IsTrue(isGetSettingValueFuncParameterCalled, "ReadOnlyXboxConfiguration Constructor did not call the getSettingValue Func parameter.");
      Assert.AreEqual(sandboxIdValue, readOnlyConfig.SandboxId, "The ReadOnlyXboxConfiguration constructor did not set the value correctly.");
    }
  }
}
