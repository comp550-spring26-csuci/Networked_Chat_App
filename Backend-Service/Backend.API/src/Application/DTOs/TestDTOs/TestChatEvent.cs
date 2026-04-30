// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: April 13 2026
//  Description: More verbose DTO envelope for a chat event, containing the ChatEvent object
//  and the name of the chat room. This is used for testing purposes to ensure that the correct
//  data is being sent from the client to the server when a chat event occurs.
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
