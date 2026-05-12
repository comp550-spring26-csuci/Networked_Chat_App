// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ivana Bavin-Gomez-San Basilio
//  Date: March 6 2026
//  Description: Defines the interface for a User entity
//               This will match the database
// --------------------------------------------


using System;
using System.Data;
using System.Collections.Generic;
using Backend.API.src.Core.Enums;

namespace Backend.API.src.Core.Interface
{
    /// <summary>
    /// Interface User
    /// </summary>
    public interface IUser
    {
        //-----Main class attributes
        // UUID - Primary Key
        Guid Id { get; set; }
        // VARCHAR
        string Username { get; set; }
        // VARCHAR
        string Email { get; set; }
        // TEXT (allows NULL)
        string PasswordHash { get; set; }

        //-----Timestamp Related Attributes
        DateTime CreatedAt { get; set; }
        // This variable is nullable (in case they never logged in)
        DateTime? LastActive { get; set; }

        ///-----State of the User
        // Options will be "Online", "Offline", "DoNotDIsturbe"
        UserStateType PresenceStatus { get; set; }
        string? CustomStatusText { get; set; }


        //----------------------------------
        //-----------  Methods -------------
        //----------------------------------


        void UpdatePresence(UserStateType newStatus, string? customText = null);
        void MarkAsActive();

    }
}
