//------------------------------------------------------------------------------
// <copyright file="XboxConsoleInfo.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents information about a Xbox console.
    /// </summary>
    internal class XboxConsoleInfo
    {
        /// <summary>
        /// Initializes a new instance of the XboxConsoleInfo class.
        /// </summary>
        /// <param name="toolsIpAddress">The tools ip address of the console.</param>
        /// <param name="consoleIPAddress">The system ip address of the console.</param>
        /// <param name="accessKey">The access key of the console.</param>
        /// <param name="consoleId">The console id.</param>
        /// <param name="certType">The cert type of the console.</param>
        /// <param name="hostName">The host name of the console.</param>
        /// <param name="deviceId">The device id of the console.</param>
        public XboxConsoleInfo(string toolsIpAddress, string consoleIPAddress, string accessKey, string consoleId, XboxCertTypes certType, string hostName, string deviceId)
        {
            this.ToolsIpAddress = toolsIpAddress;
            this.ConsoleIPAddress = consoleIPAddress;
            this.AccessKey = accessKey;
            this.ConsoleId = consoleId;
            this.CertType = certType;
            this.HostName = hostName;
            this.DeviceId = deviceId;
        }

        /// <summary>
        /// Gets the tools ip address.
        /// </summary>
        public string ToolsIpAddress { get; private set; }

        /// <summary>
        /// Gets the system ip address.
        /// </summary>
        public string ConsoleIPAddress { get; private set; }

        /// <summary>
        /// Gets the access key.
        /// </summary>
        public string AccessKey { get; private set; }
        
        /// <summary>
        /// Gets the console id.
        /// </summary>
        public string ConsoleId { get; private set; }

        /// <summary>
        /// Gets the cert type.
        /// </summary>
        public XboxCertTypes CertType { get; private set; }

        /// <summary>
        /// Gets the host name.
        /// </summary>
        public string HostName { get; private set; }

        /// <summary>
        /// Gets the device id.
        /// </summary>
        public string DeviceId { get; private set; }
    }
}
