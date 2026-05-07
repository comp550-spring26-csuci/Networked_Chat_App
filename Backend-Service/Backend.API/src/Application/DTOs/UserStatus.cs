// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: May 5 2026
//  Description: A relatively simple perk of having friends could be to see whether or not they have one or more clients
//  connected to the chat server. This DTO is used to return that information in a consistent way.
// --------------------------------------------

namespace Backend.API.src.Application.DTOs
{
    public class UserStatus
    {
        private Guid _userId;

        public Guid UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }
    }
}
