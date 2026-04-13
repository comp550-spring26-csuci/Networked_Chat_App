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
        private readonly int _chatRoomId;
        private readonly string _chatRoomName = string.Empty;
        
        public int ChatRoomId
        {
            get => _chatRoomId;
        }

        public string ChatRoomName
        {
            get => _chatRoomName;
        }
    }
}
