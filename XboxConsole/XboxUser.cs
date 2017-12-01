//------------------------------------------------------------------------------
// <copyright file="XboxUser.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox
{
    using System;
    using Microsoft.Internal.GamesTest.Xbox.Input;
    using Microsoft.Internal.GamesTest.Xbox.Telemetry;

    /// <summary>
    /// A virtual user.
    /// </summary>
    public class XboxUser : XboxItem
    {
        /// <summary>
        /// Initializes a new instance of the XboxUser class.
        /// </summary>
        /// <param name="console">The XboxConsole on which the virtual user was created.</param>
        /// <param name="userId">A unique identifier for the user.</param>
        /// <param name="emailAddress">The email address used to sign the user in.</param>
        /// <param name="gamertag">The user's GamerTag.</param>
        /// <param name="signedin">If true, the user is signed in on the console.</param>
        internal XboxUser(XboxConsole console, uint userId, string emailAddress, string gamertag, bool signedin)
            : base(console)
        {
            this.Definition = new XboxUserDefinition(userId, emailAddress, gamertag, signedin);
        }

        /// <summary>
        /// Initializes a new instance of the XboxUser class.
        /// </summary>
        /// <param name="console">The XboxConsole on which the virtual user was created.</param>
        /// <param name="userId">A unique identifier for the user.</param>
        /// <param name="emailAddress">The email address used to sign the user in.</param>
        /// <param name="gamertag">The user's GamerTag.</param>
        internal XboxUser(XboxConsole console, uint userId, string emailAddress, string gamertag)
            : this(console, userId, emailAddress, gamertag, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the XboxUser class.
        /// </summary>
        /// <param name="console">The XboxConsole on which the virtual user was created.</param>
        /// <param name="userData">The user data of the user.</param>
        internal XboxUser(XboxConsole console, XboxUserDefinition userData)
            : base(console)
        {
            if (userData == null)
            {
                throw new ArgumentNullException("userData", "userData can't be null");
            }

            this.Definition = userData;
        }

        /// <summary>
        /// Gets a unique identifier for the user.
        /// </summary>
        public uint UserId
        {
            get { return this.Definition.UserId; }
        }

        /// <summary>
        /// Gets the email address used to sign the user in.
        /// </summary>
        public string EmailAddress
        {
            get { return this.Definition.EmailAddress; }
        }

        /// <summary>
        /// Gets the user's GamerTag.
        /// </summary>
        public string GamerTag
        {
            get { return this.Definition.Gamertag; }
        }

        /// <summary>
        /// Gets a value indicating whether the user is signed in on the console.
        /// </summary>
        public bool IsSignedIn
        {
            get { return this.Definition.IsSignedIn; }
        }

        /// <summary>
        /// Gets the definition object backing this package object.
        /// This is a property and not a field so that it can be shimmed in the Unit Tests.
        /// </summary>
        internal XboxUserDefinition Definition { get; private set; }

        /// <summary>
        /// Pairs a user with a virtual controller.
        /// </summary>
        /// <param name="gamepad">The virtual gamepad with which the user will be paired.</param>
        public void PairWithVirtualController(XboxGamepad gamepad)
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            if (gamepad == null)
            {
                throw new ArgumentNullException("gamepad", "gamepad cannot be null.");
            }

            if (!gamepad.Id.HasValue)
            {
                throw new ArgumentException("gamepad must have a non-null id value.", "gamepad");
            }

            this.Console.Adapter.PairGamepadToUser(this.Console.SystemIpAddressAndSessionKeyCombined, gamepad.Id.Value, this.UserId);
        }

        /// <summary>
        /// Pairs a user with a physical controller.
        /// </summary>
        /// <param name="controllerId">The physical controller with which the user will be paired.</param>
        public void PairWithPhysicalController(ulong controllerId)
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            this.Console.Adapter.PairGamepadToUser(this.Console.SystemIpAddressAndSessionKeyCombined, controllerId, this.UserId);
        }

        /// <summary>
        /// Signs in the user.
        /// </summary>
        /// <param name="password">The password of the for signing in. If a password has been stored on the console, <c>null</c> can be passed in.</param>
        /// <param name="storePassword">If <c>true</c>, saves the given password on the console for later use.</param>
        /// <remarks>
        /// In this release, the password text is not encrypted when it is passed over the network to the console by SignIn.
        /// If this is a concern in your network environment, then use the Sign-In app on the console to sign in the user and
        /// store the user password. (The Sign-In app sends the password over the network encrypted.) After this, you can
        /// subsequently use Signin to sign in without passing a password.
        /// </remarks>
        public void SignIn(string password, bool storePassword)
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            var newDefinition = this.Console.Adapter.SignInUser(this.Console.SystemIpAddressAndSessionKeyCombined, this.Definition, password, storePassword);

            if (newDefinition == null || newDefinition.EmailAddress != this.EmailAddress)
            {
                throw new XboxSignInException("Unable to verify that the user has been signed in.");
            }

            this.Definition = newDefinition;
        }

        /// <summary>
        /// Signs out the user.
        /// </summary>
        public void SignOut()
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            var newDefinition = this.Console.Adapter.SignOutUser(this.Console.SystemIpAddressAndSessionKeyCombined, this.Definition);

            if (newDefinition == null || newDefinition.EmailAddress != this.EmailAddress)
            {
                throw new XboxSignInException("Unable to verify that the user has been signed out.");
            }

            this.Definition = newDefinition;
        }
    }
}
