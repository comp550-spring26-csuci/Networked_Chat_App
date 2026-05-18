// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: May 16 2026
//  Description: DTO for representing a friend in the context of an event. This contains the
//  user's ID, username, presence status, and custom status text. This is used to represent
//  friends in events without exposing unnecessary user details.
// --------------------------------------------

using Backend.API.src.Core.Entities;

namespace Backend.API.src.Application.DTOs
{
    public class EventFriend
    {
        private Guid _userId;
        private string _username = default!;
        //private UserStateType _presenceStatus;
        private string? _customStatusText;

        public Guid UserId { get => _userId; set => _userId = value; }
        public string Username { get => _username; set => _username = value; }
        //public UserStateType PresenceStatus { get => _presenceStatus; set => _presenceStatus = value; }
        public string? CustomStatusText { get => _customStatusText; set => _customStatusText = value;  }

        public static EventFriend FromUser(User user) => new()
        {
            UserId = user.Id,
            Username = user.Username,
            //PresenceStatus = user.PresenceStatus,
            CustomStatusText = user.CustomStatusText
        };
    }
}
