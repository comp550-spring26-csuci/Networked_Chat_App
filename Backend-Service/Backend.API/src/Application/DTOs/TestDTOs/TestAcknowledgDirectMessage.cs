// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: April 16 2026
//  Description: More verbose DTO envelope for the acknowledgment of a direct message, containing the
//  AcknowledgeDirectMessage object, the username of the sender, and the name of the chat room.
// --------------------------------------------

namespace Backend.API.src.Application.DTOs.TestDTOs
{
    public class TestAcknowledgeDirectMessage
    {
        private AcknowledgeDirectMessage _acknowledgeDirectMessage = null!;
        private string _senderUsername = default!;
        private string _chatRoomName = default!;

        public required AcknowledgeDirectMessage AcknowledgeDirectMessage
        {
            get => _acknowledgeDirectMessage;
            set => _acknowledgeDirectMessage = value;
        }

        public required string SenderUsername
        {
            get => _senderUsername;
            set => _senderUsername = value;
        }

        public required string ChatRoomName
        {
            get => _chatRoomName;
            set => _chatRoomName = value;
        }
    }
}
