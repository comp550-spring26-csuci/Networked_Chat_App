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
        public int? ChatRoomId { get; set; }
        public required string Content { get; set; }
    }
}
