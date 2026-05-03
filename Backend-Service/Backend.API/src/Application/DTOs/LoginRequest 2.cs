// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ivana Bavin-Gomez-San Basilio
//  Date: April 9 2026
//  Description: DTO envelope for incoming login credentials
// --------------------------------------------




namespace Backend.API.src.Application.DTOs
{
    public class LoginRequest
    {

        //----------------------------------
        //-------  Private Fields ----------
        //----------------------------------
        
        private string _username = string.Empty;
        private string _password = string.Empty;

        //----------------------------------
        //------  Getters and Setters ------
        //----------------------------------

        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        public string Password
        { 
            get { return _password; }
            set { _password = value; }

        }


    }
}
