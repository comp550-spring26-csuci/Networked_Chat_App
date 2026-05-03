// ----------------------------------------------------------------------
//  Project: Network Chat App
//  Engineer: Ivana Bavin-Gomez-San Basilio
//  Date: April 9 2026
//  Description: Implements the authentication business logic.
//               Handles registration validation and logic verification.
// ----------------------------------------------------------------------


using System.Threading.Tasks;
using Backend.API.src.Core.Interface;
using Backend.API.src.Core.Entities;
using Backend.API.src.Application.DTOs;
using Backend.API.src.Core.Logging;



namespace Backend.API.src.Application.Services
{

    /// <summary>
    /// Service layer responsible for user identity rules
    /// </summary>
    public class AuthService : IAuthService
    {

        //----------------------------------
        //-------  Private Fields ----------
        //----------------------------------
        private readonly IUserRepository _userRepository;


        //----------------------------------
        //--------  Constructors -----------
        //----------------------------------
        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;

        }


        //----------------------------------
        //-----------  Methods -------------
        //----------------------------------

        /// <summary>
        /// 
        /// Verifies uniqueness and registers a new user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <param name="rawPassword"></param>
        /// <returns></returns>
        public async Task<AuthResult> CheckAndRegisterUserAsync(string username, string email, string rawPassword)
        {
            AppLogger.DebugState("AuthService", $"Attempting to register user: {username}");

            // 1. SECURITY: Checking if the Username or Email already exist
            bool usernameExists = await _userRepository.UsernameExistsAsync(username);
            bool emailExists = await _userRepository.EmailExistsAsync(email);

            if (usernameExists || emailExists)
            {
                AppLogger.DebugState("AuthService", "Registration failed: Username or Email already exists.");
                // We avoid giving specific information
                return new AuthResult(" It looks like an account with this information already exists.Please sign in or reset your password.");

            }


            // 2. CREATION: Generate the user entity
            // TO DO: We are passing a raw password, we will change it once we implement IPasswordHasher
            var newUser = new User(username, email, rawPassword);

            // 3. PERSISTENCE: We will store the information onf the database
            await _userRepository.AddAsync(newUser);

            // We save the changes in the PostgreSQL database
            await _userRepository.SaveChangesAsync();

            AppLogger.DebugState("AuthService", "User registered successfully.");
            return new AuthResult(newUser);

        
        }

        /// <summary>
        /// Verifies the existence of the user and the credentials for logging in
        /// </summary>
        /// <param name="username"></param>
        /// <param name="passwordProvided"></param>
        /// <returns></returns>
        public async Task<AuthResult> ValidateLoginAsync(string username, string passwordProvided)
        {
            AppLogger.DebugState("AuthService", $"Attempting login for user: {username}");

            // 1. Searching for the user in the database
            var existingUser = await _userRepository.GetByUsernameAsync(username);

            //2. Security: Verify if it exists and if the password is correct and a match
            // TO DO: For now is clear text in teh future will be hashed
            if (existingUser == null || existingUser.PasswordHash != passwordProvided)
            {

                AppLogger.DebugState("AuthService", "Login failed: Invalid credentials.");
                return new AuthResult("your credentials are not valid. Please check your username and password.");
            }

            AppLogger.DebugState("AuthService", "Login successful.");
            return new AuthResult(existingUser);
        
        }

    }
}
