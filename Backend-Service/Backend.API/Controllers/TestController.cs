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


namespace Backend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        // Injection of UserRepository
        public TestController(IUserRepository userRepository)
        {

            _userRepository = userRepository;
        }

        // Testing POST for creating a new user
        [HttpPost("seed-user")]
        public async Task<IActionResult> SeedUser()
        {
            AppLogger.DebugState("TestController", "Seed attempt started");

            try
            {

                // Creating a dummy user
                var testUser = new User("testAdmin2", "test2@chat.com", "HashedPassword1232");

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
