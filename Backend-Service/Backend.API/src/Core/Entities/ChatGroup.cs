// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ivana Bavin-Gomez-San Basilio
//  Date: May 2nd 2026
//  Description: Defines the persistent ChatGroup entity.
// --------------------------------------------

using System;
using System.Collections.Generic;


namespace Backend.API.src.Core.Entities
{
    public class ChatGroup
    {

        //----------------------------------
        //-------  Private Fields ----------
        //----------------------------------

        private Guid _id = Guid.NewGuid();
        private string _name = string.Empty;
        private DateTime _createdAt = DateTime.UtcNow;
        private Guid _createdByUserId;


        //----------------------------------
        //------  Getters and Setters ------
        //----------------------------------

        public Guid Id
        {

            get { return _id; }
            set { _id = value; }
        
        }

        public string Name
        {

            get { return _name; }
            set
            {
                // Validation -> the group name cannot be empty
                if (!string.IsNullOrWhiteSpace(value))
                { 
                
                    _name = value;
                
                }
            
            }
        
        }


        public DateTime CreatedAt
        {

            get { return _createdAt; }
            set { _createdAt = value; }
        
        }

        public Guid CreatedByUserId
        {

            get { return _createdByUserId; }
            set { _createdByUserId = value; }

        }



        // ========================================================
        // EF CORE FIX: Auto-Property for Navigation Collection
        // ========================================================
        public ICollection<ChatGroupMember> Members { get; set; } = new List<ChatGroupMember>();



        //----------------------------------
        //---------- Constructors ----------
        //----------------------------------


        /// <summary>
        /// Public Constructor used to create a new ChatGroup
        /// </summary>
        /// <param name="name"></param>
        /// <param name="createdByUserId"></param>
        public ChatGroup(string name, Guid createdByUserId)
        { 
        
            Name = name;
            CreatedByUserId = createdByUserId;
        
        }


        /// <summary>
        /// Protected constructor to be used by the Entity Framework
        /// </summary>
        protected ChatGroup() 
        { 
        
        
        }

    }
}
