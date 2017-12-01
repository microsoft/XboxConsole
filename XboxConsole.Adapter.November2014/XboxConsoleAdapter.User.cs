//------------------------------------------------------------------------------
// <copyright file="XboxConsoleAdapter.User.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Adapter.November2014
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using Microsoft.Internal.GamesTest.Xbox;

    /// <summary>
    /// This class represents an implemenation of the XboxConsoleAdapter
    /// that is supported by the November 2014 version of the XDK.
    /// </summary>
    internal partial class XboxConsoleAdapter : XboxConsoleAdapterBase
    {
        /// <summary>
        /// Common message for NoLocalSignedInUsersInPartyException.
        /// </summary>
        private const string NoPartyMembersMessage = "There are no local signed in users in the party.";

        /// <summary>
        /// Common message for InvalidPartyInviteException.
        /// </summary>
        private const string InvalidPartyOperationMessage = "Invalid party invite operation or party ID";

        /// <summary>
        /// Common message for InvalidXuidException.
        /// </summary>
        private const string InvalidXuid = "Invalid XUID passed for one of the parameters, or one of the XUIDs cannot be used for the requested operation.";

        private const int UnknownUserAuthenticationError = unchecked((int)0x80A20301);
        private const int NonExistingControllerIdError = unchecked((int)0x80A20203);

        /// <summary>
        /// Connects to the console and retrieves the collection of users.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <returns>An enumeration of XboxUserDefinition instances.</returns>
        protected override IEnumerable<XboxUserDefinition> GetUsersImpl(string systemIpAddress)
        {
            return this.XboxXdk.GetUsers(systemIpAddress);
        }

        /// <summary>
        /// Adds a guest user.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <returns>The user id of the added guest user.</returns>
        protected override uint AddGuestUserImpl(string systemIpAddress)
        {
            return this.XboxXdk.AddGuestUser(systemIpAddress);
        }

        /// <summary>
        /// Adds a user to the console.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="emailAddress">The email address of the user to be added.</param>
        /// <returns>An XboxUserDefinition of the added user.</returns>
        protected override XboxUserDefinition AddUserImpl(string systemIpAddress, string emailAddress)
        {
            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                throw new ArgumentException("emailAddress cannot be null or whitespace.", "emailAddress");
            }

            var user = this.XboxXdk.AddUser(systemIpAddress, emailAddress);

            if (user == null)
            {
                throw new XboxConsoleException("An error occured while adding a user.", systemIpAddress);
            }

            return user;
        }

        /// <summary>
        /// Removes all users from the console.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <remarks>Signed-in users are signed out before being removed from the console.</remarks>
        protected override void DeleteAllUsersImpl(string systemIpAddress)
        {
            this.XboxXdk.DeleteAllUsers(systemIpAddress);
        }

        /// <summary>
        /// Removes the specified user from the console. 
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="user">The user to be removed.</param>
        /// <remarks>A signed-in user is signed out before being removed from the console.</remarks>
        protected override void DeleteUserImpl(string systemIpAddress, XboxUserDefinition user)
        {
            this.XboxXdk.DeleteUser(systemIpAddress, user);
        }

        /// <summary>
        /// Signs the given user into Xbox Live.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="user">The user to sign in.</param>
        /// <param name="password">The password of the user for signing in. If a password has been stored on the console, <c>null</c> can be passed in.</param>
        /// <param name="storePassword">If <c>true</c>, saves the given password on the console for later use.</param>
        /// <returns>An XboxUserDefinition of the signed-in user.</returns>
        protected override XboxUserDefinition SignInUserImpl(string systemIpAddress, XboxUserDefinition user, string password, bool storePassword)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user", "user cannot be null");
            }

            try
            {
                return this.XboxXdk.SignInUser(systemIpAddress, user, password, storePassword);
            }
            catch (COMException ex)
            {
                switch (unchecked((uint)ex.ErrorCode))
                {
                    case 0x80048823:
                        throw new XboxSignInException(string.Format(CultureInfo.InvariantCulture, "The given user's email address {0} is not valid.", user.EmailAddress), ex, systemIpAddress);
                    case 0x8004882E:
                        throw new XboxSignInException("The password for the given user is not stored on the console. You must supply a password as part of the sign in operation.", ex, systemIpAddress);
                    case 0x80048821:
                        throw new XboxSignInException("The password for the given user is invalid.", ex, systemIpAddress);
                    case 0x8015DC16:
                        throw new XboxSignInException("The given user is signed in on another console.", ex, systemIpAddress);
                    default:
                        throw;
                }
            }
        }

        /// <summary>
        /// Signs out the given user.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="user">The user to sign-out.</param>
        /// <returns>An XboxUserDefinition of the signed-out user.</returns>
        protected override XboxUserDefinition SignOutUserImpl(string systemIpAddress, XboxUserDefinition user)
        {
            return this.XboxXdk.SignOutUser(systemIpAddress, user);
        }

        /// <summary>
        /// Pairs the XboxGamepad to the XboxUser.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="gamepadId">The Id of the XboxGamepad to set the state of.</param>
        /// <param name="userId">The Id of the XboxUser to pair to.</param>
        protected override void PairGamepadToUserImpl(string systemIpAddress, ulong gamepadId, uint userId)
        {
            try
            {
                this.XboxXdk.PairControllerToUser(systemIpAddress, userId, gamepadId);
            }
            catch (COMException e)
            {
                if (e.HResult == UnknownUserAuthenticationError)
                {
                    throw new XboxException("An invalid or signed-out user attempted to pair with a gamepad.", e);
                }
                else if (e.HResult == NonExistingControllerIdError)
                {
                    throw new XboxException("A user attempted to pair with an invalid or nonexistent gamepad.", e);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Pairs the XboxGamepad to the XboxUser.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="gamepadId">The Id of the XboxGamepad to set the state of.</param>
        /// <param name="userId">The Id of the XboxUser to pair to.</param>
        protected override void PairGamepadToUserExclusiveImpl(string systemIpAddress, ulong gamepadId, uint userId)
        {
            try
            {
                this.XboxXdk.PairControllerToUserExclusive(systemIpAddress, userId, gamepadId);
            }
            catch (COMException e)
            {
                if (e.HResult == UnknownUserAuthenticationError)
                {
                    throw new XboxException("An invalid or signed-out user attempted to pair with a gamepad.", e);
                }
                else if (e.HResult == NonExistingControllerIdError)
                {
                    throw new XboxException("A user attempted to pair with an invalid or nonexistent gamepad.", e);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Creates a party for the given title ID (if one does not exist) and adds the given local users to it.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="titleId">Title ID of the title to manage a party for.</param>
        /// <param name="actingUserXuid">Acting user XUID on whose behalf to add other users to the party.</param>
        /// <param name="localUserXuidsToAdd">User XUIDs to add to the party.</param>
        protected override void AddLocalUsersToPartyImpl(string systemIpAddress, uint titleId, string actingUserXuid, string[] localUserXuidsToAdd)
        {
            try
            {
                this.XboxXdk.AddLocalUsersToParty(systemIpAddress, titleId, actingUserXuid, localUserXuidsToAdd);
            }
            catch (COMException e)
            {
                if (e.HResult == unchecked((int)0x80004003))
                {
                    throw new InvalidXuidException(InvalidXuid, e, systemIpAddress);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Invites the given users on behalf of the acting user to the party associated with the given title ID.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="titleId">Title ID of the title to manage a party for.</param>
        /// <param name="actingUserXuid">Acting user XUID on whose behalf to invite other users to the party.</param>
        /// <param name="remoteUserXuidsToInvite">Remote user XUIDs to invite to the party.</param>
        protected override void InviteToPartyImpl(string systemIpAddress, uint titleId, string actingUserXuid, string[] remoteUserXuidsToInvite)
        {
            try
            {
                this.XboxXdk.InviteToParty(systemIpAddress, titleId, actingUserXuid, remoteUserXuidsToInvite);
            }
            catch (COMException e)
            {
                if (e.HResult == unchecked((int)0x80004003))
                {
                    throw new InvalidXuidException(InvalidXuid, e, systemIpAddress);
                }
                else if (e.HResult == unchecked((int)0x87CC0007))
                {
                    throw new NoLocalSignedInUsersInPartyException(NoPartyMembersMessage, e, systemIpAddress);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Removes the given users from the party belonging to the given title ID.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="titleId">Title ID of the title to manage a party for.</param>
        /// <param name="localUserXuidsToRemove">Local user XUIDs to remove from the party.</param>
        protected override void RemoveLocalUsersFromPartyImpl(string systemIpAddress, uint titleId, string[] localUserXuidsToRemove)
        {
            try
            {
                this.XboxXdk.RemoveLocalUsersFromParty(systemIpAddress, titleId, localUserXuidsToRemove);
            }
            catch (COMException e)
            {
                if (e.HResult == unchecked((int)0x87CC0007))
                {
                    throw new NoLocalSignedInUsersInPartyException(NoPartyMembersMessage, e, systemIpAddress);
                }
                else if (e.HResult == unchecked((int)0x80004003))
                {
                    throw new InvalidXuidException(InvalidXuid, e, systemIpAddress);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Returns the party ID belonging to the given title ID.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="titleId">Title ID of the title to get the associated party ID for.</param>
        /// <returns>ID of existing party used to accept or decline an invitation to the party.</returns>
        protected override string GetPartyIdImpl(string systemIpAddress, uint titleId)
        {
            try
            {
                return this.XboxXdk.GetPartyId(systemIpAddress, titleId);
            }
            catch (COMException e)
            {
                if (e.HResult == unchecked((int)0x87CC0007))
                {
                    throw new NoLocalSignedInUsersInPartyException(NoPartyMembersMessage, e, systemIpAddress);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Lists both the current members and the reserved members of the party belonging to given title ID.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="titleId">Title ID of the title to get party members for.</param>
        /// <returns>Party member user XUIDs, which may contain a mix of local and remote users.</returns>
        protected override string[] GetPartyMembersImpl(string systemIpAddress, uint titleId)
        {
            try
            {
                return this.XboxXdk.GetPartyMembers(systemIpAddress, titleId);
            }
            catch (COMException e)
            {
                if (e.HResult == unchecked((int)0x87CC0007))
                {
                    throw new NoLocalSignedInUsersInPartyException(NoPartyMembersMessage, e, systemIpAddress);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Accepts the party invitation on behalf of the given acting user to the party associated with the given title ID.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="actingUserXuid">XUID of acting user on whose behalf to accept the invitation.</param>
        /// <param name="partyId">Title ID of the party created by another user to accept the invitation to.</param>
        protected override void AcceptInviteToPartyImpl(string systemIpAddress, string actingUserXuid, string partyId)
        {
            try
            {
                this.XboxXdk.AcceptInviteToParty(systemIpAddress, actingUserXuid, partyId);
            }
            catch (COMException e)
            {
                if (e.HResult == unchecked((int)0x8019019c))
                {
                    throw new InvalidPartyInviteException(InvalidPartyOperationMessage, e, systemIpAddress);
                }
                else if (e.HResult == unchecked((int)0x80004003))
                {
                    throw new InvalidXuidException(InvalidXuid, e, systemIpAddress);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Declines the party invitation on behalf of the given acting user to the party associated with the given title ID.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="actingUserXuid">XUID of acting user on whose behalf to decline the invitation.</param>
        /// <param name="partyId">Title ID of the party created by another user to accept the invitation to.</param>
        protected override void DeclineInviteToPartyImpl(string systemIpAddress, string actingUserXuid, string partyId)
        {
            try
            {
                this.XboxXdk.DeclineInviteToParty(systemIpAddress, actingUserXuid, partyId);
            }
            catch (COMException e)
            {
                if (e.HResult == unchecked((int)0x8019019c))
                {
                    throw new InvalidPartyInviteException(InvalidPartyOperationMessage, e, systemIpAddress);
                }
                else if (e.HResult == unchecked((int)0x80004003))
                {
                    throw new InvalidXuidException(InvalidXuid, e, systemIpAddress);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
