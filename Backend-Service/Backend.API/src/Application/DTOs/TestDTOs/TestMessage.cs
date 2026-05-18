// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: April 11 2026
//  Description: More verbose DTO envelope for a message, containing the Message object, the username of the sender,
//  and the name of the chat room. This is used for testing purposes to ensure that the correct data is being sent
//  from the client to the server when a message is sent.
// --------------------------------------------

namespace Backend.API.src.Application.DTOs.TestDTOs
{
    public class TestMessage
    {
        private MessageDto _message = default!;
        private string? _chatRoomName;

        public MessageDto Message { get => _message; set => _message = value; }
        public string? ChatRoomName { get => _chatRoomName; set => _chatRoomName = value; }
    }
}
