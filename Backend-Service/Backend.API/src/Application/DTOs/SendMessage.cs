// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: April 11 2026
//  Description: Defines the SendMessage DTO for sending messages to a chat room.
// --------------------------------------------

namespace Backend.API.src.Application.DTOs
{
    public class SendMessage
    {
        private readonly int _chatRoomId;
        private readonly string _content = string.Empty;
        private readonly string _username = string.Empty;
        
        public int ChatRoomId
        {
            get => _chatRoomId;
        }

        public string Content
        {
            get => _content;
        }
        
        public string Username
        {     
            get => _username;
        }
    }
}
