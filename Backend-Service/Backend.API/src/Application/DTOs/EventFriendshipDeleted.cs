// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: May 16, 2026
//  Description: This file defines the EventFriendshipDeleted class, which is a Data Transfer Object (DTO) used to represent the deletion
//  of a friendship between two users in the application. The class contains properties for the IDs of both the initiating user (the user
//  who deleted the friendship) and the affected user (the user whose friendship was deleted). This DTO is used to send information to
//  clients about which friendships have been deleted, allowing for updates to the user interface to reflect the change in friendship
//  status between users.
// --------------------------------------------

namespace Backend.API.src.Application.DTOs
{
    public class EventFriendshipDeleted
    {
        private Guid _initiatingUser;
        private Guid _affectedUser;

        public Guid InitiatingUser { get => _initiatingUser; set => _initiatingUser = value; }
        public Guid AffectedUser { get => _affectedUser; set => _affectedUser = value; }

        public static EventFriendshipDeleted FromIds(Guid initiatingUser, Guid affectedUser) => new()
        {
            InitiatingUser = initiatingUser,
            AffectedUser = affectedUser
        };
    }
}
