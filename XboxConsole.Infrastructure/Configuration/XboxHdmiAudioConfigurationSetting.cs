//------------------------------------------------------------------------------
// <copyright file="XboxHdmiAudioConfigurationSetting.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Configuration
{
    using System;

    /// <summary>
    /// An explicit override of the XboxConfigurationSetting class meant to represent the use of the string
    /// of the HdmiAudio configuration setting (see xbconfig command line utility).
    /// </summary>
    internal class XboxHdmiAudioConfigurationSetting : XboxConfigurationSetting<HdmiAudioOutput>
    {
        /// <summary>
        /// Initializes a new instance of the XboxHdmiAudioConfigurationSetting class.
        /// </summary>
        /// <param name="key">The setting key.</param>
        internal XboxHdmiAudioConfigurationSetting(string key)
            : base(key)
        {
        }

        /// <summary>
        /// Converts a strongly-typed value into a string value.
        /// </summary>
        /// <param name="hdmiAudioSetting">The value to be converted.</param>
        /// <returns>The string value that corresponds to the specified value.</returns>
        protected override string GetStringValueFromValue(HdmiAudioOutput hdmiAudioSetting)
        {
            // Enforce allowed values.
            switch (hdmiAudioSetting)
            {
                default:
                case HdmiAudioOutput.Off:
                    return "off";
                case HdmiAudioOutput.Stereo:
                    return "stereo";
                case HdmiAudioOutput.FivePointOne:
                    return "5.1";
                case HdmiAudioOutput.SevenPointOne:
                    return "7.1";
                case HdmiAudioOutput.Bitstream:
                    return "bitstream";
            }
        }

        /// <summary>
        /// Converts a string value into a stringly-typed value.
        /// </summary>
        /// <param name="stringVal">The string value to be converted.</param>
        /// <returns>The strongly typed value corresponding to the specified string value.</returns>
        protected override HdmiAudioOutput GetValueFromStringValue(string stringVal)
        {
            if (string.IsNullOrEmpty(stringVal))
            {
                return HdmiAudioOutput.Stereo;
            }

            switch (stringVal.ToUpperInvariant())
            {
                default:
                case "OFF":
                    return HdmiAudioOutput.Off;
                case "STEREO":
                    return HdmiAudioOutput.Stereo;
                case "5.1":
                    return HdmiAudioOutput.FivePointOne;
                case "7.1":
                    return HdmiAudioOutput.SevenPointOne;
                case "BITSTREAM":
                    return HdmiAudioOutput.Bitstream;
            }
        }
    }
}
