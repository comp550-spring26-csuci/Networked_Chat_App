// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: April 13 2026
//  Description: Defines the TestChatEvent DTO for testing chat event data transfer.
// --------------------------------------------

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
