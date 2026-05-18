// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: May 2, 2026
//  Description: DTO for testing message preview, containing a MessagePreview object,
//  the chat room name, and the sender's ID. This is used for testing purposes to verify
//  that message previews are correctly generated and associated with the correct chat rooms and senders.
// --------------------------------------------

namespace Backend.API.src.Application.DTOs.TestDTOs
{
    public class TestMessagePreview
    {
        private MessagePreview _messagePreview = default!;
        private string? _chatRoomName;

        public required MessagePreview MessagePreview { get => _messagePreview; set => _messagePreview = value; }
        public string? ChatRoomName { get => _chatRoomName; set => _chatRoomName = value; }
    }
}
