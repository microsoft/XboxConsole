//------------------------------------------------------------------------------
// <copyright file="XtfDeployCallbackActions.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Deployment
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// XtfDeployCallbackActions encapsulates the values IXtfDeployCallback_Action enumeration.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class XtfDeployCallbackActions
    {
        // Interpretation of the following values comes from the XDK team's sources.
        // [helpstring("IXtfDeployCallback offers callback information on the status of deployment, including management of files and transfer progress. The callback purpose is communicated with the iCallbackPurpose parameter of the callback, using these enum values.")]
        // typedef enum
        // {
        //     [helpstring("Deployment has started to determine initial information for deploy.")]
        //     IXtfDeployInitializing              = 10,
        // 
        //     [helpstring("Deployment has determined initial information for deploy; total files, total bytes to transfer.")]
        //     IXtfDeployMetrics                   = 20,
        // 
        //     [helpstring("File transfer progress callback; bytes transferred for file, file size.")]
        //     IXtfDeployUpdate                    = 100,
        // 
        //     [helpstring("Extra file detected remotely and deleted from the remote device.")]
        //     IXtfDeployNotifyExtraFileRemoved    = 1000,
        // 
        //     [helpstring("Extra file detected on the remove device.")]
        //     IXtfDeployNotifyExtraFileDetected   = 1010,
        // 
        //     [helpstring("Error encountered on deploy; error number")]
        //     IXtfDeployNotifyError               = 10000,
        // } IXtfDeployCallback_Action;

        /// <summary>
        /// Deployment has started to determine initial information for deploy.
        /// </summary>
        public const int IXtfDeployInitializing = 10;

        /// <summary>
        /// Deployment has determined initial information for deploy; total files, total bytes to transfer.
        /// </summary>
        public const int IXtfDeployMetrics = 20;

        /// <summary>
        /// File transfer progress callback; bytes transferred for file, file size.
        /// </summary>
        public const int IXtfDeployUpdate = 100;

        /// <summary>
        /// Extra file detected remotely and deleted from the remote device.
        /// </summary>
        public const int IXtfDeployNotifyExtraFileRemoved = 1000;

        /// <summary>
        /// Extra file detected on the remove device.
        /// </summary>
        public const int IXtfDeployNotifyExtraFileDetected = 1010;

        /// <summary>
        /// Error encountered on deploy; error number.
        /// </summary>
        public const int IXtfDeployNotifyError = 10000;
    }
}
