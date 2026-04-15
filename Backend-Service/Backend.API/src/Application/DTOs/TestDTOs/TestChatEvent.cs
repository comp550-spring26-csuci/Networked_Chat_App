using Backend.API.src.Core.Entities;

namespace Backend.API.src.Application.DTOs.TestDTOs
{
    public class TestChatEvent
    {
        private ChatEvent _chatEvent = null!;
        private string? _chatRoomName;

        public ChatEvent ChatEvent
        {
            get => _chatEvent;
            set => _chatEvent = value;
        }

        public string? ChatRoomName
        {
            get => _chatRoomName;
            set => _chatRoomName = value;
        }
    }
}
