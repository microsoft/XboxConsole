//------------------------------------------------------------------------------
// <copyright file="AssemblyInfoCore.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Reflection;

[assembly: AssemblyCompany("Microsoft Corporation")]
[assembly: AssemblyCopyright("© Microsoft Corporation. All rights reserved.")]
[assembly: AssemblyProduct("Xbox Console")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

// Version information for this assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      0 for local builds, an encoded date for team builds. (Year Mod 5)(2 digit Month)(2 digit Day)
//      0 for local builds, a per Major.Minor.Date revision for team builds.
[assembly: AssemblyVersion("2.0.0.0")]
[assembly: AssemblyFileVersion("2.0.0.0")]