//------------------------------------------------------------------------------
// <copyright file="XboxPackageDefinition.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a package that may or may not reside on an Xbox.
    /// </summary>
    internal class XboxPackageDefinition
    {
        /// <summary>
        /// Initializes a new instance of the XboxPackageDefinition class.
        /// Here is a link to a great video that explains the difference between Package Full Name and Package Family Name.
        /// http://www.youtube.com/watch?v=8cvSNT9ho58
        /// There are 3 pieces of information that identify a package.
        /// 1.  Package Full Name.  This is a string consisting of different pieces of information concatenated together with underscores.
        ///     PackageName_VersionNumber_ProcessorArchitecture_ResourceId_PublisherId
        ///     Consider this example:
        ///     NuiView.ERA_1.0.0.0_neutral__8wekyb3d8bbwe
        ///     Here we have:
        ///     PackageName = NuiView.ERA
        ///     Version = 1.0.0.0
        ///     ProcessorArchitecture = neutral
        ///     ResourceId = {blank}.  Note that this element is optional and is blank in this example.
        ///     PublisherId = 8wekyb3d8bbwe
        /// 2.  Package Family Name.  This is a string consisting of two different pieces of information concatenated together with underscores.
        ///     PackageName_PublisherId
        ///     Consider this example:
        ///     NuiView.ERA_8wekyb3d8bbwe
        ///     Here we have:
        ///     PackageName = NuiView.ERA
        ///     PublisherId = 8wekyb3d8bbwe
        /// 3.  The applications.
        /// </summary>
        /// <param name="packageFullName">The Package Full Name for this package.  See constructor comment for a thorough description of this parameter.</param>
        /// <param name="packageFamilyName">The Package Family Name for this package.  See constructor comment for a thorough description of this parameter.</param>
        /// <param name="aumids">The Application User Model Ids of the applications for this package.</param>
        public XboxPackageDefinition(string packageFullName, string packageFamilyName, IEnumerable<string> aumids)
        {
            if (string.IsNullOrWhiteSpace(packageFullName))
            {
                throw new ArgumentNullException("packageFullName");
            }
            
            if (string.IsNullOrWhiteSpace(packageFamilyName))
            {
                throw new ArgumentNullException("packageFamilyName");
            }

            string[] nameParts = packageFullName.Split(new[] { '_' });
            if (nameParts.Length != 5)
            {
                throw new ArgumentException("The packageFullName parameter is not in the <PackageName>_<VersionNumber>_<ProcessorArchitecture>_<ResourceId>_<PublisherId> format and cannot be parsed.");
            }

            this.FamilyName = packageFamilyName;
            this.FullName = packageFullName;

            this.ApplicationDefinitions = aumids.Select(aumid => new XboxApplicationDefinition(aumid));
        }

        /// <summary>
        /// Initializes a new instance of the XboxPackageDefinition class.
        /// </summary>
        /// <param name="packageFullName">The Package Full Name for this package.</param>
        /// <param name="aumids">The Application User Model Ids of the applications for this package.</param>
        public XboxPackageDefinition(string packageFullName, IEnumerable<string> aumids)
        {
            if (string.IsNullOrWhiteSpace(packageFullName))
            {
                throw new ArgumentNullException("packageFullName");
            }

            this.FullName = packageFullName;
            string[] nameParts = packageFullName.Split(new[] { '_' });
            if (nameParts.Length != 5)
            {
                throw new ArgumentException("The packageFullName parameter is not in the <PackageName>_<VersionNumber>_<ProcessorArchitecture>_<ResourceId>_<PublisherId> format and cannot be parsed.");
            }

            this.FamilyName = nameParts.First() + "_" + nameParts.Last();

            this.ApplicationDefinitions = aumids.Select(aumid => new XboxApplicationDefinition(aumid));
        }

        /// <summary>
        /// Gets the application definitions for this package.
        /// </summary>
        public IEnumerable<XboxApplicationDefinition> ApplicationDefinitions { get; private set; }

        /// <summary>
        /// Gets the Package Family Name for this package.  See the constructor comment for a description of this field.
        /// </summary>
        public string FamilyName { get; private set; }

        /// <summary>
        /// Gets the Package Full Name for this package.  See the constructor comment for a thorough description of this field.
        /// </summary>
        public string FullName { get; private set; }
    }
}
