// -------------------------------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: April 20 2026
//  Description: Controller for managing chat history, messages, and chat rooms
// -------------------------------------------------------------------

using Backend.API.src.Application.DTOs;
using Backend.API.src.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.src.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatHistoryController : ControllerBase
    {
        private readonly ChatEventRepository _chatEventRepository;
        private readonly MessageRepository _messageRepository;

        public ChatHistoryController(
            ChatEventRepository chatEventRepository, 
            MessageRepository messageRepository )
        {
            _chatEventRepository = chatEventRepository;
            _messageRepository = messageRepository;
        }

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

        [HttpGet]
        [Route("messages")]
        public async Task<IActionResult> GetAllMessages()
        {
            var messages = await _messageRepository.GetAllMessagesAsync();
            return Ok(messages.Select(MessageDto.FromEntity));
        }
    }
}