//------------------------------------------------------------------------------
// <copyright file="BaseXboxConfiguration.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Configuration
{
    /// <summary>
    /// The base functionality of Xbox configuration classes.
    /// </summary>
    public class BaseXboxConfiguration
    {
        private const string EnvironmentKey = "Environment";
        private const string SandboxIdKey = "SandboxId";
        private const string OOBECompletedKey = "OOBECompleted";
        private const string ProfilingModeKey = "ProfilingMode";
        private const string PreferredLanguagesKey = "PreferredLanguages";
        private const string GeographicRegionKey = "GeographicRegion";
        private const string TimeZoneKey = "TimeZone";
        private const string ConnectedStorageForceOfflineKey = "ConnectedStorageForceOffline";
        private const string SimulateVersionSwitchKey = "SimulateVersionSwitch";
        private const string EnableKernelDebuggingKey = "EnableKernelDebugging";
        private const string SessionKeyKey = "SessionKey";
        private const string AccessoryFlagsKey = "AccessoryFlags";
        private const string PowerModeKey = "PowerMode";
        private const string IdleShutdownTimeoutKey = "IdleShutdownTimeout";
        private const string DimTimeoutKey = "DimTimeout";
        private const string HttpProxyHostKey = "HttpProxyHost";
        private const string DisplayResolutionKey = "DisplayResolution";
        private const string ColorSpaceKey = "ColorSpace";
        private const string ColorDepthKey = "ColorDepth";
        private const string NetworkTypeKey = "NetworkType";
        private const string NetworkAddressModeKey = "NetworkAddressMode";
        private const string DefaultUserKey = "DefaultUser";
        private const string DefaultUserPairingKey = "DefaultUserPairing";
        private const string WirelessRadioSettingsKey = "WirelessRadioSettings";
        private const string HdmiAudioKey = "HDMIAudio";
        private const string OpticalAudioKey = "OpticalAudio";
        private const string AudioBitstreamFormatKey = "AudioBitstreamFormat";
        private const string HostNameKey = "HostName";

        /// <summary>
        /// Initializes a new instance of the BaseXboxConfiguration class.
        /// </summary>
        internal BaseXboxConfiguration()
        {
            this.EnvironmentSetting = new XboxNonEmptyStringConfigurationSetting(EnvironmentKey);
            this.SandboxIdSetting = new XboxNonEmptyStringConfigurationSetting(SandboxIdKey);
            this.OOBECompletedSetting = new XboxConfigurationSetting<bool?>(OOBECompletedKey);
            this.ProfilingModeSetting = new XboxProfilingModeConfigurationSetting(ProfilingModeKey);
            this.PreferredLanguagesSetting = new XboxPreferredLanguagesConfigurationSetting(PreferredLanguagesKey);
            this.GeographicRegionSetting = new XboxGeographicRegionConfigurationSetting(GeographicRegionKey);
            this.TimeZoneSetting = new XboxTimeZoneConfigurationSetting(TimeZoneKey);
            this.ConnectedStorageForceOfflineSetting = new XboxConfigurationSetting<bool?>(ConnectedStorageForceOfflineKey);
            this.SimulateVersionSwitchSetting = new XboxConfigurationSetting<bool?>(SimulateVersionSwitchKey);
            this.EnableKernelDebuggingSetting = new XboxConfigurationSetting<bool?>(EnableKernelDebuggingKey);
            this.SessionKeySetting = new XboxSessionKeyConfigurationSetting(SessionKeyKey);
            this.AccessoryFlagsSetting = new XboxConfigurationSetting<uint>(AccessoryFlagsKey);
            this.PowerModeSetting = new XboxConfigurationSetting<PowerModeType>(PowerModeKey);
            this.IdleShutdownTimeoutSetting = new XboxIdleShutdownTimeoutConfigurationSetting(IdleShutdownTimeoutKey);
            this.DimTimeoutSetting = new XboxDimTimeoutConfigurationSetting(DimTimeoutKey);
            this.HttpProxyHostSetting = new XboxHttpProxyHostConfigurationSetting(HttpProxyHostKey);
            this.DisplayResolutionSetting = new XboxDisplayResolutionConfigurationSetting(DisplayResolutionKey);
            this.ColorSpaceSetting = new XboxConfigurationSetting<ColorSpaceType>(ColorSpaceKey);
            this.ColorDepthSetting = new XboxColorDepthConfigurationSetting(ColorDepthKey);
            this.NetworkTypeSetting = new XboxConfigurationSetting<NetworkTypeType>(NetworkTypeKey);
            this.NetworkAddressModeSetting = new XboxConfigurationSetting<NetworkAddressModeType>(NetworkAddressModeKey);
            this.DefaultUserSetting = new XboxDefaultUserConfigurationSetting(DefaultUserKey);
            this.DefaultUserPairingSetting = new XboxConfigurationSetting<UserPairingType>(DefaultUserPairingKey);
            this.WirelessRadioSettingsSetting = new XboxConfigurationSetting<WirelessRadioSettingsType>(WirelessRadioSettingsKey);
            this.HdmiAudioSetting = new XboxHdmiAudioConfigurationSetting(HdmiAudioKey);
            this.OpticalAudioSetting = new XboxConfigurationSetting<OpticalAudioOutput>(OpticalAudioKey);
            this.AudioBitstreamFormatSetting = new XboxConfigurationSetting<AudioBitstreamFormatType>(AudioBitstreamFormatKey);
            this.HostNameSetting = new XboxHostNameConfigurationSetting(HostNameKey);
        }

        /// <summary>
        /// Gets the Environment setting.
        /// </summary>
        internal XboxNonEmptyStringConfigurationSetting EnvironmentSetting { get; private set; }

        /// <summary>
        /// Gets the SandboxId setting.
        /// </summary>
        internal XboxNonEmptyStringConfigurationSetting SandboxIdSetting { get; private set; }

        /// <summary>
        /// Gets the OOBECompleted setting.
        /// </summary>
        internal XboxConfigurationSetting<bool?> OOBECompletedSetting { get; private set; }

        /// <summary>
        /// Gets the ProfilingMode setting.
        /// </summary>
        internal XboxProfilingModeConfigurationSetting ProfilingModeSetting { get; private set; }
        
        /// <summary>
        /// Gets the PreferredLanguages setting.
        /// </summary>
        internal XboxPreferredLanguagesConfigurationSetting PreferredLanguagesSetting { get; private set; }

        /// <summary>
        /// Gets the GeographicRegion setting.
        /// </summary>
        internal XboxGeographicRegionConfigurationSetting GeographicRegionSetting { get; private set; }

        /// <summary>
        /// Gets the TimeZone setting.
        /// </summary>
        internal XboxTimeZoneConfigurationSetting TimeZoneSetting { get; private set; }

        /// <summary>
        /// Gets the ConnectedStorageForceOffline setting.
        /// </summary>
        internal XboxConfigurationSetting<bool?> ConnectedStorageForceOfflineSetting { get; private set; }

        /// <summary>
        /// Gets the SimulateVersionSwitch setting.
        /// </summary>
        internal XboxConfigurationSetting<bool?> SimulateVersionSwitchSetting { get; private set; }

        /// <summary>
        /// Gets the EnableKernelDebugging setting.
        /// </summary>
        internal XboxConfigurationSetting<bool?> EnableKernelDebuggingSetting { get; private set; }

        /// <summary>
        /// Gets the SessionKey setting.
        /// </summary>
        internal XboxSessionKeyConfigurationSetting SessionKeySetting { get; private set; }

        /// <summary>
        /// Gets the AccessoryFlags setting.
        /// </summary>
        internal XboxConfigurationSetting<uint> AccessoryFlagsSetting { get; private set; }

        /// <summary>
        /// Gets the PowerMode setting.
        /// </summary>
        internal XboxConfigurationSetting<PowerModeType> PowerModeSetting { get; private set; }

        /// <summary>
        /// Gets the IdleShutdownTimeout setting.
        /// </summary>
        internal XboxIdleShutdownTimeoutConfigurationSetting IdleShutdownTimeoutSetting { get; private set; }

        /// <summary>
        /// Gets the DimTimeout setting.
        /// </summary>
        internal XboxDimTimeoutConfigurationSetting DimTimeoutSetting { get; private set; }

        /// <summary>
        /// Gets the HttpProxyHost setting.
        /// </summary>
        internal XboxHttpProxyHostConfigurationSetting HttpProxyHostSetting { get; private set; }

        /// <summary>
        /// Gets the DisplayResolution setting.
        /// </summary>
        internal XboxDisplayResolutionConfigurationSetting DisplayResolutionSetting { get; private set; }

        /// <summary>
        /// Gets the ColorSpace setting.
        /// </summary>
        internal XboxConfigurationSetting<ColorSpaceType> ColorSpaceSetting { get; private set; }

        /// <summary>
        /// Gets the ColorDepth setting.
        /// </summary>
        internal XboxColorDepthConfigurationSetting ColorDepthSetting { get; private set; }

        /// <summary>
        /// Gets the NetworkType setting.
        /// </summary>
        internal XboxConfigurationSetting<NetworkTypeType> NetworkTypeSetting { get; private set; }

        /// <summary>
        /// Gets the NetworkAddressMode setting.
        /// </summary>
        internal XboxConfigurationSetting<NetworkAddressModeType> NetworkAddressModeSetting { get; private set; }

        /// <summary>
        /// Gets the DefaultUser setting.
        /// </summary>
        internal XboxDefaultUserConfigurationSetting DefaultUserSetting { get; private set; }

        /// <summary>
        /// Gets the DefaultUserPairing setting.
        /// </summary>
        internal XboxConfigurationSetting<UserPairingType> DefaultUserPairingSetting { get; private set; }

        /// <summary>
        /// Gets the WirelessRadioSettings setting.
        /// </summary>
        internal XboxConfigurationSetting<WirelessRadioSettingsType> WirelessRadioSettingsSetting { get; private set; }

        /// <summary>
        /// Gets the HdmiAudio setting.
        /// </summary>
        internal XboxHdmiAudioConfigurationSetting HdmiAudioSetting { get; private set; }

        /// <summary>
        /// Gets the OpticalAudio setting.
        /// </summary>
        internal XboxConfigurationSetting<OpticalAudioOutput> OpticalAudioSetting { get; private set; }

        /// <summary>
        /// Gets the AudioBitstreamFormat setting.
        /// </summary>
        internal XboxConfigurationSetting<AudioBitstreamFormatType> AudioBitstreamFormatSetting { get; private set; }

        /// <summary>
        /// Gets the HostName setting.
        /// </summary>
        internal XboxHostNameConfigurationSetting HostNameSetting { get; private set; }
    }
}
