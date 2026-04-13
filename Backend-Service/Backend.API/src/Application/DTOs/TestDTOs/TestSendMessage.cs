// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: April 11 2026
//  Description: Defines the TestSendMessage DTO for testing message sending data transfer.
// --------------------------------------------

namespace Backend.API.src.Application.DTOs.TestDTOs
{
    public class TestSendMessage
    {
        private readonly SendMessage _sendMessage;
        private readonly string _chatRoomName = string.Empty;

        public SendMessage SendMessage
        {
            get => _sendMessage;
        }

        public string ChatRoomName
        {
            get => _chatRoomName;
        }
    }
}
