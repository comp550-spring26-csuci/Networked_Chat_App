// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ivana Bavin-Gomez-San Basilio
//  Date: May 2nd 2026
//  Description: Defines the ChatGroupMember member
// --------------------------------------------

using System;
using System.Data;

namespace Backend.API.src.Core.Entities
{
    public class ChatGroupMember
    {

        //----------------------------------
        //-------  Private Fields ----------
        //----------------------------------

        private Guid _chatGroupId;
        private Guid _userId;
        private DateTime _joinedAt = DateTime.UtcNow;
        private int _unreadMessageCount;



        //----------------------------------
        //------  Getters and Setters ------
        //----------------------------------

        public Guid ChatGroupId
        {

            get { return _chatGroupId; }
            set { _chatGroupId = value;  }
        
        }

        public Guid UserId
        { 
        
            get { return _userId; }
            set { _userId = value; }
        
        }

        public DateTime JoinedAt
        {

            get { return _joinedAt; }
            set { _joinedAt = value; }
        
        }

        public int UnreadMessageCount
        { 
            get { return _unreadMessageCount; }
            set { _unreadMessageCount = value; }
        }

        // ========================================================
        // EF CORE FIX: Auto-Properties for Navigation 
        // We do not use private backing fields for these to prevent 
        // EF Core from trying to map them twice!
        // ========================================================
        public ChatGroup? ChatGroup { get; set; }
        public User? User { get; set; }


        //----------------------------------
        //--------  Constructors -----------
        //----------------------------------

        /// <summary>
        /// Public constructor used when a user joins a group
        /// </summary>
        /// <param name="chatGroupId"></param>
        /// <param name="userId"></param>
        public ChatGroupMember(Guid chatGroupId, Guid userId)
        { 
        
            ChatGroupId = chatGroupId;
            UserId = userId;
        
        }


        /// <summary>
        /// Protected constructor to be used by Entity Framework
        /// </summary>
        protected ChatGroupMember()
        {

        }


    }
}
