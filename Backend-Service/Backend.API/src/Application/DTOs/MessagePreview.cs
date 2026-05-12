// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: May 2, 2026
//  Description: DTO for message preview, containing the message ID, chat room ID, sender's username,
//  content preview, and timestamp. This is used to display a summary of messages in a chat room without
//  loading the full message details.
// --------------------------------------------

namespace Backend.API.src.Application.DTOs
{
    public class MessagePreview
    {
        private string? messageId;
        private Guid _chatRoomId;
        private string? _senderUsername;
        private string _content = default!;
        private DateTime _timestamp = DateTime.UtcNow;

        public string? MessageId
        {
            get => messageId;
            set => messageId = value;
        }

        public Guid ChatRoomId
        {
            get => _chatRoomId;
            set => _chatRoomId = value;
        }

        public string? SenderUsername
        {
            get => _senderUsername;
            set => _senderUsername = value;
        }

        public string Content
        {
            get => _content;
            set => _content = value;
        }

        public DateTime Timestamp
        {
            get => _timestamp;
            set => _timestamp = value;
        }
    }
}
