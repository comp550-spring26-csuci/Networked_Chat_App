// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: May 16, 2026
//  Description: This file defines the TestChatRoomRead class, which is a Data Transfer Object (DTO) used for testing
//  purposes in the chat application. The class contains properties for a ChatRoomRead object, the name of the chat room,
//  the user ID of the user who marked messages as read, and the username of that user. This DTO is used to simulate
//  or represent the data sent to clients when messages in a chat room are marked as read, allowing for testing of client-side
//  handling of read receipts and related events.
// --------------------------------------------

namespace Backend.API.src.Application.DTOs.TestDTOs
{
    public class TestChatRoomRead
    {
        private ChatRoomRead _chatRoomRead = default!;
        private string? _chatRoomName;
        private Guid? _userId;
        private string? _username;

        public ChatRoomRead ChatRoomRead { get => _chatRoomRead; set => _chatRoomRead = value; }
        public string? ChatRoomName { get => _chatRoomName; set => _chatRoomName = value; }
        public Guid? UserId { get => _userId; set => _userId = value; }
        public string? Username { get => _username; set => _username = value; }
    }
}
