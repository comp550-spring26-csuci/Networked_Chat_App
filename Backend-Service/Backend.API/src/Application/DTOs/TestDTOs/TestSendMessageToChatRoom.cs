// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: April 13 2026
//  Description: More verbose DTO envelope for sending a message to a chat room, containing the
//  SendMessageToChatRoom object, the username of the sender, and the name of the chat room.
//  This is used for testing purposes to ensure that the correct data is being sent from the
//  client to the server when a message is sent.
// --------------------------------------------

namespace Backend.API.src.Application.DTOs.TestDTOs
{
    public class TestSendMessageToChatRoom
    {
        private SendMessageToChatRoom _sendMessageToChatRoom = null!;
        private string? _chatRoomName;
        private Guid? _senderUserId;
        private string? _senderUsername;

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

        public Guid? SenderUserId
        {
            get => _senderUserId;
            set => _senderUserId = value;
        }

        public string? SenderUsername
        {
            get => _senderUsername;
            set => _senderUsername = value;
        }
    }
}
