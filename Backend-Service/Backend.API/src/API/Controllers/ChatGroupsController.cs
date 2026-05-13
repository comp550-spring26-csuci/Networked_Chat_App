// ---------------------------------------------------------------------------
//  Project: Network Chat App
//  Engineer: Ivana Bavin-Gomez-San Basilio
//  Date: May 2nd 2026
//  Description: Controller for managing persistent Chat Groups.
// ---------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Backend.API.src.Application.DTOs;
using Backend.API.src.Core.Interface;
using Backend.API.src.Core.Entities;
using Backend.API.src.Core.Logging;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Security.Cryptography.X509Certificates;


namespace Backend.API.src.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatGroupsController : ControllerBase
    {
        private readonly IChatGroupRepository _chatGroupRepository;
        private readonly IFriendshipRepository _friendshipRepository;

        // We inject both repositories so we can verify friendships before adding to a group!
        public ChatGroupsController(IChatGroupRepository chatGroupRepository,IFriendshipRepository friendshipRepository)
        {
            _chatGroupRepository = chatGroupRepository;
            _friendshipRepository = friendshipRepository;
            AppLogger.DebugState("ChatGroupsController", "Controller Initialized");
        }
    


        // POST api/ChatGroup/create
        [HttpPost("create")]
        public async Task<IActionResult> CreateGroup([FromBody] CreateChatGroupDto request)
        {

            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                // 1. Create the Group
                var newGroup = new ChatGroup(request.GroupName, request.CreatorUserId);
                await _chatGroupRepository.CreateGroupAsync(newGroup);

                // 2. Add the Creator as the first member automatically
                var firstMember = new ChatGroupMember(newGroup.Id, request.CreatorUserId);
                await _chatGroupRepository.AddMemberAsync(firstMember);

                await _chatGroupRepository.SaveChangesAsync();

                AppLogger.UserAction(request.CreatorUserId.ToString(), $"Created chat group: {newGroup.Name}");
                return Ok(new { message = "Group created successfully", groupId = newGroup.Id });


            }
            catch (Exception ex)
            {

                AppLogger.ShieldFailure("ChatGroupController_CreateGroup", ex);
                return StatusCode(500, "Internal error when creating the group.");
            
            }
        }


        // ====================================================================
        // POST api/ChatGroups/add-member
        // ====================================================================
        [HttpPost("add-member")]
        public async Task<IActionResult> AddMember([FromBody] AddGroupMemberDto request)
        {

            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                // Guard Rail 1: Does the group exist?
                var group = await _chatGroupRepository.GetGroupByIdAsync(request.ChatGroupId);
                if (group == null) return NotFound("Chat group not found.");

                // Guard Rail 2: Is the requester actually in the group?
                bool isRequesterInGroup = await _chatGroupRepository.IsUserInGroupAsync(request.ChatGroupId, request.RequesterId);
                if (!isRequesterInGroup) return Unauthorized("You must be in the group to add someone.");


                // Guard Rail 3: Are they friends?
                // This checks your existing friendship logic to ensure they are connected
                bool areFriends = await _friendshipRepository.ExistsAsync(request.RequesterId, request.TargetUserId);

                if (!areFriends) return BadRequest("You can only add friends to a chat group.");

                // Guard Rail 4: Is the target already in the group?
                bool isTargetAlreadyInGroup = await _chatGroupRepository.IsUserInGroupAsync(request.ChatGroupId, request.TargetUserId);
                if (isTargetAlreadyInGroup) return BadRequest("User is already in this group");

                // Add the user
                var newMember = new ChatGroupMember(request.ChatGroupId, request.TargetUserId);
                await _chatGroupRepository.AddMemberAsync(newMember);
                await _chatGroupRepository.SaveChangesAsync();

                AppLogger.UserAction(request.RequesterId.ToString(), $"Added user {request.TargetUserId} to group {group.Name}");
                return Ok(new { message = "Friend added to group successfully."});

            }
            catch (Exception ex)
            {
                AppLogger.ShieldFailure("ChatGroupsController_AddMember", ex);
                return StatusCode(500, "Internal error when adding member.");
            
            }

        }


        // ====================================================================
        // GET api/ChatGroups/user/{userId}
        // ====================================================================
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetMyGroups(Guid userId)
        {

            try
            {
                var groups = await _chatGroupRepository.GetGroupsForUserAsync(userId);

                // We shape the data safely before sending it to the frontend
                var result = groups.Select(g => new
                {
                    id = g.Id,
                    name = g.Name,
                    createdAt = g.CreatedAt,
                    createdByUserId = g.CreatedByUserId

                });


                return Ok(result);

            }
            catch (Exception ex)
            {
                AppLogger.ShieldFailure("ChatGroupsController_GetMyGroups", ex);
                return StatusCode(500, "Internal error retrieving groups.");


            }

        
        }


        // ====================================================================
        // GET api/ChatGroups/{groupId}/get-group-chat-members
        // Retrieves all users within a specific chat group
        // ====================================================================
        [HttpGet("{groupId}/get-group-chat-members")]
        public async Task<IActionResult> GetGroupMembers(Guid groupId)
        {
            try
            {
                var members = await _chatGroupRepository.GetGroupMembersAsync(groupId);

                if (members == null || !members.Any())
                {
                    return NotFound(new { Message = "No members found or group does not exist." });
                }

                // Map to a clean response object to avoid circular JSON loops
                var result = members.Select(m => new
                {
                    userId = m.UserId,
                    username = m.User?.Username ?? "Unknown User",
                    joinedAt = m.JoinedAt
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Assuming you have your logger injected or available statically!
                AppLogger.ShieldFailure("ChatGroupsController_GetGroupMembers", ex);
                return StatusCode(500, new { Message = "Internal error retrieving group members." });
            }
        }




        // ====================================================================
        // DELETE api/ChatGroups/leave-group/{groupId}/{userId}
        // ====================================================================
        [HttpDelete("leave-group/{groupId}/{userId}")]
        public async Task<IActionResult> LeaveGroup(Guid groupId, Guid userId)
        {

            try
            {
                await _chatGroupRepository.RemoveMemberAsync(groupId, userId);
                await _chatGroupRepository.SaveChangesAsync();

                AppLogger.UserAction(userId.ToString(), $"Left chat group {groupId}");
                return Ok(new { message = "You have left the group." }); 

            }
            catch (Exception ex)
            {
                AppLogger.ShieldFailure("ChatGroupsController_LeaveGroup", ex);
                return StatusCode(500, "Internal error when leaving the group.");
            
            }
        
        }



        // ====================================================================
        // DELETE api/ChatGroups/delete-group/{groupId}/{requesterId}
        // ====================================================================
        [HttpDelete("delete-group/{groupId}/{requesterId}")]
        public async Task<IActionResult> DeleteGroup(Guid groupId, Guid requesterId)
        {

            try
            {

                var group = await _chatGroupRepository.GetGroupByIdAsync(groupId);
                if (group == null) return NotFound("Chat group not found.");

                // Guard Rail: Only the creator can delete the group!
                if (group.CreatedByUserId != requesterId)
                {
                    return Unauthorized("Only the creator of the group can delete it.");               
                }

                await _chatGroupRepository.DeleteGroupAsync(group);
                await _chatGroupRepository.SaveChangesAsync();

                AppLogger.UserAction(requesterId.ToString(), $"Deleted chat group {group.Name}");
                return Ok(new { message = "Group deleted successfully." });

            }
            catch (Exception ex)
            {
                AppLogger.ShieldFailure("ChatGroupsController_DeleteGroup", ex);
                return StatusCode(500, "Internal error when deleting the group.");

            }

        }

    }
}
