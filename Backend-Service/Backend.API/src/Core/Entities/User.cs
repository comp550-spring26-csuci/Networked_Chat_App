// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ivana Bavin-Gomez-San Basilio
//  Date: March 6 2026
//  Description: Defines the User entity.
//               It implements IUser.
//               This will match the "Users" table
// --------------------------------------------

using Backend.API.src.Core.Interface;
using Backend.API.src.Core.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Backend.API.src.Core.Enums;

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
        // Options will be "Online", "Offline", "DoNotDisturbe"
        // Initial user is defaulted to offline
        private UserStateType _presenceStatus = UserStateType.Inactive;
        private string? _customStatusText =  string.Empty;


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
                // Validation -> the passwordHash can not be empty
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
        public UserStateType PresenceStatus
        {
            get { return _presenceStatus; } 
            set { _presenceStatus = value; }
            
        }

        // Added Data Annotation for security limits (Max 100 chars)
        [MaxLength(100, ErrorMessage = "Custom status cannot exceed 100 characters.")]
        public string? CustomStatusText 
        {
            get { return _customStatusText; }
            set { _customStatusText = value; } 
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
        /// To update the user being active
        /// </summary>
        public void MarkAsActive()
        {
            LastActive = DateTime.UtcNow;
            AppLogger.UserAction(Id.ToString(), $"Last time user was active {LastActive}");

        }

        /// <summary>
        /// Handles user  presence status with specific business rules that show status
        /// </summary>
        /// <param name="newStatus"></param>
        /// <param name="customText"></param>
        public void UpdatePresence(UserStateType newStatus, string? customText = null)
        {
            // 1. Guard Rail
            // Rule: You can only set a Custom status (2) if you are currently Active (1)
            if (newStatus == UserStateType.Custom && this.PresenceStatus == UserStateType.Inactive)
            {

                AppLogger.DebugState("UserEntity", $"Rejected status change: {Username} cannot go from Inactive to Custom directly.");
                return; //Exit the method without changing anything
            
            }

            // Rule: If you are Custom (2) and want to go back to Active (1), that is allowed.
            // Rule: Going to Inactive (0) is always allowed (for Logout)

            // 2. Capturing the old state ---
            UserStateType oldStatus = this.PresenceStatus;

            // 3. Apply Updates
            this.PresenceStatus = newStatus;
            this.LastActive = DateTime.UtcNow;

            // 4.Text Cleanup Logic
            if (newStatus == UserStateType.Custom)
            {

                this.CustomStatusText = customText;

            }
            else
            {

                this.CustomStatusText = null;
            
            }

            // 5. Logging
            AppLogger.DebugState("UserEntity", $"Status changed for {Username}: " +
                $"{(int)oldStatus} ({oldStatus}) -> {(int)newStatus} ({newStatus})");
        
        }


    }
}
