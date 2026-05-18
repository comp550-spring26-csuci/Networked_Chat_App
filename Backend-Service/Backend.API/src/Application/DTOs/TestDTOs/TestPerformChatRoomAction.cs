// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: May 16, 2026
//  Description: This file defines the TestPerformChatRoomAction class, which is a Data Transfer Object (DTO) used for testing
//  purposes in the chat application. The class contains properties for a PerformChatRoomAction object, the name of the chat room,
//  the user ID of the user performing the action, and the username of that user. This DTO is used to simulate or represent
//  the data sent to clients when a user performs an action related to a chat room (such as joining or leaving), allowing for testing
//  of client-side handling of such events.
// --------------------------------------------

namespace Backend.API.src.Application.DTOs.TestDTOs
{
    public class TestPerformChatRoomAction
    {
        private PerformChatRoomAction _performChatRoomAction = default!;
        private string? _chatRoomName;
        private Guid? _userId;
        private string? _username;

        public required PerformChatRoomAction PerformChatRoomAction { get => _performChatRoomAction; set => _performChatRoomAction = value; }
        public string? ChatRoomName { get => _chatRoomName; set => _chatRoomName = value; }
        public Guid? UserId { get => _userId; set => _userId = value; }
        public string? Username { get => _username; set => _username = value; }
    }
}
