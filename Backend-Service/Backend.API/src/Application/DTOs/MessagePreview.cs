namespace Backend.API.src.Application.DTOs
{
    public class MessagePreview
    {
        private Guid _chatRoomId;
        private string? _senderUsername;
        private string _content = default;
        private DateTime _timestamp = DateTime.UtcNow;

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
