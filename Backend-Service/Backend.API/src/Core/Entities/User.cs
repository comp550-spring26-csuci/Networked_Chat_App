// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ivana Bavin-Gomez-San Basilio
//  Date: March 6 2026
//  Description: Defines the User entity.
//               It implements IUser.
//               This will match the "Users" table
// --------------------------------------------

using System;
using System.Collections.Generic;
using Backend.API.src.Core.Interface;
using Backend.API.src.Core.Logging;

namespace Backend.API.src.Core.Entities
{
    /// <summary>
    /// This class implementes IUser
    /// </summary>
    public class User : IUser
    {
        //----------------------------------
        //-------  Private Fields ----------
        //----------------------------------

        //-----Main class attributes
        private Guid _id = Guid.NewGuid();
        private string _username = string.Empty;
        private string _email = string.Empty;
        private string _passwordHash = string.Empty;

        //-----Timestamp Related Attributes
        // Moment of creation
        private DateTime _createdAt = DateTime.UtcNow;
        private DateTime? _lastActive;

        //-----State of the User
        // Options will be "Online", "Offline", "DoNotDIsturbe"
        // Initialuser is defaulted to offline
        private string _presenceStatus = "Offline";
        private string? _customStatusText;

        //-----Connections
        // We just store the IDs or links to other classses
        // They are initialized as empy lists
        private ICollection<Guid> _joinedServerIds = new List<Guid>();
        private ICollection<Guid> _friendIds = new List<Guid>();


        //----------------------------------
        //------  Getters and Setters ------
        //----------------------------------


        //-----Main class attributes
        // UUID - Primary Key 
        // It will automatically generate a user id
        public Guid Id 
        {
            get { return _id; } 
            set { _id = value; }
        } 


        public string Username
        {
            get { return _username; }
            set
            {
                // Validation -> the username can not be empty
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _username = value;
                }
            }

        }

        public string Email
        {
            get { return _email; }
            set
            {
                // Validation -> the email can not be empty, basic check for @
                if (!string.IsNullOrWhiteSpace(value) && value.Contains("@"))
                {
                    _email = value;
                }
            }

        }


        public string PasswordHash
        {
            get { return _passwordHash; }
            set
            {
                // Validation -> the passwordhash can not be empty
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _passwordHash = value;
                }
            }

        }

        //-----Timestamp Related Attributes
        public DateTime CreatedAt 
        { 
            get { return _createdAt; } 
            set {  _createdAt = value; } 
        } 


        public DateTime? LastActive
        {
            get { return _lastActive; }
            set { _lastActive = value; }
        }



        ///-----State of the User
        public string PresenceStatus
        {
            get { return _presenceStatus; } 
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _presenceStatus = value; 
                }
            } 
        }


        public string? CustomStatusText { get; set; }



        //-----Connections
        public ICollection<Guid> JoinedServerIds 
        { 
            get { return _joinedServerIds; } 

            // '??' if someone tries to set this to null, using an empy list instead
            set { _joinedServerIds = value ?? new List<Guid>(); } 
        } 
        public ICollection<Guid> FriendIds 
        { 
            get { return _friendIds;  }

            set { _friendIds = value ?? new List<Guid>(); }

        }



        //----------------------------------
        //--------  Constructors -----------
        //----------------------------------

        /// <summary>
        /// Public constructor use to create a new user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <param name="passwordHash"></param>
        public User(string username, string email, string passwordHash) 
        {
            Username = username;
            Email = email;
            PasswordHash = passwordHash;

            // The rest of the fields are automatically initialized
        }

        /// <summary>
        /// Protected constructor to be used by the Entity Framework
        /// </summary>
        protected User() 
        { 
        }


        //----------------------------------
        //-----------  Methods -------------
        //----------------------------------


        /// <summary>
        /// Handles user satus
        /// </summary>
        /// <param name="newStatus"></param>
        /// <param name="customText"></param>
        public void UpdatePresence(string newStatus, string? customText = null)
        {
            string oldStatus = _presenceStatus;
            PresenceStatus = newStatus;
            CustomStatusText = customText;
            // Stamps moment
            LastActive = DateTime.UtcNow;

            // Logging the state change
            AppLogger.DebugState("UserEntity", $"Status changed for {Username}: {oldStatus} -> {newStatus}");
        
        }


        /// <summary>
        /// Adding  a friend, avoiding adding the same person twice
        /// </summary>
        /// <param name="newFriendId"></param>
        public void AddFriend(Guid newFriendId) 
        {
            if (!_friendIds.Contains(newFriendId))
            { 
                _friendIds.Add(newFriendId);
                AppLogger.UserAction(Id.ToString(), $"Added friend {newFriendId}");
            }
        }


        /// <summary>
        /// Removing a friend
        /// </summary>
        /// <param name="friendId"></param>
        public void RemoveFriend(Guid friendId)
        {
            if (_friendIds.Contains(friendId))
            {
                _friendIds.Remove(friendId);
                AppLogger.UserAction(Id.ToString(), $"Removed friend {friendId}");
            }
        }


        /// <summary>
        /// Adds a server ID
        /// </summary>
        /// <param name="serverId"></param>
        public void JoinServer(Guid serverId)
        { 
            if (!_joinedServerIds.Contains(serverId))
            {
                _joinedServerIds.Add(serverId);
                AppLogger.UserAction(Id.ToString(), $"Joined server {serverId}");
            }
        }


        /// <summary>
        /// Drops the Server ID from the list
        /// The user will be disconnected 
        /// </summary>
        /// <param name="serverId"></param>
        public void LeaveServer(Guid serverId)
        {
            if (_joinedServerIds.Contains(serverId))
            {
                _joinedServerIds.Remove(serverId);
                AppLogger.UserAction(Id.ToString(), $"Leaved server {serverId}");
            }
        }


        /// <summary>
        /// To update the user being active
        /// </summary>
        public void MarkAsActive()
        {
            LastActive = DateTime.UtcNow;
            AppLogger.UserAction(Id.ToString(), $"Last time user was active {LastActive}");

        }


    }
}
