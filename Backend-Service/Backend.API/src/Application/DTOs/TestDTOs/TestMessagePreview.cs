namespace Backend.API.src.Application.DTOs.TestDTOs
{
    public class TestMessagePreview
    {
        private MessagePreview _messagePreview = null!;
        private string? _chatRoomName;
        private Guid _senderId = default!;

        public MessagePreview MessagePreview
        {
            get => _messagePreview;
            set => _messagePreview = value;
        }

        public string? ChatRoomName
        {
            get => _chatRoomName;
            set => _chatRoomName = value;
        }

        public Guid SenderId
        {
            get => _senderId;
            set => _senderId = value;
        }
    }
}
