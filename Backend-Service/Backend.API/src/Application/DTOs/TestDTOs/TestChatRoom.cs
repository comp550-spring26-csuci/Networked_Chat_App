// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: April 11 2026
//  Description: Defines the TestChatRoom DTO for testing chat room data transfer.
// --------------------------------------------

namespace Backend.API.src.Application.DTOs.TestDTOs
{
    public class TestChatRoom
    {
        private int? _chatRoomId;
        private string? _chatRoomName;

        public int? ChatRoomId
        {
            get => _chatRoomId;
            set => _chatRoomId = value;
        }

        public string? ChatRoomName
        {
            get => _chatRoomName;
            set => _chatRoomName = value;
        }
    }
}
