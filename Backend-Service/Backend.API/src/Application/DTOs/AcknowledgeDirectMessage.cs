// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: April 14 2026
//  Description: Defines the AcknowledgeDirectMessage DTO for acknowledging direct messages.
// --------------------------------------------

namespace Backend.API.src.Application.DTOs
{
    public class AcknowledgeDirectMessage
    {
        private Guid _senderId;
        private string _username = default!;
        private Guid _chatRoomId;
        private string _chatRoomName = default!;

        public required Guid SenderId
        {
            get => _senderId;
            set => _senderId = value;
        }

        public required string Username
        {
            get => _username;
            set => _username = value;
        }

        public required Guid ChatRoomId
        {
            get => _chatRoomId;
            set => _chatRoomId = value;
        }

        public required string ChatRoomName
        {
            get => _chatRoomName;
            set => _chatRoomName = value;
        }
    }
}
