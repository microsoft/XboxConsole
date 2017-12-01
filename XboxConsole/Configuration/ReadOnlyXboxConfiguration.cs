//------------------------------------------------------------------------------
// <copyright file="ReadOnlyXboxConfiguration.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Configuration
{
    using System;

    /// <summary>
    /// The set of read-only settings in Xbox configuration (see xbconfig command line utility).
    /// </summary>
    internal class ReadOnlyXboxConfiguration : BaseXboxConfiguration, IXboxConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the ReadOnlyXboxConfiguration class.
        /// </summary>
        /// <param name="getSettingValue">The setting value provider.</param>
        internal ReadOnlyXboxConfiguration(Func<string, string> getSettingValue)
        {
            this.GetSettingValues(getSettingValue);
        }

        /// <summary>
        /// Gets the Environment.
        /// </summary>
        public string Environment
        {
            get
            {
                return this.EnvironmentSetting.Value;
            }
        }

        /// <summary>
        /// Gets the SandboxId.
        /// </summary>
        public string SandboxId
        {
            get
            {
                return this.SandboxIdSetting.Value;
            }
        }

        /// <summary>
        /// Gets the OOBECompleted boolean.
        /// </summary>
        public bool? OOBECompleted
        {
            get
            {
                return this.OOBECompletedSetting.Value;
            }
        }

        /// <summary>
        /// Gets the ProfilingMode enumeration.
        /// </summary>
        public ProfilingModeType ProfilingMode
        {
            get
            {
                return this.ProfilingModeSetting.Value;
            }
        }

        /// <summary>
        /// Gets the PreferredLanguages.
        /// </summary>
        public System.Collections.Generic.IEnumerable<System.Globalization.CultureInfo> PreferredLanguages
        {
            get
            {
                return this.PreferredLanguagesSetting.Value;
            }
        }

        /// <summary>
        /// Gets the GeographicRegion.
        /// </summary>
        public System.Globalization.RegionInfo GeographicRegion
        {
            get
            {
                return this.GeographicRegionSetting.Value;
            }
        }

        /// <summary>
        /// Gets the TimeZone.
        /// </summary>
        public TimeZoneInfo TimeZone
        {
            get
            {
                return this.TimeZoneSetting.Value;
            }
        }

        /// <summary>
        /// Gets the ConnectedStorageForceOffline boolean.
        /// </summary>
        [Obsolete("This setting has been removed starting from April 2014 QFE")]
        public bool? ConnectedStorageForceOffline
        {
            get
            {
                return this.ConnectedStorageForceOfflineSetting.Value;
            }
        }

        /// <summary>
        /// Gets the SimulateVersionSwitch boolean.
        /// </summary>
        public bool? SimulateVersionSwitch
        {
            get
            {
                return this.SimulateVersionSwitchSetting.Value;
            }
        }

        /// <summary>
        /// Gets the EnableKernelDebugging boolean.
        /// </summary>
        public bool? EnableKernelDebugging
        {
            get
            {
                return this.EnableKernelDebuggingSetting.Value;
            }
        }

        /// <summary>
        /// Gets the SessionKey.
        /// </summary>
        public string SessionKey
        {
            get
            {
                return this.SessionKeySetting.Value;
            }
        }

        /// <summary>
        /// Gets the AccessoryFlags.
        /// </summary>
        public uint AccessoryFlags
        {
            get
            {
                return this.AccessoryFlagsSetting.Value;
            }
        }

        /// <summary>
        /// Gets the PowerMode.
        /// </summary>
        public PowerModeType PowerMode
        {
            get
            {
                return this.PowerModeSetting.Value;
            }
        }

        /// <summary>
        /// Gets the IdleShutdownTimeout.
        /// </summary>
        public IdleShutdownTimeoutType IdleShutdownTimeout
        {
            get
            {
                return this.IdleShutdownTimeoutSetting.Value;
            }
        }

        /// <summary>
        /// Gets the DimTimeout.
        /// </summary>
        public int DimTimeout
        {
            get
            {
                return this.DimTimeoutSetting.Value;
            }
        }

        /// <summary>
        /// Gets the HttpProxyHost.
        /// </summary>
        public string HttpProxyHost
        {
            get
            {
                return this.HttpProxyHostSetting.Value;
            }
        }

        /// <summary>
        /// Gets the DisplayResolution.
        /// </summary>
        public DisplayResolutionType DisplayResolution
        {
            get
            {
                return this.DisplayResolutionSetting.Value;
            }
        }

        /// <summary>
        /// Gets the ColorSpace.
        /// </summary>
        public ColorSpaceType ColorSpace
        {
            get
            {
                return this.ColorSpaceSetting.Value;
            }
        }

        /// <summary>
        /// Gets the ColorDepth.
        /// </summary>
        public ColorDepthType ColorDepth
        {
            get
            {
                return this.ColorDepthSetting.Value;
            }
        }

        /// <summary>
        /// Gets the NetworkType.
        /// </summary>
        public NetworkTypeType NetworkType
        {
            get
            {
                return this.NetworkTypeSetting.Value;
            }
        }

        /// <summary>
        /// Gets the NetworkAddressMode.
        /// </summary>
        public NetworkAddressModeType NetworkAddressMode
        {
            get
            {
                return this.NetworkAddressModeSetting.Value;
            }
        }

        /// <summary>
        /// Gets the DefaultUser.
        /// </summary>
        public string DefaultUser
        {
            get
            {
                return this.DefaultUserSetting.Value;
            }
        }

        /// <summary>
        /// Gets the DefaultUserPairing.
        /// </summary>
        public UserPairingType DefaultUserPairing
        {
            get
            {
                return this.DefaultUserPairingSetting.Value;
            }
        }

        /// <summary>
        /// Gets the WirelessRadioSettings.
        /// </summary>
        public WirelessRadioSettingsType WirelessRadioSettings
        {
            get
            {
                return this.WirelessRadioSettingsSetting.Value;
            }
        }

        /// <summary>
        /// Gets the HdmiAudio.
        /// </summary>
        public HdmiAudioOutput HdmiAudio
        {
            get
            {
                return this.HdmiAudioSetting.Value;
            }
        }

        /// <summary>
        /// Gets the OpticalAudio.
        /// </summary>
        public OpticalAudioOutput OpticalAudio
        {
            get
            {
                return this.OpticalAudioSetting.Value;
            }
        }

        /// <summary>
        /// Gets the AudioBitstreamFormat.
        /// </summary>
        public AudioBitstreamFormatType AudioBitstreamFormat
        {
            get
            {
                return this.AudioBitstreamFormatSetting.Value;
            }
        }

        /// <summary>
        /// Gets the DebugMemoryMode.
        /// </summary>
        public DebugMemoryModeType DebugMemoryMode
        {
            get
            {
                return this.DebugMemoryModeSetting.Value;
            }
        }

        /// <summary>
        /// Gets the DisableSelectiveSuspend.
        /// </summary>
        public bool? DisableSelectiveSuspend
        {
            get
            {
                return this.DisableSelectiveSuspendSetting.Value;
            }
        }

        /// <summary>
        /// Gets the DevkitAllowACG.
        /// </summary>
        public bool? DevkitAllowACG
        {
            get
            {
                return this.DevkitAllowACGSetting.Value;
            }
        }

        /// <summary>
        /// Gets the AutoBoot.
        /// </summary>
        public bool? AutoBoot
        {
            get
            {
                return this.AutoBootSetting.Value;
            }
        }

        /// <summary>
        /// Gets the MACAddress.
        /// </summary>
        public string MACAddress
        {
            get
            {
                return this.MACAddressSetting.Value;
            }
        }

        /// <summary>
        /// Gets the DNSServer.
        /// </summary>
        public string DNSServer
        {
            get
            {
                return this.DNSServerSetting.Value;
            }
        }

        /// <summary>
        /// Gets the HostName.
        /// </summary>
        public string HostName
        {
            get
            {
                return this.HostNameSetting.Value;
            }
        }

        /// <summary>
        /// Loads the Xbox configuration from an XML file.
        /// </summary>
        /// <param name="path">The configuration file path.</param>
        public void Load(string path)
        {
            throw new InvalidOperationException("XboxConsole.Configuration describes the current configuration and cannot be loaded from file. Create a new XboxConfiguration, use its Load method, and apply it to XboxConsole using the Restart method.");
        }

        /// <summary>
        /// Saves the Xbox configuration to an XML file.
        /// </summary>
        /// <param name="path">The configuration file path.</param>
        public void Save(string path)
        {
            this.Save(path, XboxConsole.XdkVersion);
        }
    }
}
