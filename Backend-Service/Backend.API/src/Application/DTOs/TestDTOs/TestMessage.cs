// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: April 11 2026
//  Description: Defines the TestMessage DTO for testing message data transfer.
// --------------------------------------------

using Backend.API.src.Core.Entities;

namespace Backend.API.src.Application.DTOs.TestDTOs
{
    public class TestMessage
    {
        private Message _message;
        private string _chatRoomName = string.Empty;

        public Message Message
        {
            get => _message;
            set => _message = value;
        }

        public string ChatRoomName
        {
            get => _chatRoomName;
            set => _chatRoomName = value;
        }
    }
}
