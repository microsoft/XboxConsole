//------------------------------------------------------------------------------
// <copyright file="XboxConfiguration.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Configuration
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Xml;
    using Microsoft.Internal.GamesTest.Xbox.Exceptions;
    using Microsoft.Internal.GamesTest.Xbox.Telemetry;

    /// <summary>
    /// The set of settings in Xbox configuration (see xbconfig command line utility).
    /// Call XboxConsole.Reboot method to apply configuration to a console.
    /// </summary>
    public class XboxConfiguration : BaseXboxConfiguration, IXboxConfiguration
    {
        private static readonly Lazy<System.Xml.Schema.XmlSchema> serializationSchema = new Lazy<System.Xml.Schema.XmlSchema>(() =>
            {
                using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Microsoft.Internal.GamesTest.Xbox.Configuration.XboxSettingsSchema.xsd"))
                {
                    return System.Xml.Schema.XmlSchema.Read(stream, (o, a) => { });
                }
            });

        /// <summary>
        /// Initializes a new instance of the XboxConfiguration class. Call XboxConsole.Reboot method to apply configuration to a console.
        /// </summary>
        public XboxConfiguration()
        {
            XboxConsoleEventSource.Logger.ObjectCreated(XboxConsoleEventSource.GetCurrentConstructor());
        }

        /// <summary>
        /// Initializes a new instance of the XboxConfiguration class and initialized settings using the source configuration. Call XboxConsole.Reboot method to apply configuration to a console.
        /// </summary>
        /// <param name="source">The source configuration used to initialize the settings in this class.</param>
        public XboxConfiguration(IXboxConfiguration source)
        {
            XboxConsoleEventSource.Logger.ObjectCreated(XboxConsoleEventSource.GetCurrentConstructor());

            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            this.Environment = source.Environment;
            this.SandboxId = source.SandboxId;
            this.OOBECompleted = source.OOBECompleted;
            this.ProfilingMode = source.ProfilingMode;
            this.PreferredLanguages = source.PreferredLanguages;
            this.GeographicRegion = source.GeographicRegion;
            this.TimeZone = source.TimeZone;
            this.SimulateVersionSwitch = source.SimulateVersionSwitch;
            this.EnableKernelDebugging = source.EnableKernelDebugging;
            this.SessionKey = source.SessionKey;
            this.HostName = source.HostName;
            this.AccessoryFlags = source.AccessoryFlags;
            this.PowerMode = source.PowerMode;
            this.IdleShutdownTimeout = source.IdleShutdownTimeout;
            this.DimTimeout = source.DimTimeout;
            this.HttpProxyHost = source.HttpProxyHost;
            this.DisplayResolution = source.DisplayResolution;
            this.ColorSpace = source.ColorSpace;
            this.ColorDepth = source.ColorDepth;
            this.DefaultUser = source.DefaultUser;
            this.DefaultUserPairing = source.DefaultUserPairing;
            this.WirelessRadioSettings = source.WirelessRadioSettings;
            this.HdmiAudio = source.HdmiAudio;
            this.OpticalAudio = source.OpticalAudio;
            this.AudioBitstreamFormat = source.AudioBitstreamFormat;
            this.DebugMemoryMode = source.DebugMemoryMode;
            this.DisableSelectiveSuspend = source.DisableSelectiveSuspend;
            this.DevkitAllowACG = source.DevkitAllowACG;
            this.AutoBoot = source.AutoBoot;
        }

        /// <summary>
        /// Gets the schema used for validating the serialized XboxConfiguration.
        /// </summary>
        public static System.Xml.Schema.XmlSchema SerializationSchema
        {
            get { return serializationSchema.Value; }
        }

        /// <summary>
        /// Gets or sets the Environment. Call XboxConsole.Reboot method to apply configuration to a console.
        /// </summary>
        public string Environment
        {
            get
            {
                return this.EnvironmentSetting.Value;
            }

            set
            {
                this.EnvironmentSetting.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the SandboxId. Call XboxConsole.Reboot method to apply configuration to a console.
        /// </summary>
        public string SandboxId
        {
            get
            {
                return this.SandboxIdSetting.Value;
            }

            set
            {
                this.SandboxIdSetting.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the OOBECompleted value. Call XboxConsole.Reboot method to apply configuration to a console.
        /// </summary>
        public bool? OOBECompleted
        {
            get
            {
                return this.OOBECompletedSetting.Value;
            }

            set
            {
                this.OOBECompletedSetting.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the ProfilingMode value. Call XboxConsole.Reboot method to apply configuration to a console.
        /// </summary>
        public ProfilingModeType ProfilingMode
        {
            get
            {
                return this.ProfilingModeSetting.Value;
            }

            set
            {
                this.ProfilingModeSetting.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the PreferredLanguages. Call XboxConsole.Reboot method to apply configuration to a console.
        /// </summary>
        public System.Collections.Generic.IEnumerable<System.Globalization.CultureInfo> PreferredLanguages
        {
            get
            {
                return this.PreferredLanguagesSetting.Value;
            }

            set
            {
                this.PreferredLanguagesSetting.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the GeographicRegion. Call XboxConsole.Reboot method to apply configuration to a console.
        /// </summary>
        public System.Globalization.RegionInfo GeographicRegion
        {
            get
            {
                return this.GeographicRegionSetting.Value;
            }

            set
            {
                this.GeographicRegionSetting.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the TimeZone. Call XboxConsole.Reboot method to apply configuration to a console.
        /// </summary>
        public TimeZoneInfo TimeZone
        {
            get
            {
                return this.TimeZoneSetting.Value;
            }

            set
            {
                this.TimeZoneSetting.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the ConnectedStorageForceOffline value. Call XboxConsole.Reboot method to apply configuration to a console.
        /// </summary>
        [Obsolete("This setting has been removed starting from April 2014 QFE")]
        public bool? ConnectedStorageForceOffline
        {
            get
            {
                return this.ConnectedStorageForceOfflineSetting.Value;
            }

            set
            {
                this.ConnectedStorageForceOfflineSetting.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the SimulateVersionSwitch value. Call XboxConsole.Reboot method to apply configuration to a console.
        /// </summary>
        public bool? SimulateVersionSwitch
        {
            get
            {
                return this.SimulateVersionSwitchSetting.Value;
            }

            set
            {
                this.SimulateVersionSwitchSetting.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the EnableKernelDebugging value. Call XboxConsole.Reboot method to apply configuration to a console.
        /// </summary>
        public bool? EnableKernelDebugging
        {
            get
            {
                return this.EnableKernelDebuggingSetting.Value;
            }

            set
            {
                this.EnableKernelDebuggingSetting.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the SessionKey value. Call XboxConsole.Reboot method to apply configuration to a console. 
        /// </summary>
        /// <remarks>
        /// The session key can either string.Empty, or an alphanumeric string 31 characters or less.
        /// </remarks>
        public string SessionKey
        {
            get
            {
                return this.SessionKeySetting.Value;
            }

            set
            {
                this.SessionKeySetting.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the AccessoryFlags value. Call XboxConsole.Reboot method to apply configuration to a console.
        /// </summary>
        public uint AccessoryFlags
        {
            get
            {
                return this.AccessoryFlagsSetting.Value;
            }

            set
            {
                this.AccessoryFlagsSetting.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the PowerMode value. Call XboxConsole.Reboot method to apply configuration to a console.
        /// </summary>
        public PowerModeType PowerMode
        {
            get
            {
                return this.PowerModeSetting.Value;
            }

            set
            {
                this.PowerModeSetting.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the IdleShutdownTimeout value. Call XboxConsole.Reboot method to apply configuration to a console.
        /// </summary>
        public IdleShutdownTimeoutType IdleShutdownTimeout
        {
            get
            {
                return this.IdleShutdownTimeoutSetting.Value;
            }

            set
            {
                this.IdleShutdownTimeoutSetting.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the DimTimeout value. Call XboxConsole.Reboot method to apply configuration to a console.
        /// </summary>
        public int DimTimeout
        {
            get
            {
                return this.DimTimeoutSetting.Value;
            }

            set
            {
                this.DimTimeoutSetting.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the HttpProxyHost value. Call XboxConsole.Reboot method to apply configuration to a console.
        /// </summary>
        public string HttpProxyHost
        {
            get
            {
                return this.HttpProxyHostSetting.Value;
            }

            set
            {
                this.HttpProxyHostSetting.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the DisplayResolution value. Call XboxConsole.Reboot method to apply configuration to a console.
        /// </summary>
        public DisplayResolutionType DisplayResolution
        {
            get
            {
                return this.DisplayResolutionSetting.Value;
            }

            set
            {
                this.DisplayResolutionSetting.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the ColorSpace value. Call XboxConsole.Reboot method to apply configuration to a console.
        /// </summary>
        public ColorSpaceType ColorSpace
        {
            get
            {
                return this.ColorSpaceSetting.Value;
            }

            set
            {
                this.ColorSpaceSetting.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the ColorDepth value. Call XboxConsole.Reboot method to apply configuration to a console.
        /// </summary>
        public ColorDepthType ColorDepth
        {
            get
            {
                return this.ColorDepthSetting.Value;
            }

            set
            {
                this.ColorDepthSetting.Value = value;
            }
        }

        /// <summary>
        /// Gets the NetworkType value.
        /// </summary>
        public NetworkTypeType NetworkType
        {
            get
            {
                return this.NetworkTypeSetting.Value;
            }
        }

        /// <summary>
        /// Gets the NetworkAddressMode value.
        /// </summary>
        public NetworkAddressModeType NetworkAddressMode
        {
            get
            {
                return this.NetworkAddressModeSetting.Value;
            }
        }

        /// <summary>
        /// Gets or sets the DefaultUser value. Call XboxConsole.Reboot method to apply configuration to a console.
        /// </summary>
        public string DefaultUser
        {
            get
            {
                return this.DefaultUserSetting.Value;
            }

            set
            {
                this.DefaultUserSetting.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the DefaultUserPairing value. Call XboxConsole.Reboot method to apply configuration to a console.
        /// </summary>
        public UserPairingType DefaultUserPairing
        {
            get
            {
                return this.DefaultUserPairingSetting.Value;
            }

            set
            {
                this.DefaultUserPairingSetting.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the HostName value. Call XboxConsole.Reboot method to apply configuration to a console. 
        /// </summary>
        /// <remarks>
        /// The host name can either be string.Empty, or an alphanumeric string 31 characters or less.
        /// </remarks>
        public string HostName
        {
            get
            {
                return this.HostNameSetting.Value;
            }

            set
            {
                this.HostNameSetting.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the WirelessRadioSettings value. Call XboxConsole.Reboot method to apply configuration to a console.
        /// </summary>
        public WirelessRadioSettingsType WirelessRadioSettings
        {
            get
            {
                return this.WirelessRadioSettingsSetting.Value;
            }

            set
            {
                this.WirelessRadioSettingsSetting.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the HdmiAudio value. Call XboxConsole.Reboot method to apply configuration to a console.
        /// </summary>
        public HdmiAudioOutput HdmiAudio
        {
            get
            {
                return this.HdmiAudioSetting.Value;
            }

            set
            {
                this.HdmiAudioSetting.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the OpticalAudio value. Call XboxConsole.Reboot method to apply configuration to a console.
        /// </summary>
        public OpticalAudioOutput OpticalAudio
        {
            get
            {
                return this.OpticalAudioSetting.Value;
            }

            set
            {
                this.OpticalAudioSetting.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the AudioBitstreamFormat value. Call XboxConsole.Reboot method to apply configuration to a console.
        /// </summary>
        public AudioBitstreamFormatType AudioBitstreamFormat
        {
            get
            {
                return this.AudioBitstreamFormatSetting.Value;
            }

            set
            {
                this.AudioBitstreamFormatSetting.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the DebugMemoryMode value. Call XboxConsole.Reboot method to apply configuration to a console. 
        /// </summary>
        public DebugMemoryModeType DebugMemoryMode
        {
            get
            {
                return this.DebugMemoryModeSetting.Value;
            }

            set
            {
                this.DebugMemoryModeSetting.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the DisableSelectiveSuspend value. Call XboxConsole.Reboot method to apply configuration to a console.
        /// </summary>
        public bool? DisableSelectiveSuspend
        {
            get
            {
                return this.DisableSelectiveSuspendSetting.Value;
            }

            set
            {
                this.DisableSelectiveSuspendSetting.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the DevkitAllowACG value. Call XboxConsole.Reboot method to apply configuration to a console.
        /// </summary>
        public bool? DevkitAllowACG
        {
            get
            {
                return this.DevkitAllowACGSetting.Value;
            }

            set
            {
                this.DevkitAllowACGSetting.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the AutoBoot value. Call XboxConsole.Reboot method to apply configuration to a console.
        /// </summary>
        public bool? AutoBoot
        {
            get
            {
                return this.AutoBootSetting.Value;
            }

            set
            {
                this.AutoBootSetting.Value = value;
            }
        }

        /// <summary>
        /// Gets the MACAddress value. Call XboxConsole.Reboot method to apply configuration to a console. 
        /// </summary>
        /// <remarks>
        /// The MACAddress is formatted as a sequence of six 2-digit hex bytes separated by dashes.
        /// </remarks>
        public string MACAddress
        {
            get
            {
                return this.MACAddressSetting.Value;
            }
        }

        /// <summary>
        /// Gets the DNSServer value. Call XboxConsole.Reboot method to apply configuration to a console. 
        /// </summary>
        /// <remarks>
        /// The DNSServer can be one or two IPv4 addresses separated by a comma.
        /// </remarks>
        public string DNSServer
        {
            get
            {
                return this.DNSServerSetting.Value;
            }
        }

        /// <summary>
        /// Load the Xbox configuration from an XML file.
        /// </summary>
        /// <param name="path">The configuration file path.</param>
        public void Load(string path)
        {
            // Read XML document
            XmlDocument doc = new XmlDocument();
            doc.Schemas.Add(XboxConfiguration.serializationSchema.Value);
            doc.Load(path);

            // Validate
            if (doc.DocumentElement.Name != BaseXboxConfiguration.SettingsKey ||
                !doc.DocumentElement.HasAttribute(BaseXboxConfiguration.XdkVersionAttribute))
            {
                throw new FormatException("The document is not a valid configuration settings file.");
            }

            if (doc.DocumentElement.Attributes[BaseXboxConfiguration.XdkVersionAttribute].Value != XboxConsole.XdkVersion)
            {
                throw new XdkVersionMismatchException("Attempted to load Xbox Configuration saved on another XDK version.");
            }

            System.Collections.Generic.List<System.Xml.Schema.ValidationEventArgs> validationErrors = new System.Collections.Generic.List<System.Xml.Schema.ValidationEventArgs>();
            doc.Validate((o, e) => { validationErrors.Add(e); });

            if (validationErrors.Count > 0)
            {
                throw new XboxConfigurationValidationException("Validation of XboxConfiguration XML file failed", validationErrors);
            }

            // Load settings
            this.GetSettingValues((setting) =>
            {
                // Certain settings are read-only and will not exist in the configuration file.
                XmlNodeList nodes = doc.DocumentElement.SelectNodes(setting);
                return nodes.Count > 0 ? nodes[0].InnerText : null;
            });
        }

        /// <summary>
        /// Save the configuration to an XML file.
        /// </summary>
        /// <param name="path">The configuration file path.</param>
        public void Save(string path)
        {
            this.Save(path, XboxConsole.XdkVersion);
        }

        /// <summary>
        /// Returns an XboxConfigurationSetting object based on a provided key. Used for testing purposes only.
        /// </summary>
        /// <param name="key">A key corresponding to an XboxConfigurationSetting object.</param>
        /// <returns>The StringValue property of the desired XboxConfigurationSetting.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Refactoring will require adding a base class for all config settings and a dictionary to BaseConfiguration, footprint too large.")]
        internal string GetStringValueFromSettingKey(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            switch (key)
            {
                case "Environment": return this.EnvironmentSetting.StringValue;
                case "SandboxId": return this.SandboxIdSetting.StringValue;
                case "OOBECompleted": return this.OOBECompletedSetting.StringValue;
                case "ProfilingMode": return this.ProfilingModeSetting.StringValue;
                case "PreferredLanguages": return this.PreferredLanguagesSetting.StringValue;
                case "GeographicRegion": return this.GeographicRegionSetting.StringValue;
                case "TimeZone": return this.TimeZoneSetting.StringValue;
                case "SimulateVersionSwitch": return this.SimulateVersionSwitchSetting.StringValue;
                case "EnableKernelDebugging": return this.EnableKernelDebuggingSetting.StringValue;
                case "SessionKey": return this.SessionKeySetting.StringValue;
                case "HostName": return this.HostNameSetting.StringValue;
                case "AccessoryFlags": return this.AccessoryFlagsSetting.StringValue;
                case "PowerMode": return this.PowerModeSetting.StringValue;
                case "IdleShutdownTimeout": return this.IdleShutdownTimeoutSetting.StringValue;
                case "DimTimeout": return this.DimTimeoutSetting.StringValue;
                case "HttpProxyHost": return this.HttpProxyHostSetting.StringValue;
                case "DisplayResolution": return this.DisplayResolutionSetting.StringValue;
                case "ColorSpace": return this.ColorSpaceSetting.StringValue;
                case "ColorDepth": return this.ColorDepthSetting.StringValue;
                case "NetworkType": return this.NetworkTypeSetting.StringValue;
                case "NetworkAddressMode": return this.NetworkAddressModeSetting.StringValue;
                case "DefaultUser": return this.DefaultUserSetting.StringValue;
                case "DefaultUserPairing": return this.DefaultUserPairingSetting.StringValue;
                case "WirelessRadioSettings": return this.WirelessRadioSettingsSetting.StringValue;
                case "HDMIAudio": return this.HdmiAudioSetting.StringValue;
                case "OpticalAudio": return this.OpticalAudioSetting.StringValue;
                case "AudioBitstreamFormat": return this.AudioBitstreamFormatSetting.StringValue;
                case "DebugMemoryMode": return this.DebugMemoryModeSetting.StringValue;
                case "DisableSelectiveSuspend": return this.DisableSelectiveSuspendSetting.StringValue;
                case "DevkitAllowACG": return this.DevkitAllowACGSetting.StringValue;
                case "AutoBoot": return this.AutoBootSetting.StringValue;
                case "MACAddress": return this.MACAddressSetting.StringValue;
                case "DNSServer": return this.DNSServerSetting.StringValue;
                default: throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Provided key '{0}' does not correspond to any XboxConfigurationSettings.", key));
            }
        }
    }
}
