// --------------------------------------------------------------------------
//  Project: Network Chat App
//  Engineer: Ivana Bavin-Gomez-San Basilio
//  Date: May 4th 2026
//  Description: DTO for returning a friend's details (ID and Username) 
//               in a list view.
// ---------------------------------------------------------------------------



namespace Backend.API.src.Application.DTOs
{
    public class FriendUserDto
    {

        //----------------------------------
        //-------  Private Fields ----------
        //----------------------------------

        private Guid _id;
        private string _username;

        //----------------------------------
        //------  Getters and Setters ------
        //----------------------------------



        public Guid Id
        {
            get
            { 
                return _id; 
            }

            set
            {
                _id = value;
            }
        }

        public string Username
        {
            get
            {
                return _username;
            }

            set
            {
                _username = value;
            }
        }


    }
}
