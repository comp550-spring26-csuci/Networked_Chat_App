// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: May 16, 2026
//  Description: This file defines the ChatRoomRead class, which is a Data Transfer Object (DTO) used to represent the read status of
//  messages in a chat room. The class contains a single property for the chat room's ID. This DTO is used to send information
//  to clients about which chat rooms have had their messages marked as read, allowing for updates to the user interface to reflect read
//  receipts and related features.
// --------------------------------------------

namespace Backend.API.src.Application.DTOs
{
    public class ChatRoomRead
    {
        private Guid _id; 

        public Guid Id { get => _id; set => _id = value; }
    }
}
