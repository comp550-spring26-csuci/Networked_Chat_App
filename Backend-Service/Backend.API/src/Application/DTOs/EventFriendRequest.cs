// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: May 5 2026
//  Description: This DTO is used to represent a friend request event in the context of an event. It contains only the
//  information about the friend request that is relevant to the event, such as the requester's ID and username. This allows us to
//  include friend request information in events without having to include the entire User entity, which may
//  contain additional data that is not necessary for the event.
// --------------------------------------------

namespace Backend.API.src.Application.DTOs
{
    public class EventFriendRequest
    {
        private Guid _requesterId;
        private string _requesterUsername = default!;

        public Guid Id
        {
            get => _requesterId;
            set => _requesterId = value;
        }

        public string Username
        {
            get => _requesterUsername;
            set => _requesterUsername = value;
        }
    }
}
