// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: April 13 2026
//  Description
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

        public string? ChatRoomName
        {
            get => _chatRoomName;
            set => _chatRoomName = value;
        }

        public string? Username
        {
            get => _username;
            set => _username = value;
        }
    }
}
