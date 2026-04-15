// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: April 11 2026
//  Description: Defines the TestSendMessage DTO for testing the SendMessage data transfer in the network chat application.
// --------------------------------------------

namespace Backend.API.src.Application.DTOs.TestDTOs
{
    public class TestSendMessage
    {
        private SendMessage _sendMessage = null!;
        private string? _username;

        public SendMessage SendMessage
        {
            get => _sendMessage;
            set => _sendMessage = value;
        }

        public string? Username
        {
            get => _username;
            set => _username = value;
        }
    }
}
