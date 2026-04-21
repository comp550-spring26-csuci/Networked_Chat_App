using Backend.API.src.Infrastructure.Persistence.Repositories;
using Backend.API.src.Infrastructure.Persistence.Repositories.TestRepository;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.src.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatHistoryController : ControllerBase
    {
        private readonly ChatEventRepository _chatEventRepository;
        private readonly MessageRepository _messageRepository;
        private readonly TestChatRoomRepository _testChatRoomRepository;

        public ChatHistoryController(ChatEventRepository chatEventRepository, MessageRepository messageRepository, TestChatRoomRepository testChatRoomRepository)
        {
            _chatEventRepository = chatEventRepository;
            _messageRepository = messageRepository;
            _testChatRoomRepository = testChatRoomRepository;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId) ? userId : Guid.Empty;
        }

        private string GetUsername()
        {
            var usernameClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            return usernameClaim != null ? usernameClaim.Value : string.Empty;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var chatHistory = await _chatEventRepository.GetAllChatEventsAsync();
            return Ok(chatHistory);
        }

        [HttpGet]
        [Route("room/{roomId}/messages")]
        public async Task<IActionResult> GetMessagesByRoomId(Guid roomId)
        {
            var messages = await _messageRepository.GetMessagesByRoomIdAsync(roomId);
            return Ok(messages);
        }

        [HttpGet]
        [Route("room/{roomId}/events")]
        public async Task<IActionResult> GetEventsByRoomId(Guid roomId)
        {
            var events = await _chatEventRepository.GetEventsByRoomIdAsync(roomId);
            return Ok(events);
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
            return Ok(message);
        }

        [HttpGet]
        [Route("rooms")]
        public IActionResult GetAllChatRooms()
        {
            var chatRooms = _testChatRoomRepository.GetAllChatRooms();

            return Ok(chatRooms);
        }

        [HttpGet]
        [Route("user/mine/rooms")]
        public IActionResult GetMyChatRooms()
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var username = GetUsername();

            return Ok(_testChatRoomRepository.GetChatRoomsByUsername(username));
        }

        [HttpGet]
        [Route("user/mine/id")]
        public IActionResult GetMyUserId()
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            return Ok(userId);
        }

        [HttpGet]
        [Route("messages")]
        public async Task<IActionResult> GetAllMessages()
        {
            var messages = await _messageRepository.GetAllMessagesAsync();
            return Ok(messages);
        }
    }
}