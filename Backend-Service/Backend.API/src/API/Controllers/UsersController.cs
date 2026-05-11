// -------------------------------------------------------------------
//  Project: Network Chat App
//  Engineer: Ivana Bavin-Gomez-San Basilio
//  Date: March 7th 2026
//  Description: Smoke test for the UserRepository
//		functions
// -------------------------------------------------------------------


using Microsoft.AspNetCore.Mvc;
using Backend.API.src.Core.Entities;
using Backend.API.src.Core.Interface;
using Backend.API.src.Core.Logging;
using FluentValidation;
using Backend.API.src.Application.DTOs;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace Backend.API.src.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IValidator<CreateAccountRequest> _validator;
        private readonly IAuthService _authService;
        private readonly IValidator<LoginRequest> _loginValidator;
        private readonly IUserRepository _userRepo;

        // Injection of UserRepository, the validator, and AuthService
        public UsersController(
            IUserRepository userRepository, 
            IValidator<CreateAccountRequest> validator,
            IAuthService authService,
            IValidator<LoginRequest> loginValidator,
            IUserRepository userRepo)
        {

            _userRepository = userRepository;
            _validator = validator;
            _authService = authService;
            _loginValidator = loginValidator;
            _userRepo = userRepo;
        }


        // -------------------------------------
        // *****VALIDATED ACCOUNT CREATION******
        // -------------------------------------

        /// <summary>
        /// URL: POST api/users/create-account
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("create-account")]
        public async Task<IActionResult> CreateAccount([FromBody]CreateAccountRequest request)
        {
            AppLogger.DebugState("TestController", $"Account creation request initiated for: {request.Email}");

            try
            {
                // 1. Validate the incoming data (DTO envelope)
                var validationResult = await _validator.ValidateAsync(request);

                if (!validationResult.IsValid)
                {
                    var errorMessages = string.Join(" | ", validationResult.Errors.Select(equals => equals.ErrorMessage));
                    AppLogger.DebugState("TestController", $"Validation failed for {request.Email}, Reasons: {errorMessages}");

                    return BadRequest(validationResult.Errors);
            
                }


                // 2. Delegating the creation and validation of dulicates to the AuthService
                var authResult = await _authService.CheckAndRegisterUserAsync(request.Username, request.Email, request.Password);

                // 3. We verify if there was a conflict (e.g. if user already exists)
                if (!authResult.IsSuccess)
                {
                    AppLogger.DebugState("TestController", $"Business validation failed: {authResult.ErrorMessage}");
                    // We return error 409 conflict with the message
                    return Conflict(new { Message = authResult.ErrorMessage });
                }

                // 4. Success
                AppLogger.UserAction(authResult.CreatedUser!.Id.ToString(), "The account was created after being validated securely.");
                return Ok(new
                {
                    Message = "Welcome!. The account was created successfully.",
                    UserId = authResult.CreatedUser.Id,
                    Token = authResult.Token // The front end can see the token
                });

            } 
            catch (Exception ex)
            {
                AppLogger.ShieldFailure("testController", ex);
                return StatusCode(500, $"Internal Error: {ex.Message}");

            }
            
        }


        // -------------------------------------
        // *********** USER LOGIN **************
        // -------------------------------------

        /// <summary>
        /// URL: POST api/users/login
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("login")]
        // We use the class LoginRequest
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {

            // We check if the request is empty or invalid before even touching the data
            var validationResult = await _loginValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                AppLogger.DebugState("testController", "Login blocked: Empty or invalid fields provided.");
                return BadRequest(validationResult.Errors);
            }

            AppLogger.DebugState("TestController", $"Login attempt initiated for user: {request.Username}");

            try
            {
                // 1. Passing the data from the LoginRequest to the AuthService
                // the AuthService class will communicate with teh database
                var authResult = await _authService.ValidateLoginAsync(request.Username, request.Password);

                // 2. We Check if all was successful
                if (!authResult.IsSuccess)
                {
                    AppLogger.DebugState("testController", $"Login rejected: {authResult.ErrorMessage}");
                    return Unauthorized(new { Message = authResult.ErrorMessage });
                }

                // 3. If everything is successful we return a success message
                AppLogger.UserAction(authResult.CreatedUser!.Id.ToString(), "User logged in successfully.");


                return Ok(new
                {
                    Message = "Login successful! Welcome back.",
                    UserId = authResult.CreatedUser.Id,
                    Token = authResult.Token // The front end can see the token
                });

            }
            catch (Exception ex)
            {
                AppLogger.ShieldFailure("TestController", ex);
                return StatusCode(500, $"Internal Error: {ex.Message}");
            }

        }


        /// <summary>
        /// Find a user's ID by their username
        /// URL: GET api/users/search/Alegria5
        /// </summary>
        [HttpGet("search/{username}")]
        public async Task<IActionResult> SearchByUsername(string username)
        {
            // 1. Call your existing repository method
            var user = await _userRepo.GetByUsernameAsync(username);

            // 2. If the user doesn't exist, return a 404
            if (user == null)
            {
                return NotFound(new { message = $"User '{username}' not found." });
            }

            // 3. Return the ID so the frontend can use it for the friend request
            return Ok(new
            {
                id = user.Id,
                username = user.Username
            });
        }


        // -------------------------------------
        // *********** SMOKE TESTS**************
        // -------------------------------------


        /// <summary>
        /// Testing POST for creating a new user
        /// URL: POST api/users/seed-user
        /// </summary>
        /// <returns></returns>
        [HttpPost("seed-user")]
        public async Task<IActionResult> SeedUser()
        {
            AppLogger.DebugState("TestController", "Seed attempt started");

            try
            {

                // Creating a dummy user
                var testUser = new User("testAdmi8", "test8@chat.com", "HashedPassword1328");

                // Using the repository to add them
                await _userRepository.AddAsync(testUser);

                // Committing to PostgreSQL database
                var success = await _userRepository.SaveChangesAsync();

                if (success)
                {
                    AppLogger.UserAction(testUser.Id.ToString(), "Created via Test Seed");
                    return Ok(new { Message = "User created successfully!", UserId = testUser.Id });

                }

                return BadRequest("Failed to save user to database");
            }
            catch (Exception ex)
            {
                AppLogger.ShieldFailure("TestCOntroller", ex);
                return StatusCode(500, $"Interal Error: {ex.Message}");

            }

        }


        /// <summary>
        /// Testing GET to see all the users
        /// URL: GET api/users/all-users
        /// </summary>
        /// <returns></returns>
        [HttpGet("all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                //Asking the dabase for all the users
                var users = await _userRepository.GetAllAsync();

                // If it is empty we will let the user know
                if (users == null || !users.Any())
                {
                    return Ok(new { Message = "The Library is currently empty}" });
                }
                return Ok(users);
            }
            catch (Exception ex)
            {
                // If there is any issues we will log them
                return StatusCode(500, $"Internal Error: {ex.Message}");
            }


        }


        /// <summary>
        /// Deleting the content "Users" table of the POSTGRESQL db, applying CASCADE
        /// URL: DELETE api/users/purge-all-users
        /// </summary>
        /// <returns></returns>
        [HttpDelete("purge-all-users")]
        public async Task<IActionResult> PurgeUsers()
        {
            AppLogger.DebugState("TestController", "Deletion of Users table received.");

            try
            {
                await _userRepository.ClearAllUsersAsync();
                return Ok(new { Message = "Database deletion. All user entries were removed." });
            }
            catch (Exception ex)
            {
                AppLogger.ShieldFailure("testController", ex);
                return StatusCode(500, $"UInternal error during Users table deletion: {ex.Message}");

            }
        }


    }
}
