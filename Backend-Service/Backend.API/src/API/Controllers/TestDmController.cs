// -------------------------------------------------------------------
//  Project: Network Chat App
//  Engineer: Ivana Bavin-Gomez-San Basilio (Early version modified by Ian Milin)
//  Date: March 7th 2026
//  Description: Smoke test for the UserRepository
//		functions
// -------------------------------------------------------------------

using Backend.API.src.Application.Services;
using Backend.API.src.Core.Entities;
using Backend.API.src.Core.Interface;
using Backend.API.src.Core.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.src.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestDmController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtTokenService _tokenService;

        // Injection of UserRepository
        public TestDmController(IUserRepository userRepository, JwtTokenService tokenService)
        {

            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        // Pretending this is what client would use to log in after they've created an account
        public class TestUserDto
        {
            public string? Email { get; set; }
            public string? Password { get; set; }
        }

        // Testing POST for creating a new user
        [HttpPost("seed-create-users")]
        public async Task<IActionResult> SeedCreateUsers()
        {
            AppLogger.DebugState("TestController", "Seed attempt started");

            try
            {
                bool updateSuccess = false;
                foreach (var UserName in new[] { "ian", "kenneth", "brielle", "ivana" })
                {
                    var oldUser = await _userRepository.GetByEmailAsync($"{UserName}@chat.com");

                    var success = false;

                    User testUser;

                    if (oldUser != null && oldUser.Username != UserName)
                    {
                        _userRepository.Delete(oldUser);

                        // Committing to PostgreSQL databas
                        success = await _userRepository.SaveChangesAsync();

                        oldUser = null;
                    }

                    if (oldUser == null)
                    {
                        // Creating a dummy user
                        testUser = new User(UserName, $"{UserName}@chat.com", "HashedPassword1232");

                        // Using the repository to add them
                        await _userRepository.AddAsync(testUser);

                        // Committing to PostgreSQL database
                        success = await _userRepository.SaveChangesAsync();
                    }
                    else
                    {
                        testUser = oldUser;
                    }

                    if (success || testUser == oldUser)
                    {
                        if (testUser == oldUser)
                        {
                            AppLogger.DebugState("TestController", "User already exists, skipping creation");
                        }
                        else
                        {
                            AppLogger.UserAction(testUser.Id.ToString(), "Created via Test Seed");
                        }
                    }

                    if (success)
                    {
                        updateSuccess = true;
                    }
                }

                if (updateSuccess)
                {
                    return Ok(new { message = "Users seeded successfully with updates." });
                }
                else
                {
                    return Ok(new { message = "Users seeded successfully without updates." });
                }
            }
            catch (Exception ex)
            {
                AppLogger.ShieldFailure("TestCOntroller", ex);
                return StatusCode(500, $"Interal Error: {ex.Message}");
            }
        }

        // Testing POST for creating a new user
        [HttpPost("seed-user")]
        public async Task<IActionResult> SeedUser(string UserName = "testAdmin2", bool OverWrite = true)
        {
            AppLogger.DebugState("TestController", "Seed attempt started");

            switch (UserName)
            {
                case "ian":
                case "kenneth":
                case "brielle":
                case "ivana":
                    break;
                default:
                    return BadRequest("Invalid username. Please use 'ian', 'kenneth', 'brielle', or 'ivana'.");
            }

            if (OverWrite == true)
            {
                return BadRequest("OverWrite is set to true, which will delete existing user. Please set to false if you do not want to overwrite.");
            }

            try
            {
                var oldUser = await _userRepository.GetByEmailAsync($"{UserName}@chat.com");

                var success = false;

                User testUser;

                if (oldUser != null && OverWrite)
                {
                    _userRepository.Delete(oldUser);

                    // Committing to PostgreSQL databas
                    success = await _userRepository.SaveChangesAsync();
                }

                if (oldUser == null || OverWrite)
                {
                    // Creating a dummy user
                    testUser = new User(UserName, $"{UserName}@chat.com", "HashedPassword1232");

                    // Using the repository to add them
                    await _userRepository.AddAsync(testUser);

                    // Committing to PostgreSQL database
                    success = await _userRepository.SaveChangesAsync();
                }
                else
                {
                    testUser = oldUser;
                }

                if (success || testUser == oldUser)
                {
                    if (testUser == oldUser)
                    {
                        // AppLogger.DebugState("TestController", "User already exists, skipping creation");
                    }
                    else
                    {
                        AppLogger.UserAction(testUser.Id.ToString(), "Created via Test Seed");
                    }

                    // Log in

                    TestUserDto loginDto = new TestUserDto
                    {
                        Email = $"{UserName}@chat.com",
                        Password = "HashedPassword1232"
                    };

                    var user = await _userRepository.GetByEmailAsync(loginDto.Email);

                    if (user == null)
                    {
                        return NotFound("User not found");
                    }

                    var token = _tokenService.GenerateToken(user, TimeSpan.FromHours(24));

                    return Ok(new { token, userId = user.Id, username = user.Username });
                }

                return BadRequest("Failed to save user to database");
            }
            catch (Exception ex)
            {
                AppLogger.ShieldFailure("TestCOntroller", ex);
                return StatusCode(500, $"Interal Error: {ex.Message}");

            }
        }
    }
}
