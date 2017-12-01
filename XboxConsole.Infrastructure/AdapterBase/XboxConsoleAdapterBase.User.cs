//------------------------------------------------------------------------------
// <copyright file="XboxConsoleAdapterBase.User.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox
{
    using System.Collections.Generic;

    /// <summary>
    /// The base class for all XboxConsole adapters.  This class provides a default implementation
    /// for all parts of the Xbox Console API, even if they are not supported by one particular
    /// version of the XDK (in which case an exception is thrown).  It is assumed that the adapter
    /// for each version of the XDK will override the pieces of functionality that are available or
    /// different in that particular build.
    /// </summary>
    internal abstract partial class XboxConsoleAdapterBase : DisposableObject
    {
        /// <summary>
        /// Connects to the console and retrieves the collection of users.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <returns>An enumeration of XboxUserDefinition instances.</returns>
        public IEnumerable<XboxUserDefinition> GetUsers(string systemIpAddress)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            return this.PerformXdkFunc(
                systemIpAddress,
                () => this.GetUsersImpl(systemIpAddress),
                "Failed to retrieve users.");
        }

        /// <summary>
        /// Adds a guest user.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <returns>The user id of the added guest user.</returns>
        public uint AddGuestUser(string systemIpAddress)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            return this.PerformXdkFunc(
                systemIpAddress,
                () => this.AddGuestUserImpl(systemIpAddress),
                "Failed to add guest user.");
        }

        /// <summary>
        /// Adds a user to the console.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="emailAddress">The email address of the user to be added.</param>
        /// <returns>An XboxUserDefinition of the added user.</returns>
        public XboxUserDefinition AddUser(string systemIpAddress, string emailAddress)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            return this.PerformXdkFunc(
                systemIpAddress,
                () => this.AddUserImpl(systemIpAddress, emailAddress),
                "Failed to add user.");
        }

        /// <summary>
        /// Removes all users from the console.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <remarks>Signed-in users are signed out before being removed from the console.</remarks>
        public void DeleteAllUsers(string systemIpAddress)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            this.PerformXdkAction(
                systemIpAddress,
                () => this.DeleteAllUsersImpl(systemIpAddress),
                "Failed to delete all users.");
        }

        /// <summary>
        /// Removes the specified user from the console. 
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="user">The user to be removed.</param>
        /// <remarks>A signed-in user is signed out before being removed from the console.</remarks>
        public void DeleteUser(string systemIpAddress, XboxUserDefinition user)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            this.PerformXdkAction(
                systemIpAddress,
                () => this.DeleteUserImpl(systemIpAddress, user),
                "Failed to delete user.");
        }

        /// <summary>
        /// Signs the given user into Xbox Live.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="user">The user to sign in.</param>
        /// <param name="password">The password of the for signing in. If a password has been stored on the console, <c>null</c> can be passed in.</param>
        /// <param name="storePassword">If <c>true</c>, saves the given password on the console for later use.</param>
        /// <returns>An XboxUserDefinition of the signed-in user.</returns>
        public XboxUserDefinition SignInUser(string systemIpAddress, XboxUserDefinition user, string password, bool storePassword)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            return this.PerformXdkFunc(
                systemIpAddress,
                () => this.SignInUserImpl(systemIpAddress, user, password, storePassword),
                "Failed to sign-in user.");
        }

        /// <summary>
        /// Signs out the given user.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="user">The user to sign-out.</param>
        /// <returns>An XboxUserDefinition of the signed-out user.</returns>
        public XboxUserDefinition SignOutUser(string systemIpAddress, XboxUserDefinition user)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            return this.PerformXdkFunc(
                systemIpAddress,
                () => this.SignOutUserImpl(systemIpAddress, user),
                "Failed to sign-out user.");
        }

        /// <summary>
        /// Pairs the XboxGamepad to the XboxUser.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="gamepadId">The Id of the XboxGamepad to set the state of.</param>
        /// <param name="userId">The Id of the XboxUser to pair to.</param>
        public void PairGamepadToUser(string systemIpAddress, ulong gamepadId, uint userId)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            this.PerformXdkAction(
                systemIpAddress,
                () => this.PairGamepadToUserImpl(systemIpAddress, gamepadId, userId),
                "Failed to pair a gamepad to a user.");
        }

        /// <summary>
        /// Creates a party for the given title ID (if one does not exist) and adds the given local users to it.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="titleId">Title ID of the title to manage a party for.</param>
        /// <param name="actingUserXuid">Acting user XUID on whose behalf to add other users to the party.</param>
        /// <param name="localUserXuidsToAdd">User XUIDs to add to the party.</param>
        public void AddLocalUsersToParty(string systemIpAddress, uint titleId, string actingUserXuid, string[] localUserXuidsToAdd)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            this.PerformXdkAction(
                systemIpAddress,
                () => this.AddLocalUsersToPartyImpl(systemIpAddress, titleId, actingUserXuid, localUserXuidsToAdd),
                "Failed to add local users to a party.");
        }

        /// <summary>
        /// Invites the given users on behalf of the acting user to the party associated with the given title ID.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="titleId">Title ID of the title to manage a party for.</param>
        /// <param name="actingUserXuid">Acting user XUID on whose behalf to invite other users to the party.</param>
        /// <param name="remoteUserXuidsToInvite">Remote user XUIDs to invite to the party.</param>
        public void InviteToParty(string systemIpAddress, uint titleId, string actingUserXuid, string[] remoteUserXuidsToInvite)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            this.PerformXdkAction(
                systemIpAddress,
                () => this.InviteToPartyImpl(systemIpAddress, titleId, actingUserXuid, remoteUserXuidsToInvite),
                "Failed to invite remote users to a party.");
        }

        /// <summary>
        /// Removes the given users from the party belonging to the given title ID.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="titleId">Title ID of the title to manage a party for.</param>
        /// <param name="localUserXuidsToRemove">Local user XUIDs to remove from the party.</param>
        public void RemoveLocalUsersFromParty(string systemIpAddress, uint titleId, string[] localUserXuidsToRemove)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            this.PerformXdkAction(
                systemIpAddress,
                () => this.RemoveLocalUsersFromPartyImpl(systemIpAddress, titleId, localUserXuidsToRemove),
                "Failed to remove local users from a party.");
        }

        /// <summary>
        /// Returns the party ID belonging to the given title ID.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="titleId">Title ID of the title to get the associated party ID for.</param>
        /// <returns>ID of existing party used to accept or decline an invitation to the party.</returns>
        public string GetPartyId(string systemIpAddress, uint titleId)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            return this.PerformXdkFunc(
                systemIpAddress,
                () => this.GetPartyIdImpl(systemIpAddress, titleId),
                "Failed to get party ID.");
        }

        /// <summary>
        /// Lists both the current members and the reserved members of the party belonging to given title ID.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="titleId">Title ID of the title to get party members for.</param>
        /// <returns>Party member user XUIDs, which may contain a mix of local and remote users.</returns>
        public string[] GetPartyMembers(string systemIpAddress, uint titleId)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            return this.PerformXdkFunc(
                systemIpAddress,
                () => this.GetPartyMembersImpl(systemIpAddress, titleId),
                "Failed to get party members.");
        }

        /// <summary>
        /// Accepts the party invitation on behalf of the given acting user to the party associated with the given title ID.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="actingUserXuid">XUID of acting user on whose behalf to accept the invitation.</param>
        /// <param name="partyId">Title ID of the party created by another user to accept the invitation to.</param>
        public void AcceptInviteToParty(string systemIpAddress, string actingUserXuid, string partyId)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            this.PerformXdkAction(
                systemIpAddress,
                () => this.AcceptInviteToPartyImpl(systemIpAddress, actingUserXuid, partyId),
                "Failed to accept an invitation to a party.");
        }

        /// <summary>
        /// Declines the party invitation on behalf of the given acting user to the party associated with the given title ID.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="actingUserXuid">XUID of acting user on whose behalf to decline the invitation.</param>
        /// <param name="partyId">Title ID of the party created by another user to accept the invitation to.</param>
        public void DeclineInviteToParty(string systemIpAddress, string actingUserXuid, string partyId)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            this.PerformXdkAction(
                systemIpAddress,
                () => this.DeclineInviteToPartyImpl(systemIpAddress, actingUserXuid, partyId),
                "Failed to decline an invitation to a party.");
        }

        /// <summary>
        /// Connects to the console and retrieves the collection of users.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <returns>An enumeration of XboxUserDefinition instances.</returns>
        protected virtual IEnumerable<XboxUserDefinition> GetUsersImpl(string systemIpAddress)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Adds a guest user.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <returns>The user id of the added guest user.</returns>
        protected virtual uint AddGuestUserImpl(string systemIpAddress)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Adds a user to the console.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="emailAddress">The email address of the user to be added.</param>
        /// <returns>An XboxUserDefinition of the added user.</returns>
        protected virtual XboxUserDefinition AddUserImpl(string systemIpAddress, string emailAddress)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Removes all users from the console.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <remarks>Signed-in users are signed out before being removed from the console.</remarks>
        protected virtual void DeleteAllUsersImpl(string systemIpAddress)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Removes the specified user from the console. 
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="user">The user to be removed.</param>
        /// <remarks>A signed-in user is signed out before being removed from the console.</remarks>
        protected virtual void DeleteUserImpl(string systemIpAddress, XboxUserDefinition user)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Signs the given user into Xbox Live.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="user">The user to sign in.</param>
        /// <param name="password">The password of the for signing in. If a password has been stored on the console, <c>null</c> can be passed in.</param>
        /// <param name="storePassword">If <c>true</c>, saves the given password on the console for later use.</param>
        /// <returns>An XboxUserDefinition of the signed-in user.</returns>
        protected virtual XboxUserDefinition SignInUserImpl(string systemIpAddress, XboxUserDefinition user, string password, bool storePassword)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Signs out the given user.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="user">The user to sign-out.</param>
        /// <returns>An XboxUserDefinition of the signed-out user.</returns>
        protected virtual XboxUserDefinition SignOutUserImpl(string systemIpAddress, XboxUserDefinition user)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Pairs the XboxGamepad to the XboxUser.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="gamepadId">The Id of the XboxGamepad to set the state of.</param>
        /// <param name="userId">The Id of the XboxUser to pair to.</param>
        protected virtual void PairGamepadToUserImpl(string systemIpAddress, ulong gamepadId, uint userId)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Creates a party for the given title ID (if one does not exist) and adds the given local users to it.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="titleId">Title ID of the title to manage a party for.</param>
        /// <param name="actingUserXuid">Acting user XUID on whose behalf to add other users to the party.</param>
        /// <param name="localUserXuidsToAdd">User XUIDs to add to the party.</param>
        protected virtual void AddLocalUsersToPartyImpl(string systemIpAddress, uint titleId, string actingUserXuid, string[] localUserXuidsToAdd)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Invites the given users on behalf of the acting user to the party associated with the given title ID.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="titleId">Title ID of the title to manage a party for.</param>
        /// <param name="actingUserXuid">Acting user XUID on whose behalf to invite other users to the party.</param>
        /// <param name="remoteUserXuidsToInvite">Remote user XUIDs to invite to the party.</param>
        protected virtual void InviteToPartyImpl(string systemIpAddress, uint titleId, string actingUserXuid, string[] remoteUserXuidsToInvite)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Removes the given users from the party belonging to the given title ID.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="titleId">Title ID of the title to manage a party for.</param>
        /// <param name="localUserXuidsToRemove">Local user XUIDs to remove from the party.</param>
        protected virtual void RemoveLocalUsersFromPartyImpl(string systemIpAddress, uint titleId, string[] localUserXuidsToRemove)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Returns the party ID belonging to the given title ID.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="titleId">Title ID of the title to get the associated party ID for.</param>
        /// <returns>ID of existing party used to accept or decline an invitation to the party.</returns>
        protected virtual string GetPartyIdImpl(string systemIpAddress, uint titleId)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Lists both the current members and the reserved members of the party belonging to given title ID.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="titleId">Title ID of the title to get party members for.</param>
        /// <returns>Party member user XUIDs, which may contain a mix of local and remote users.</returns>
        protected virtual string[] GetPartyMembersImpl(string systemIpAddress, uint titleId)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Accepts the party invitation on behalf of the given acting user to the party associated with the given title ID.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="actingUserXuid">XUID of acting user on whose behalf to accept the invitation.</param>
        /// <param name="partyId">Title ID of the party created by another user to accept the invitation to.</param>
        protected virtual void AcceptInviteToPartyImpl(string systemIpAddress, string actingUserXuid, string partyId)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Declines the party invitation on behalf of the given acting user to the party associated with the given title ID.
        /// </summary>
        /// <param name="systemIpAddress">The system IP address of the console.</param>
        /// <param name="actingUserXuid">XUID of acting user on whose behalf to decline the invitation.</param>
        /// <param name="partyId">Title ID of the party created by another user to accept the invitation to.</param>
        protected virtual void DeclineInviteToPartyImpl(string systemIpAddress, string actingUserXuid, string partyId)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }
    }
}
