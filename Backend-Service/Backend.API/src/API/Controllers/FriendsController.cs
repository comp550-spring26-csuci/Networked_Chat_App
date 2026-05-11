// ---------------------------------------------------------------------------
//  Project: Network Chat App
//  Engineer: Ivana Bavin-Gomez-San Basilio
//  Date: May 2nd 2026
//  Description: Controller to handle friendship requests and management.
// ---------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Backend.API.src.Core.Entities;
using Backend.API.src.Core.Interface;
using System.ComponentModel.DataAnnotations;
using Backend.API.src.Application.DTOs;


namespace Backend.API.src.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FriendsController : ControllerBase
    {

        private readonly IFriendshipRepository _friendRepo;
        private readonly IUserRepository _userRepo;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="friendRepo"></param>
        public FriendsController(IFriendshipRepository friendRepo, IUserRepository userRepo)
        {

            _friendRepo = friendRepo;
            _userRepo = userRepo;

        }

        /// <summary>
        /// Endpoint to add a friend by their IDs
        /// URL: POST api/friends/add
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="friendId"></param>
        /// <returns></returns>
        [HttpPost("add-by-guid")]
        public async Task<IActionResult> AddFriendByGuid([FromBody] FriendRequestDto request)
        {

            // 1. Validation: You can't be friends with yourself
            if (request.UserId == request.FriendId)
            {
                return BadRequest("You cannot add yourself as a friend.");
            }

            // 2. Check if they are already friends
            if (await _friendRepo.ExistsAsync(request.UserId, request.FriendId))
            {

                return Conflict("This friendship already exists.");

            }

            // 3. Create the friendship entity
            var friendship = new Friendship(request.UserId, request.FriendId);

            // 4. Add to database
            await _friendRepo.AddAsync(friendship);

            // 5. Save changes
            if (await _friendRepo.SaveChangesAsync())
            {

                return Ok(new { message = "Friendship established successfully!" });

            }

            return StatusCode(500, "An error occurred while saving the friendship.");

        }

        /// <summary>
        /// Description: Adding a friend by receiving the friends username
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("add-by-username")]
        public async Task<IActionResult> AddFriendByUsername([FromBody] AddFriendByUsernameDto request)
        {

            // 1. We look at the friends information
            var friendUser = await _userRepo.GetByUsernameAsync(request.FriendUsername);

            if (friendUser == null)
            {
                return NotFound(new { message = $"User '{request.FriendUsername}' not found" });
            }

            // 2. Use the user ID we just foind for our logic checks
            if (request.UserId == friendUser.Id)
            {
                return BadRequest("You cannot add yourself as a friend.");
            }

            // 3. Check if they are already friends
            if (await _friendRepo.ExistsAsync(request.UserId, friendUser.Id))
            {

                return Conflict("This friendship already exists.");

            }

            // 4. Create the friendship entity
            var friendship = new Friendship(request.UserId, friendUser.Id);

            // 5. Add to database
            await _friendRepo.AddAsync(friendship);

            //6. Save changes
            if (await _friendRepo.SaveChangesAsync())
            {

                return Ok(new { message = $"Friendship established successfully! We added  '{friendUser.Username}'" });

            }

            return StatusCode(500, "An error occurred while saving the friendship.");

        }

        /// <summary>
        /// GetFriendsList
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("list/{userId}")]
        public async Task<IActionResult> GetFriendsList(Guid userId) 
        {
            // 1. Validation: Check if teh GUID is empty
            if (userId == Guid.Empty) 
            { 
                return BadRequest("A valid user ID is required.");
            }

            // 2. Fetch the mapped DTO list from the Repo
            var friends = await _friendRepo.GetFriendsByUserIdAsync(userId);

            // 3. Return the results
            // Even if the list is empty [], we return 200 OK because
            // "having no friends" isn;t a server error (it just happens some times 😅)
            return Ok(friends);

        }

        /// <summary>
        /// Remove Friend By Username
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete("remove-friend-by-username")]
        public async Task<IActionResult> RemoveFriendByUsername([FromBody] RemoveFriendByUsernameDto request)
        {
            // 1. Find the friend's ID
            var friendUser = await _userRepo.GetByUsernameAsync(request.FriendUsername);

            if (friendUser == null)
            {
                return NotFound(new { message = $"User '{request.FriendUsername}' not found." });
            }

            //2.  We validate that the friendship exists before we try to delete it.
            bool exists = await _friendRepo.ExistsAsync(request.UserId, friendUser.Id);

            if (!exists)
            {
                return NotFound(new { message = $"You are currently not friends with '{request.FriendUsername}'." });
            }

            // 3. Remove th friendship
            await _friendRepo.DeleteAsync(request.UserId, friendUser.Id);

            // 4. Save changes
            if (await _friendRepo.SaveChangesAsync())
            {
                return Ok(new { message = $"Successfully removed '{request.FriendUsername}' from your list." });
            }

            return StatusCode(500, "An error occurred while removing the friend.");

        }


        [HttpGet("check-friendship-by-username/{userId}/{friendUsername}")]
        public async Task<IActionResult> GCheckFriendshipByUsername(Guid userId, String friendUsername)
        {
            // 1. Find the friend by username
            var friendUser = await _userRepo.GetByUsernameAsync(friendUsername);

            if (friendUser == null)
            {
                return NotFound(new { message = "User not found.", isFriend = false});
            }


            //2.  We validate that the friendship exists before we try to delete it.
            bool isFriend = await _friendRepo.ExistsAsync(userId, friendUser.Id);


            // 3. Return the boolean status
            return Ok(new {
            
                isFriend,
                friendId = friendUser.Id,
                username = friendUser.Username

            });

        }


    }
}
