// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ivana Bavin-Gomez-San Basilio
//  Date: March 6 2026
//  Description: Defines the interface for the
//  process of checking if the user is already registered
// --------------------------------------------


using System.Data;
using System.Collections.Generic;
using Backend.API.src.Core.Entities;
using System.Threading.Tasks;
using Backend.API.src.Application.DTOs;



namespace Backend.API.src.Core.Interface
{
    /// <summary>
    /// Handles the business logic for user identity and security
    /// </summary>
    public interface IAuthService
    {

        /// <summary>
        /// Verifies if a user can be registered and saves them to the database
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <param name="rawPassword"></param>
        /// <returns></returns>
        Task<AuthResult> CheckAndRegisterUserAsync(string username, string email, string rawPassword);

        /// <summary>
        /// Verifies credentials for user login
        /// </summary>
        /// <param name="username"></param>
        /// <param name="passwordProvided"></param>
        /// <returns></returns>
        Task<AuthResult> ValidateLoginAsync(string username, string passwordProvided);
    }
}
