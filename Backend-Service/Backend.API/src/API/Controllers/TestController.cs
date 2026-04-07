// -------------------------------------------------------------------
//  Project: Network Chat App
//  Engineer: Ivana Bavin-Gomez-San Basilio
//  Date: March 7th 2026
//  Description: S,moke test for the UserRepository
//		fucntions
// -------------------------------------------------------------------


using Microsoft.AspNetCore.Mvc;
using Backend.API.src.Core.Entities;
using Backend.API.src.Core.Interface;
using Backend.API.src.Core.Logging;
using FluentValidation;
using Backend.API.src.Application.DTOs;


namespace Backend.API.src.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IValidator<CreateAccountRequest> _validator;

        // Injection of UserRepository and the validator
        public TestController(IUserRepository userRepository, IValidator<CreateAccountRequest> validator)
        {

            _userRepository = userRepository;
            _validator = validator;
        }


        // -------------------------------------
        // *****VALIDATED ACCOUNT CREATION******
        // -------------------------------------

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

                // 2. After comfirming that the Age is validated
                var newUser = new User(request.Username, request.Email, request.Password);

                // 3. Saving to the database using the repository
                await _userRepository.AddAsync(newUser);
                var success = await _userRepository.SaveChangesAsync();

                if (success)
                {
                    AppLogger.UserAction(newUser.Id.ToString(), "The account was created after being validated.");
                    return Ok(new {  Message = "Welcome!. The account was created successfully.", UserId = newUser.Id});
                }


                return BadRequest("The energetic transfer to the database failed.");

            } 
            catch (Exception ex)
            {
                AppLogger.ShieldFailure("testController", ex);
                return StatusCode(500, $"Internal Error: {ex.Message}");

            }
            
        }


        // -------------------------------------
        // *********** SMOKE TESTS**************
        // -------------------------------------

        // Testing POST for creating a new user
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

        // Testing GET to see all the users
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

    }
}
