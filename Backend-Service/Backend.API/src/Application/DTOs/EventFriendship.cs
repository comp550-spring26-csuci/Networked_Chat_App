// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: May 14 2026
//  Description: DTO for representing a friendship between two users in the context of an event. This contains two EventFriend objects,
//  which are simplified representations of users that include only the information relevant to the event. This is used to represent
//  friendships in events without exposing unnecessary user details.
// --------------------------------------------

using Backend.API.src.Core.Entities;

namespace Backend.API.src.Application.DTOs
{
    public class EventFriendship
    {
        private EventFriend _initiatingUser = default!;
        private EventFriend _affectedUser = default!;

        public EventFriend InitiatingUser { get => _initiatingUser; set => _initiatingUser = value; }
        public EventFriend AffectedUser { get => _affectedUser; set => _affectedUser = value; }

        public static EventFriendship FromUsers(User initiating, User affected) => new()
        {
            InitiatingUser = EventFriend.FromUser(initiating),
            AffectedUser = EventFriend.FromUser(affected)
        };
    }
}
