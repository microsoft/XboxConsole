//------------------------------------------------------------------------------
// <copyright file="BaseXboxConfiguration.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Configuration
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Xml;

    /// <summary>
    /// The base functionality of Xbox configuration classes.
    /// </summary>
    public class BaseXboxConfiguration
    {
        /// <summary>
        /// Name of the settings root element in XML file used to store Xbox configuration.
        /// </summary>
        protected const string SettingsKey = "XboxSettings";

        /// <summary>
        /// Name of the attribute of the settings element used to store XDK version.
        /// </summary>
        protected const string XdkVersionAttribute = "xdk";

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
        private const string DebugMemoryModeKey = "DebugMemoryMode";
        private const string DisableSelectiveSuspendKey = "DisableSelectiveSuspend";
        private const string DevkitAllowACGKey = "DevkitAllowACG";
        private const string AutoBootKey = "AutoBoot";
        private const string MACAddressKey = "MACAddress";
        private const string DNSServerKey = "DNSServer";

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
            this.HttpProxyHostSetting = new XboxConfigurationSetting<string>(HttpProxyHostKey);
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
            this.DebugMemoryModeSetting = new XboxConfigurationSetting<DebugMemoryModeType>(DebugMemoryModeKey);
            this.DisableSelectiveSuspendSetting = new XboxConfigurationSetting<bool?>(DisableSelectiveSuspendKey);
            this.DevkitAllowACGSetting = new XboxDevkitAllowAcgConfigurationSetting(DevkitAllowACGKey);
            this.AutoBootSetting = new XboxConfigurationSetting<bool?>(AutoBootKey);
            this.MACAddressSetting = new XboxMacAddressConfigurationSetting(MACAddressKey);
            this.DNSServerSetting = new XboxDnsServerConfigurationSetting(DNSServerKey);
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
        internal XboxConfigurationSetting<string> HttpProxyHostSetting { get; private set; }

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
        /// Gets the DebugMemoryMode setting.
        /// </summary>
        internal XboxConfigurationSetting<DebugMemoryModeType> DebugMemoryModeSetting { get; private set; }

        /// <summary>
        /// Gets the DisableSelectiveSuspend setting.
        /// </summary>
        internal XboxConfigurationSetting<bool?> DisableSelectiveSuspendSetting { get; private set; }

        /// <summary>
        /// Gets the DevkitAllowACG setting.
        /// </summary>
        internal XboxDevkitAllowAcgConfigurationSetting DevkitAllowACGSetting { get; private set; }

        /// <summary>
        /// Gets the AutoBoot setting.
        /// </summary>
        internal XboxConfigurationSetting<bool?> AutoBootSetting { get; private set; }

        /// <summary>
        /// Gets the MACAddress setting.
        /// </summary>
        internal XboxMacAddressConfigurationSetting MACAddressSetting { get; private set; }

        /// <summary>
        /// Gets the DNSServer setting.
        /// </summary>
        internal XboxDnsServerConfigurationSetting DNSServerSetting { get; private set; }

        /// <summary>
        /// Gets the HostName setting.
        /// </summary>
        internal XboxHostNameConfigurationSetting HostNameSetting { get; private set; }

        /// <summary>
        /// Parses settings from strings returned by the specified functor.
        /// </summary>
        /// <param name="getSettingValue">Function called to get setting value given its name.</param>
        internal void GetSettingValues(Func<string, string> getSettingValue)
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
            this.AccessoryFlagsSetting.StringValue = getSettingValue(this.AccessoryFlagsSetting.Key);
            this.PowerModeSetting.StringValue = getSettingValue(this.PowerModeSetting.Key);
            this.IdleShutdownTimeoutSetting.StringValue = getSettingValue(this.IdleShutdownTimeoutSetting.Key);
            this.DimTimeoutSetting.StringValue = getSettingValue(this.DimTimeoutSetting.Key);
            this.HttpProxyHostSetting.StringValue = getSettingValue(this.HttpProxyHostSetting.Key);
            this.DisplayResolutionSetting.StringValue = getSettingValue(this.DisplayResolutionSetting.Key);
            this.ColorSpaceSetting.StringValue = getSettingValue(this.ColorSpaceSetting.Key);
            this.ColorDepthSetting.StringValue = getSettingValue(this.ColorDepthSetting.Key);
            this.NetworkTypeSetting.StringValue = getSettingValue(this.NetworkTypeSetting.Key);
            this.NetworkAddressModeSetting.StringValue = getSettingValue(this.NetworkAddressModeSetting.Key);
            this.DefaultUserSetting.StringValue = getSettingValue(this.DefaultUserSetting.Key);
            this.DefaultUserPairingSetting.StringValue = getSettingValue(this.DefaultUserPairingSetting.Key);
            this.WirelessRadioSettingsSetting.StringValue = getSettingValue(this.WirelessRadioSettingsSetting.Key);
            this.AudioBitstreamFormatSetting.StringValue = getSettingValue(this.AudioBitstreamFormatSetting.Key);
            this.HdmiAudioSetting.StringValue = getSettingValue(this.HdmiAudioSetting.Key);
            this.OpticalAudioSetting.StringValue = getSettingValue(this.OpticalAudioSetting.Key);
            this.DebugMemoryModeSetting.StringValue = getSettingValue(this.DebugMemoryModeSetting.Key);
            this.DisableSelectiveSuspendSetting.StringValue = getSettingValue(this.DisableSelectiveSuspendSetting.Key);
            this.DevkitAllowACGSetting.StringValue = getSettingValue(this.DevkitAllowACGSetting.Key);
            this.AutoBootSetting.StringValue = getSettingValue(this.AutoBootSetting.Key);
            this.MACAddressSetting.StringValue = getSettingValue(this.MACAddressSetting.Key);
            this.DNSServerSetting.StringValue = getSettingValue(this.DNSServerSetting.Key);
            this.HostNameSetting.StringValue = getSettingValue(this.HostNameSetting.Key);
        }

        /// <summary>
        /// Sets setting values using the specified action.
        /// </summary>
        /// <param name="setSettingValue">The action called for each setting.</param>
        internal void SetSettingValues(Action<string, string> setSettingValue)
        {
            if (setSettingValue == null)
            {
                throw new ArgumentNullException("setSettingValue");
            }

            // Converting the boolean strings to lowercase since the XML
            // Boolean type doesn't support uppercase values.
            var toLowerCase = new Func<string, string>((s) =>
            {
                // Switch statement used to get around dealing with CodeAnalysis error CA1308 : Microsoft.Globalization
                // which doesn't allow using the ToLower or ToLowerInvariant methods.
                switch (s)
                {
                    case "True": return "true";
                    case "False": return "false";
                    default: return s;
                }
            });

            setSettingValue(this.EnvironmentSetting.Key, this.EnvironmentSetting.StringValue);
            setSettingValue(this.SandboxIdSetting.Key, this.SandboxIdSetting.StringValue);
            setSettingValue(this.OOBECompletedSetting.Key, toLowerCase(this.OOBECompletedSetting.StringValue));
            setSettingValue(this.ProfilingModeSetting.Key, this.ProfilingModeSetting.StringValue);
            setSettingValue(this.PreferredLanguagesSetting.Key, this.PreferredLanguagesSetting.StringValue);
            setSettingValue(this.GeographicRegionSetting.Key, this.GeographicRegionSetting.StringValue);
            setSettingValue(this.TimeZoneSetting.Key, this.TimeZoneSetting.StringValue);
            setSettingValue(this.SimulateVersionSwitchSetting.Key, toLowerCase(this.SimulateVersionSwitchSetting.StringValue));
            setSettingValue(this.EnableKernelDebuggingSetting.Key, toLowerCase(this.EnableKernelDebuggingSetting.StringValue));
            setSettingValue(this.SessionKeySetting.Key, this.SessionKeySetting.StringValue);
            setSettingValue(this.AccessoryFlagsSetting.Key, this.AccessoryFlagsSetting.StringValue);
            setSettingValue(this.PowerModeSetting.Key, this.PowerModeSetting.StringValue);
            setSettingValue(this.IdleShutdownTimeoutSetting.Key, this.IdleShutdownTimeoutSetting.StringValue);
            setSettingValue(this.DimTimeoutSetting.Key, this.DimTimeoutSetting.StringValue);
            setSettingValue(this.HttpProxyHostSetting.Key, this.HttpProxyHostSetting.StringValue);
            setSettingValue(this.DisplayResolutionSetting.Key, this.DisplayResolutionSetting.StringValue);
            setSettingValue(this.ColorSpaceSetting.Key, this.ColorSpaceSetting.StringValue);
            setSettingValue(this.ColorDepthSetting.Key, this.ColorDepthSetting.StringValue);
            setSettingValue(this.DefaultUserSetting.Key, this.DefaultUserSetting.StringValue);
            setSettingValue(this.DefaultUserPairingSetting.Key, this.DefaultUserPairingSetting.StringValue);
            setSettingValue(this.WirelessRadioSettingsSetting.Key, this.WirelessRadioSettingsSetting.StringValue);
            setSettingValue(this.HdmiAudioSetting.Key, this.HdmiAudioSetting.StringValue);
            setSettingValue(this.OpticalAudioSetting.Key, this.OpticalAudioSetting.StringValue);
            setSettingValue(this.AudioBitstreamFormatSetting.Key, this.AudioBitstreamFormatSetting.StringValue);
            setSettingValue(this.DebugMemoryModeSetting.Key, this.DebugMemoryModeSetting.StringValue);
            setSettingValue(this.DisableSelectiveSuspendSetting.Key, toLowerCase(this.DisableSelectiveSuspendSetting.StringValue));
            setSettingValue(this.DevkitAllowACGSetting.Key, this.DevkitAllowACGSetting.StringValue);
            setSettingValue(this.AutoBootSetting.Key, toLowerCase(this.AutoBootSetting.StringValue));
            setSettingValue(this.HostNameSetting.Key, this.HostNameSetting.StringValue);
        }

        /// <summary>
        /// Saves the Xbox Configuration to a path using the given XDK version.
        /// </summary>
        /// <param name="path">The configuration file path.</param>
        /// <param name="xdkVersion">The XDK version to include in the file.</param>
        internal void Save(string path, string xdkVersion)
        {
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                
                using (XmlWriter writer = XmlWriter.Create(stream, settings))
                {
                    writer.WriteStartElement(SettingsKey);
                    writer.WriteAttributeString(XdkVersionAttribute, xdkVersion);

                    this.SetSettingValues((setting, value) =>
                    {
                        if (value != null)
                        {
                            writer.WriteElementString(setting, value);
                        }
                    });

                    writer.WriteEndElement();
                }
            }
        }
    }
}
