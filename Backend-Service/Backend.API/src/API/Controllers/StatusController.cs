// -------------------------------------------------------------------
//  Project: Network Chat App
//  Engineer: Ivana Bavin-Gomez-San Basilio
//  Date: March 10th 2026
//  Description: Controller for the User Status
// -------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Backend.API.src.Application.DTOs;
using Backend.API.src.Core.Interface;
using Backend.API.src.Core.Logging;
using System.Threading.Tasks;
using System;
using Backend.API.Migrations;
using Backend.API.src.Application.Services;

namespace Backend.API.src.API.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class StatusController : ControllerBase
    {

        // We inject the interface of the repository
        private readonly IUserRepository _userRepository;
        private readonly UserEventPublisher _userEventPublisher;

        public StatusController(IUserRepository userRepository, UserEventPublisher userEventPublisher)
        {
            _userRepository = userRepository;
            _userEventPublisher = userEventPublisher;
            AppLogger.DebugState("StatusController", "Controller Initialized");
        }


        /// <summary>
        /// Updates the user's presence status and custom text.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusDto request) 
        {
            AppLogger.DebugState("StatusController", $"UpdateStatus requested for UserId: {request.UserId}");

            if (!ModelState.IsValid) 
            {

                AppLogger.DebugState("StatusController", $"Invalid model state for UpdateStatus request from UserId: {request.UserId}");
                return BadRequest(ModelState);

            }

            // Using the Interface to avoid the "User" class naming conflict
            var user = await _userRepository.GetByIdAsync(request.UserId);

            if (user == null) 
            { 
                AppLogger.DebugState("StatusController", $"UpdateStatus failed: User {request.UserId} not found.");
                return NotFound("User not found.");


            }

            user.UpdatePresence(request.NewStatus, request.CustomText);

            _userRepository.Update(user);

            try
            {

                await _userRepository.SaveChangesAsync();

                // Publish the status change event to notify friends
                await _userEventPublisher.PublishUserStatusChangeAsync(request.UserId);

                AppLogger.UserAction(request.UserId.ToString(), $"Successfully updated presence status to {request.NewStatus}");
                return Ok(new { message = "Status correctly updated." });

            }
            catch (Exception ex)
            {
                AppLogger.ShieldFailure("StatusController_UpdateStatus", ex);
                return StatusCode(500, "Internal error when saving the status.");
            }
            

        }


        [HttpGet("{userId}")]
        public async Task<IActionResult> GetStatus(Guid userId)
        {

            AppLogger.DebugState("StatusController", $"GetStatus requested for userId: {userId}");

            var user = await _userRepository.GetByIdAsync(userId);


            if (user == null)
            {

                AppLogger.DebugState("StatusController", $"GetStatus failed:User {userId} not found.");
                return NotFound("User not found.");
            
            }

            AppLogger.DebugState("StatusController", $"Successfully retrieved status for userId: {userId}");

            return Ok(new
            {
            
                presenceStatus = (int)user.PresenceStatus,
                customStatusText = user.CustomStatusText,
                lastActive = user.LastActive

            });

        }

    }
}
