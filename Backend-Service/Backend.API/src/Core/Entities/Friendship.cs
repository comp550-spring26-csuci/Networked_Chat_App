// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ivana Bavin-Gomez-San Basilio
//  Date: May 2nd 2026
//  Description: Defines the Friendship entity.
//               This represents the many-to-many 
//               relationship between Users.
// --------------------------------------------


using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Data;


namespace Backend.API.src.Core.Entities
{
    public class Friendship
    {
        //----------------------------------
        //-------  Private Fields ----------
        //----------------------------------


        //-----Main class attributes

        // Unique internal ID for this specific relationship record
        private Guid _id = Guid.NewGuid();

        // ID of the User who initiated the Friendship (The Sender)
        private Guid _userId;

        // ID of the User who was added as a friend (The Receiver)
        private Guid _friendId;

        // Moment the friendship record was created
        private DateTime _createdAt = DateTime.UtcNow;

        // Numeric representation of the friendship state (0: pending, 1: Accepted, 2: Blocked)
        private int _status = 1; // Default to 1 (Accepted)


        //-----Navigation fields
        // Backing field for the User object who owns this relationship
        private User _user = null!;

        // backing field for the User object who is the friend in this relationship
        private User _friend = null!;


        //----------------------------------
        //------  Getters and Setters ------
        //----------------------------------


        //-----Main class attributes
        
        /// <summary>
        /// Primary Key: unique Identifier fr the Friendship record
        /// </summary>
        public Guid Id
        {
            get => _id;
            set => _id = value;
        }

        /// <summary>
        /// Foreign Key: The ID of the user initiating the relationship
        /// </summary>
        public Guid UserId
        {
            get => _userId;
            set
            {

                // Validation: Ensuring the Guid is not empty
                if (value != Guid.Empty) 
                { 
                    _userId = value;
                }
            }
        }

        /// <summary>
        /// Foreign Key: The ID of the target friend.
        /// </summary>
        public Guid FriendId
        {
            get => _friendId;
            set
            {
                if (value != Guid.Empty)
                {
                    _friendId = value;
                }
            
            }
        }

        /// <summary>
        /// The universal timestamp for when the friendship was established
        /// </summary>
        public DateTime CreatedAt
        {
            get => _createdAt;
            set => _createdAt = value;
        }

        //-----Status of Friendship Request

        /// <summary>
        /// The current state of the friendship
        /// 0 = pending, 1 = Accepted, 2 = Blocked
        /// </summary>
        public int Status
        {
            get => _status;
            set
            {
                // Validation: Only allowing specific statuses
                // (0: pending, 1: Accepted, 2: Blocked)
                if (value >= 0 && value <= 2)
                {
                    _status = value;
                }
            
            }
        }

        //-----Navigation fields

        /// <summary>
        /// Navigation property for the owner of the friendship.
        /// Marked as virtual to allow Entity Framework's "Lazy Loading".
        /// </summary>
        public virtual User User
        { 
            get => _user;
            set => _user = value;
        }

        /// <summary>
        /// Navigation property for the friend in teh relationship
        /// </summary>
        public virtual User Friend
        {
            get => _friend;
            set => _friend = value;
        }



        //----------------------------------
        //--------  Constructors -----------
        //----------------------------------

        /// <summary>
        /// Description: Constructor for manual creation (Add Friend action)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="friendId"></param>
        public Friendship(Guid userId, Guid friendId) 
        {
            UserId = userId;
            FriendId = friendId;
        }

        /// <summary>
        /// Protected constructor for Entity Framework Core
        /// </summary>
        protected Friendship() { }



    }
}
