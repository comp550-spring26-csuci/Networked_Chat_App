// --------------------------------------------------------------------------
//  Project: Network Chat App
//  Engineer: Ivana Bavin-Gomez-San Basilio
//  Date: May 10nd 2026
//  Description: Remove friends by using their username which needs to be unique
// ---------------------------------------------------------------------------

namespace Backend.API.src.Application.DTOs
{
    public class RemoveFriendByUsernameDto
    {
        //----------------------------------
        //-------  Private Fields ----------
        //----------------------------------

        private Guid _userId;
        private string _friendUsername = null!;

        //----------------------------------
        //------  Getters and Setters ------
        //--------------------------------

        public Guid UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        public string FriendUsername
        {
            get { return _friendUsername; }
            set { _friendUsername = value; }
        }
    }
}
