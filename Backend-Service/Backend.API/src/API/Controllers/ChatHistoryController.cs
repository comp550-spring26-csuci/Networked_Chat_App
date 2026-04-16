using Backend.API.src.Infrastructure.Persistence.Repositories;
using Backend.API.src.Infrastructure.Persistence.Repositories.TestRepository;
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

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var chatHistory = await _chatEventRepository.GetAllChatEventsAsync();
            return Ok(chatHistory);
        }

        [HttpGet]
        [Route("room/{roomId}")]
        public async Task<IActionResult> GetMessagesByRoomId(Guid roomId)
        {
            var messages = await _messageRepository.GetMessagesByRoomIdAsync(roomId);
            return Ok(messages);
        }

        [HttpGet]
        [Route("{id}")]
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
        [Route("messages")]
        public async Task<IActionResult> GetAllMessages()
        {
            var messages = await _messageRepository.GetAllMessagesAsync();
            return Ok(messages);
        }
    }
}