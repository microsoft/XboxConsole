//------------------------------------------------------------------------------
// <copyright file="HdmiAudioOutput.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Configuration
{
    /// <summary>
    /// Range of valid values for the HDMIAudio configuration setting.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Hdmi", Justification = "HDMI is spelled correctly.")]
    public enum HdmiAudioOutput
    {
        /// <summary>
        /// HDMI audio output is off.
        /// </summary>
        Off,
        
        /// <summary>
        /// HDMI audio output is stereo.
        /// </summary>
        Stereo,

        /// <summary>
        /// HDMI audio output is 5.1 surround sound.
        /// </summary>
        FivePointOne,

        /// <summary>
        /// HDMI audio output is 7.1 surround sound.
        /// </summary>
        SevenPointOne,

        /// <summary>
        /// HDMI audio output is bit stream.
        /// </summary>
        Bitstream
    }
}
