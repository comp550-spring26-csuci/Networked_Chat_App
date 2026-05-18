// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: May 16, 2026
//  Description: This file defines the EventChatGroupMembershipDeleted class, which is a Data Transfer Object (DTO) used to represent the
//  deletion of a chat group membership in the application. The class contains a single property for the ID of the deleted chat group
//  membership. This DTO is used to send information to clients about which chat group memberships have been deleted, allowing for
//  updates to the user interface to reflect the removal of the chat group membership and any related data.
// --------------------------------------------

namespace Backend.API.src.Application.DTOs
{
    public class EventChatGroupMembershipDeleted
    {
        private Guid _id;

        public Guid Id { get => _id; set => _id = value; }
    }
}
