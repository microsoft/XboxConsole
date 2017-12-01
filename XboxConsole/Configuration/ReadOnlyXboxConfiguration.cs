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
            if (getSettingValue == null)
            {
                throw new ArgumentNullException("getSettingValue");
            }

            this.EnvironmentSetting.StringValue = getSettingValue(this.EnvironmentSetting.Key);
            this.SandboxIdSetting.StringValue = getSettingValue(this.SandboxIdSetting.Key);
            this.OOBECompletedSetting.StringValue = getSettingValue(this.OOBECompletedSetting.Key);
            this.ProfilingModeSetting.StringValue = getSettingValue(this.ProfilingModeSetting.Key);
            this.PreferredLanguagesSetting.StringValue = getSettingValue(this.PreferredLanguagesSetting.Key);
            this.GeographicRegionSetting.StringValue = getSettingValue(this.GeographicRegionSetting.Key);
            this.TimeZoneSetting.StringValue = getSettingValue(this.TimeZoneSetting.Key);
            this.SimulateVersionSwitchSetting.StringValue = getSettingValue(this.SimulateVersionSwitchSetting.Key);
            this.EnableKernelDebuggingSetting.StringValue = getSettingValue(this.EnableKernelDebuggingSetting.Key);
            this.SessionKeySetting.StringValue = getSettingValue(this.SessionKeySetting.Key);

            if (this.SessionKeySetting.StringValue == null)
            {
                this.SessionKeySetting.StringValue = string.Empty;
            }

            this.AccessoryFlagsSetting.StringValue = getSettingValue(this.AccessoryFlagsSetting.Key);
            this.PowerModeSetting.StringValue = getSettingValue(this.PowerModeSetting.Key);
            this.IdleShutdownTimeoutSetting.StringValue = getSettingValue(this.IdleShutdownTimeoutSetting.Key);
            this.DimTimeoutSetting.StringValue = getSettingValue(this.DimTimeoutSetting.Key);
            this.HttpProxyHostSetting.StringValue = getSettingValue(this.HttpProxyHostSetting.Key);

            if (this.HttpProxyHostSetting.StringValue == null)
            {
                this.HttpProxyHostSetting.StringValue = string.Empty;
            }

            this.DisplayResolutionSetting.StringValue = getSettingValue(this.DisplayResolutionSetting.Key);
            this.ColorSpaceSetting.StringValue = getSettingValue(this.ColorSpaceSetting.Key);
            this.ColorDepthSetting.StringValue = getSettingValue(this.ColorDepthSetting.Key);
            this.NetworkTypeSetting.StringValue = getSettingValue(this.NetworkTypeSetting.Key);
            this.NetworkAddressModeSetting.StringValue = getSettingValue(this.NetworkAddressModeSetting.Key);
            this.DefaultUserSetting.StringValue = getSettingValue(this.DefaultUserSetting.Key);

            if (this.DefaultUserSetting.StringValue == null)
            {
                this.DefaultUserSetting.StringValue = string.Empty;
            }

            this.DefaultUserPairingSetting.StringValue = getSettingValue(this.DefaultUserPairingSetting.Key);
            this.WirelessRadioSettingsSetting.StringValue = getSettingValue(this.WirelessRadioSettingsSetting.Key);
            this.AudioBitstreamFormatSetting.StringValue = getSettingValue(this.AudioBitstreamFormatSetting.Key);
            this.HdmiAudioSetting.StringValue = getSettingValue(this.HdmiAudioSetting.Key);
            this.OpticalAudioSetting.StringValue = getSettingValue(this.OpticalAudioSetting.Key);
            this.HostNameSetting.StringValue = getSettingValue(this.HostNameSetting.Key);
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
        /// Gets the ProfilingMode boolean.
        /// </summary>
        public bool? ProfilingMode
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
        public int ColorDepth
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
        /// Gets the HostName.
        /// </summary>
        public string HostName
        {
            get
            {
                return this.HostNameSetting.Value;
            }
        }
    }
}
