//------------------------------------------------------------------------------
// <copyright file="OpticalAudioOutput.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Configuration
{
    /// <summary>
    /// Range of valid values for the OpticalAudio configuration setting.
    /// </summary>
    public enum OpticalAudioOutput
    {
        /// <summary>
        /// Optical audio output is off.
        /// </summary>
        Off,
        
        /// <summary>
        /// Optical audio output is stereo.
        /// </summary>
        Stereo,

        /// <summary>
        /// Optical audio output is bitstream.
        /// </summary>
        Bitstream
    }
}
