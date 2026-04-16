// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: April 14 2026
//  Description: Defines the StartDirectMessage DTO for initiating a direct message between users.
// --------------------------------------------

namespace Backend.API.src.Application.DTOs
{
    public class StartDirectMessage
    {
        private Guid _otherUserId;
        private string _chatRoomName = default!;

        public required Guid OtherUserId
        {
            get => _otherUserId;
            set => _otherUserId = value;
        }

        public string ChatRoomName
        {
            get => _chatRoomName;
            set => _chatRoomName = value;
        }
    }
}
