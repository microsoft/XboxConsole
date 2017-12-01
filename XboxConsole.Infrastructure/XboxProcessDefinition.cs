//------------------------------------------------------------------------------
// <copyright file="XboxProcessDefinition.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox
{
    using System;

    /// <summary>
    /// Represents the definition of a process on an Xbox console.
    /// </summary>
    public class XboxProcessDefinition
    {
        /// <summary>
        /// Initializes a new instance of the XboxProcessDefinition class.
        /// </summary>
        /// <param name="operatingSystem">The Xbox operating system.</param>
        /// <param name="processId">The Id (PID) of the Xbox process.</param>
        /// <param name="imageFileName">The file name of the Xbox process.</param>
        internal XboxProcessDefinition(XboxOperatingSystem operatingSystem, uint processId, string imageFileName)
        {
            // When XTF returns a null imageFileName, we change it to an empty string to avoid breaking our existing code
            // It is not exactly clear why XTF returns a null imageFileName, but it does in some cases. 
            if (imageFileName == null)
            {
                imageFileName = string.Empty;
            }

            this.OperatingSystem = operatingSystem;
            this.ProcessId = processId;
            this.ImageFileName = imageFileName;
        }

        /// <summary>
        /// Gets the Xbox operating system the process is running in.
        /// </summary>
        public XboxOperatingSystem OperatingSystem { get; private set; }

        /// <summary>
        /// Gets the Id (PID) of the Xbox process.
        /// </summary>
        public uint ProcessId { get; private set; }

        /// <summary>
        /// Gets the file name of the Xbox process.
        /// </summary>
        public string ImageFileName { get; private set; }
    }
}
