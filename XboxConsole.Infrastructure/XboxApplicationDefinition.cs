//------------------------------------------------------------------------------
// <copyright file="XboxApplicationDefinition.cs" company="Microsoft">
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
    /// Represents an application that resides in a package.
    /// </summary>
    internal class XboxApplicationDefinition
    {
        /// <summary>
        /// Initializes a new instance of the XboxApplicationDefinition class.
        /// </summary>
        /// <param name="aumid">The Application User Model Id for this application. This is equal to "PackageFamilyName!ApplicationId".</param>
        public XboxApplicationDefinition(string aumid)
        {
            if (string.IsNullOrWhiteSpace(aumid))
            {
                throw new ArgumentNullException("aumid");
            }

            this.Aumid = aumid;

            // Since the August 2013 XDK IXtfApplicationClient.GetInstalled() doesn't return ApplicationId and PackageFamilyName info anymore.
            // We can parse aumid to try to get these pieces of information.

            string[] appParts = aumid.Split(new[] { '!' }, StringSplitOptions.RemoveEmptyEntries);
            if (appParts.Length == 2)
            {
                this.PackageFamilyName = appParts[0];
                this.ApplicationId = appParts[1];
            }
            else
            {
                throw new ArgumentException("The aumid parameter is not in the <PackageFamilyName>!<ApplicationId> format and cannot be parsed.");
            }
        }

        /// <summary>
        /// Gets the Application Id for this application.  The Application Id is the name of the executable for this application
        /// without the ".exe" extension.
        /// </summary>
        public string ApplicationId { get; private set; }

        /// <summary>
        /// Gets the Application User Model Id for this application.  This is equal to "PackageFamilyName!ApplicationId".
        /// </summary>
        public string Aumid { get; private set; }

        /// <summary>
        /// Gets the Package Family Name for this package.  See the XboxPackageDefinition constructor comment for a description of this field.
        /// </summary>
        public string PackageFamilyName { get; private set; }
    }
}