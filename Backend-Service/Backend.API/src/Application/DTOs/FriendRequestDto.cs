// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ivana Bavin-Gomez-San Basilio
//  Date: May 2nd 2026
//  Description: Friend Request DTO
// --------------------------------------------


namespace Backend.API.src.Application.DTOs
{
    public class FriendRequestDto
    {

        //----------------------------------
        //-------  Private Fields ----------
        //----------------------------------

        private Guid _userId;
        private Guid _friendId;

        //----------------------------------
        //------  Getters and Setters ------
        //----------------------------------

        public Guid UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        public Guid FriendId
        {
            get { return _friendId; }
            set { _friendId = value; }
        }

    }
}
