// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: May 16, 2026
//  Description: This file defines the PerformChatRoomAction class, which is a Data Transfer Object (DTO) used to represent an action
//  performed on a chat room in the application. The class contains a single property for the ID of the chat room on which the action
//  is performed. This DTO is used to send information to clients about actions taken on chat rooms, allowing for updates to the user
//  interface to reflect changes in chat room status or membership.
// --------------------------------------------

namespace Backend.API.src.Application.DTOs
{
    public class PerformChatRoomAction
    {
        private Guid _chatRoomId;

        public Guid ChatRoomId { get => _chatRoomId; set => _chatRoomId = value; }
    }
}
