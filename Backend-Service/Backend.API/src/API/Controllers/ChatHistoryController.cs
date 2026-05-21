// -------------------------------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: April 20 2026
//  Description: Controller for managing chat history, messages, and chat rooms
// -------------------------------------------------------------------

using Backend.API.src.Application.DTOs;
//using Backend.API.src.Core.Interface;
using Backend.API.src.Infrastructure.Persistence.Repositories;
//using Backend.API.src.Infrastructure.Persistence.Repositories.TestRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using System.Security.Claims;

namespace Backend.API.src.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatHistoryController : ControllerBase
    {
        private readonly ChatEventRepository _chatEventRepository;
        private readonly MessageRepository _messageRepository;
        //private readonly TestChatRoomRepository _testChatRoomRepository;
        //private readonly IChatGroupRepository _chatGroupRepository;

        public ChatHistoryController(
            ChatEventRepository chatEventRepository, 
            MessageRepository messageRepository 
            /*TestChatRoomRepository testChatRoomRepository*/
            /*IChatGroupRepository chatGroupRepository*/)
        {
            _chatEventRepository = chatEventRepository;
            _messageRepository = messageRepository;
            //_testChatRoomRepository = testChatRoomRepository;
             //_chatGroupRepository = chatGroupRepository;
        }

        //private Guid GetCurrentUserId()
        //{
        //    var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        //    return userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId) ? userId : Guid.Empty;
        //}

        //private string GetUsername()
        //{
        //    var usernameClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
        //    return usernameClaim != null ? usernameClaim.Value : string.Empty;
        //}

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var chatHistory = await _chatEventRepository.GetAllChatEventsAsync();
            return Ok(chatHistory.Select(ChatEventDto.FromEntity));
        }

        [HttpGet]
        [Route("room/{roomId}/messages")]
        public async Task<IActionResult> GetMessagesByRoomId(Guid roomId)
        {
            var messages = await _messageRepository.GetMessagesByRoomIdAsync(roomId);
            return Ok(messages.Select(MessageDto.FromEntity));
        }

        [HttpGet]
        [Route("room/{roomId}/events")]
        public async Task<IActionResult> GetEventsByRoomId(Guid roomId)
        {
            var events = await _chatEventRepository.GetEventsByRoomIdAsync(roomId);
            return Ok(events.Select(ChatEventDto.FromEntity));
        }

        [HttpGet]
        [Route("message/{id}")]
        public async Task<IActionResult> GetMessageById(string id)
        {
            var message = await _messageRepository.GetMessageByIdAsync(id);
            if (message == null)
            {
                return NotFound();
            }
            return Ok(MessageDto.FromEntity(message));
        }

        //[HttpGet]
        //[Route("rooms")]
        //public IActionResult GetAllChatRooms()
        //{
        //    var chatRooms = _chatGroupRepository.;

        //    return Ok(chatRooms);
        //}

        //[HttpGet]
        //[Route("user/mine/rooms")]
        //public IActionResult GetMyChatRooms()
        //{
        //    var userId = GetCurrentUserId();
        //    if (userId == Guid.Empty)
        //    {
        //        return Unauthorized();
        //    }

        //    var username = GetUsername();

        //    return Ok(_chatGroupRepository.GetGroupsForUserAsync(GetCurrentUserId()).Result);
        //}

        [HttpGet]
        [Route("messages")]
        public async Task<IActionResult> GetAllMessages()
        {
            var messages = await _messageRepository.GetAllMessagesAsync();
            return Ok(messages.Select(MessageDto.FromEntity));
        }
    }
}