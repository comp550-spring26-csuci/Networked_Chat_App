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
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;


namespace Backend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;

        // Injection of UserRepository
        public TestController(IUserRepository userRepository, IConfiguration config)
        {

            _userRepository = userRepository;

            _config = config;
        }

        // Pretending this is what client would use to log in after they've created an account
        public class TestUserDto
        {
            public string? Email { get; set; }
            public string? Password { get; set; }
        }

        // Testing POST for creating a new user
        [HttpPost("seed-user")]
        public async Task<IActionResult> SeedUser(string UserName = "testAdmin2")
        {
            AppLogger.DebugState("TestController", "Seed attempt started");

            try
            {
                var oldUser = await _userRepository.GetByEmailAsync("test2@chat.com");

                var success = false;

                if (oldUser != null)
                {
                    _userRepository.Delete(oldUser);

                    // Committing to PostgreSQL databas
                    success = await _userRepository.SaveChangesAsync();
                }

                // Creating a dummy user
                var testUser = new User(UserName, $"{UserName}@chat.com", "HashedPassword1232");

                // Using the repository to add them
                await _userRepository.AddAsync(testUser);

                // Committing to PostgreSQL database
                success = await _userRepository.SaveChangesAsync();

                if (success)
                {
                    AppLogger.UserAction(testUser.Id.ToString(), "Created via Test Seed");

                    // Log in

                    TestUserDto loginDto = new TestUserDto
                    {
                        Email = "test2@chat.com",
                        Password = "HashedPassword1232"
                    };

                    var user = await _userRepository.GetByEmailAsync(loginDto.Email);

                    var tokenHandler = new JwtSecurityTokenHandler();

                    var keyInfo = _config.GetSection("JwtSettings:Key").Value;

                    var key = System.Text.Encoding.UTF8.GetBytes(keyInfo!);

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, user!.Id.ToString()),
                            new Claim(ClaimTypes.Email, user.Email),
                            new Claim(ClaimTypes.Name, user.Username)
                        }),
                        Expires = DateTime.UtcNow.AddHours(24),
                        Issuer = _config.GetSection("JwtSettings:Issuer").Value,
                        Audience = _config.GetSection("JwtSettings:Audience").Value,
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };

                    var token = tokenHandler.CreateToken(tokenDescriptor);

                    return Ok(new { token = tokenHandler.WriteToken(token), userId = user.Id, username = user.Username });

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
