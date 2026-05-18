// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: April 13 2026
//  Description: Defines the SendMessageToChatRoom DTO for sending messages to a specific chat room.
// --------------------------------------------

namespace Backend.API.src.Application.DTOs
{
    public class SendMessageToChatRoom
    {
        private Guid _chatRoomId;
        private string _content = default!;

        public Guid ChatRoomId { get => _chatRoomId; set => _chatRoomId = value; }
        public required string Content { get => _content; set => _content = value; }
    }
}
