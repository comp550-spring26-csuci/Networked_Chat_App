// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: April 13 2026
//  Description: Defines the TestSendMessageToChatRoom DTO for testing sending messages to chat rooms.
// --------------------------------------------

namespace Backend.API.src.Application.DTOs.TestDTOs
{
    public class TestSendMessageToChatRoom
    {
        private SendMessageToChatRoom _sendMessageToChatRoom = null!;
        private string? _chatRoomName;
        private string? _username;

        public SendMessageToChatRoom SendMessageToChatRoom
        {
            get => _sendMessageToChatRoom;
            set => _sendMessageToChatRoom = value;
        }

        public string? Username
        {
            get => _username;
            set => _username = value;
        }
    }
}
