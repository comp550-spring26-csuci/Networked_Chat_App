// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: April 14 2026
//  Description: DTO for acknowledging a direct message, containing the sender's ID and the chat room ID.
// --------------------------------------------

namespace Backend.API.src.Application.DTOs
{
    public class AcknowledgeDirectMessage
    {
        private Guid _senderId;
        private Guid _chatRoomId;
        
        public required Guid SenderId
        {
            get => _senderId;
            set => _senderId = value;
        }

        public required Guid ChatRoomId
        {
            get => _chatRoomId;
            set => _chatRoomId = value;
        }
    }
}
