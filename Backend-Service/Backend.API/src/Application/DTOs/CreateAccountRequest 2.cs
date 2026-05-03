// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ivana Bavin-Gomez-San Basilio
//  Date: April 5 2026
//  Description: Defines the Data Transfer Object for creating an account.
//               Features encapsulated fields for energetic protection.
// --------------------------------------------




using System;

namespace Backend.API.src.Application.DTOs
{

    /// <summary>
    ///  DTO for incoming account creation requests
    /// </summary>
    public class CreateAccountRequest
    {

        //----------------------------------
        //-------  Private Fields ----------
        //----------------------------------

        //-----Main class attributes
        private string _username = string.Empty;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private int _age;

        //----------------------------------
        //------  Getters and Setters ------
        //----------------------------------


        //-----Main class attributes

        public string Username
        {
            get { return _username; }
            set
            {
                // Validation -> the username can not be empty
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _username = value;
                }
            }

        }

        public string Email
        {
            get { return _email; }
            set
            {
                // Validation -> the email can not be empty, basic check for @
                if (!string.IsNullOrWhiteSpace(value) && value.Contains("@"))
                {
                    _email = value;
                }
            }

        }


        public string Password
        {
            get { return _password; }
            set
            {
                // Validation -> the passwordhash can not be empty
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _password = value;
                }
            }

        }


        public int Age
        {
            get { return _age; }
            set { _age = value; }
        }



        //----------------------------------
        //--------  Constructors -----------
        //----------------------------------

        /// <summary>
        /// Public constructor for manual instantiation
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="age"></param>
        public CreateAccountRequest(string username, string email, string password, int age)
        {
            Username = username;
            Email = email;
            Password = password;
            Age = age;

        }

        /// <summary>
        ///  Parameterless cosntruction requered by AP.NET Core for JSON desirialization
        /// </summary>
        public CreateAccountRequest()
        {
        }
    }

}


