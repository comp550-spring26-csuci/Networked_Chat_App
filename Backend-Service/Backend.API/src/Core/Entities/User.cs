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
    /// This class implements IUser
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
        /// To update the user being active
        /// </summary>
        public void MarkAsActive()
        {
            LastActive = DateTime.UtcNow;
            AppLogger.UserAction(Id.ToString(), $"Last time user was active {LastActive}");

        }


    }
}
