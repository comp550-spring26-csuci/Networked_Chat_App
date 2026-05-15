// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: May 14 2026
//  Description: This DTO is used to represent a friendship event in the context of an event. It contains only the
//  information about the friendship that is relevant to the event, such as the friend's ID, username, and status. This allows us to
//  include friendship information in events without having to include the entire User entity, which may
//  contain additional data that is not necessary for the event.
// --------------------------------------------

namespace Backend.API.src.Application.DTOs
{
    public class EventFriendship
    {
        private Guid _userId1;
        private Guid _userId2;
        private string _username1 = default!;
        private string _username2 = default!;
        //private UserStatusType _status1;
        //private UserStatusType _status2;
        private string? _statusMessage1;
        private string? _statusMessage2;

        public Guid UserId1
        {
            get => _userId1;
            set => _userId1 = value;
        }

        public Guid UserId2
        {
            get => _userId2;
            set => _userId2 = value;
        }

        public string Username1
        {
            get => _username1;
            set => _username1 = value;
        }

        public string Username2
        {
            get => _username2;
            set => _username2 = value;
        }

        //public UserStatusType Status1
        //{
        //    get => _status1;
        //    set => _status1 = value;
        //}

        //public UserStatusType Status2
        //{
        //    get => _status2;
        //    set => _status2 = value;
        //}

        public string? StatusMessage1
        {
            get => _statusMessage1;
            set => _statusMessage1 = value;
        }

        public string? StatusMessage2
        {
            get => _statusMessage2;
            set => _statusMessage2 = value;
        }
    }
}
