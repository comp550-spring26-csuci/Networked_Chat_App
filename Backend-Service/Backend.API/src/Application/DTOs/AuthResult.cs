// ----------------------------------------------------------------------------------------
//  Project: Network Chat App
//  Engineer: Ivana Bavin-Gomez-San Basilio
//  Date: April 8 2026
//  Description: Defines the Data Transfer Object for authentication results.
//               Features encapsulated fields for protection and immutability.
// ----------------------------------------------------------------------------------------

using System;
using Backend.API.src.Core.Entities;


namespace Backend.API.src.Application.DTOs
{
    /// <summary>
    /// DTO for outgoing authentication and registration results
    /// </summary>
    public class AuthResult
    {

        //----------------------------------
        //-------  Private Fields ----------
        //----------------------------------

        //-----Main class attributes

        private bool _isSuccess;
        private string _errorMessage = string.Empty;
        private User? _createdUser;

        //----------------------------------
        //------  Getters and Setters ------
        //----------------------------------

        //-----Main class attributes

        public bool IsSuccess
        { 
            get { return _isSuccess; }
            // No setter to protect the integrity of the response
        
        }

        public string ErrorMessage 
        {
            get { return _errorMessage; }
            // No setter to protect the integrity of the response       
        }

        public User? CreatedUser
        {
            get { return _createdUser; }
            // No setter to protect the integrity of the response
        }

        //----------------------------------
        //---------  Constructors ----------
        //----------------------------------

        /// <summary>
        /// Public constructor for a failed authentication or registration attempt
        /// </summary>
        /// <param name="errorMessage"></param>
        public AuthResult(string errorMessage)
        {
            _isSuccess = false;
            _errorMessage = errorMessage;
            _createdUser = null;

        }

        /// <summary>
        /// Public constructor for a successful authentication or registration attempt
        /// </summary>
        /// <param name="user"></param>
        public AuthResult(User user) 
        {
            _isSuccess = true;
            _errorMessage = String.Empty;
            _createdUser = user;
        }
    }
}
