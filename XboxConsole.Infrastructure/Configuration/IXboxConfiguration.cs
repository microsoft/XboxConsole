//------------------------------------------------------------------------------
// <copyright file="IXboxConfiguration.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net;

    /// <summary>
    /// The interface for a set of system properties in configuration of Xbox (see xbconfig command line utility).
    /// </summary>
    public interface IXboxConfiguration
    {
        /// <summary>
        /// Gets the Environment.
        /// </summary>
        string Environment { get; }

        /// <summary>
        /// Gets the SandboxId.
        /// </summary>
        string SandboxId { get; }
        
        /// <summary>
        /// Gets the OOBECompleted boolean.
        /// </summary>
        bool? OOBECompleted { get; }

        /// <summary>
        /// Gets the ProfilingMode boolean.
        /// </summary>
        ProfilingModeType ProfilingMode { get; }

        /// <summary>
        /// Gets the PreferredLanguages.
        /// </summary>
        IEnumerable<CultureInfo> PreferredLanguages { get; }

        /// <summary>
        /// Gets the GeographicRegion.
        /// </summary>
        RegionInfo GeographicRegion { get; }

        /// <summary>
        /// Gets the TimeZone.
        /// </summary>
        TimeZoneInfo TimeZone { get; }

        /// <summary>
        /// Gets the ConnectedStorageForceOffline boolean.
        /// </summary>
        [Obsolete("This setting has been removed starting from April 2014 QFE")]
        bool? ConnectedStorageForceOffline { get; }

        /// <summary>
        /// Gets the SimulateVersionSwitch boolean.
        /// </summary>
        bool? SimulateVersionSwitch { get; }

        /// <summary>
        /// Gets the EnableKernelDebugging boolean.
        /// </summary>
        bool? EnableKernelDebugging { get; }

        /// <summary>
        /// Gets the AccessKey value. 
        /// </summary>
        string SessionKey { get; }

        /// <summary>
        /// Gets the AccessoryFlags value.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "AccessoryFlags is named according to corresponding config setting.")]
        uint AccessoryFlags { get; }

        /// <summary>
        /// Gets the PowerMode value.
        /// </summary>
        PowerModeType PowerMode { get; }

        /// <summary>
        /// Gets the IdleShutdownTimeout value.
        /// </summary>
        IdleShutdownTimeoutType IdleShutdownTimeout { get; }

        /// <summary>
        /// Gets the DimTimeout value.
        /// </summary>
        int DimTimeout { get; }

        /// <summary>
        /// Gets the HttpProxyHost value.
        /// </summary>
        string HttpProxyHost { get; }

        /// <summary>
        /// Gets the DisplayResolution value.
        /// </summary>
        DisplayResolutionType DisplayResolution { get; }

        /// <summary>
        /// Gets the ColorSpace value.
        /// </summary>
        ColorSpaceType ColorSpace { get; }

        /// <summary>
        /// Gets the ColorDepth value.
        /// </summary>
        ColorDepthType ColorDepth { get; }

        /// <summary>
        /// Gets the NetworkType value.
        /// </summary>
        NetworkTypeType NetworkType { get; }

        /// <summary>
        /// Gets the NetworkAddressMode value.
        /// </summary>
        NetworkAddressModeType NetworkAddressMode { get; }

        /// <summary>
        /// Gets the DefaultUser value.
        /// </summary>
        string DefaultUser { get; }

        /// <summary>
        /// Gets the DefaultUserPairing value.
        /// </summary>
        UserPairingType DefaultUserPairing { get; }

        /// <summary>
        /// Gets the WirelessRadioSettings value.
        /// </summary>
        WirelessRadioSettingsType WirelessRadioSettings { get; }

        /// <summary>
        /// Gets the HDMIAudio value.
        /// </summary>
        HdmiAudioOutput HdmiAudio { get; }

        /// <summary>
        /// Gets the OpticalAudio value.
        /// </summary>
        OpticalAudioOutput OpticalAudio { get; }

        /// <summary>
        /// Gets the AudioBitstreamFormat value.
        /// </summary>
        AudioBitstreamFormatType AudioBitstreamFormat { get; }

        /// <summary>
        /// Gets the DebugMemoryMode value.
        /// </summary>
        DebugMemoryModeType DebugMemoryMode { get; }

        /// <summary>
        /// Gets the DisableSelectiveSuspend value.
        /// </summary>
        bool? DisableSelectiveSuspend { get; }

        /// <summary>
        /// Gets the DevkitAllowACG value.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ACG", Justification = "Matches xbconfig setting"),
        System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Devkit", Justification = "Matches xbconfig setting")]
        bool? DevkitAllowACG { get; }

        /// <summary>
        /// Gets the AutoBoot value.
        /// </summary>
        bool? AutoBoot { get; }

        /// <summary>
        /// Gets the MACAddress value.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "MAC", Justification = "Matches xbconfig setting")]
        string MACAddress { get; }

        /// <summary>
        /// Gets the DNSServer value.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "DNS", Justification = "Matches xbconfig setting")]
        string DNSServer { get; }

        /// <summary>
        /// Gets the HostName value (network name).
        /// </summary>
        string HostName { get; }

        /// <summary>
        /// Load the Xbox configuration from an XML file.
        /// </summary>
        /// <param name="path">Configuration file path.</param>
        void Load(string path);

        /// <summary>
        /// Save the Xbox configuration to an XML file.
        /// </summary>
        /// <param name="path">Configuration file path.</param>
        void Save(string path);
    }
}