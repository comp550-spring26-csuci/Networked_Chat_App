// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: March 11 2026
//  Description: Defines the ChatHub class for real-time communication using SignalR.
//  This hub allows clients to send messages to all connected clients or to specific groups,
//  and manage group memberships.
// --------------------------------------------

using Backend.API.src.Application.DTOs;
using Backend.API.src.Application.DTOs.TestDTOs;
using Backend.API.src.Core.Entities;
using Backend.API.src.Core.Interface;
using Backend.API.src.Core.Logging;
using Backend.API.src.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver.Core.Servers;

namespace Backend.API.src.API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly MessageRepository _messageRepository;
        private readonly ChatEventRepository _chatEventRepository;

        public ChatHub(MessageRepository messageRepositoy, ChatEventRepository chatEventRepository) 
        { 
            _messageRepository = messageRepositoy;
            _chatEventRepository = chatEventRepository;
        }

        private string GetUsername()
        {
            return Context.User?.Identity?.Name ?? "UnknownUser";
        }

        // By default, SignalR automatically maps the ClaimTypes.NameIdentifier claim to Context.UserIdentifier
        private string GetUserId()
        {
            return Context.UserIdentifier ?? "UnknownUserId";
        }

        private async Task SendMessageToGroupAsync(TestMessage testMessage)
        {
            var userName = GetUsername();
            var userId = GetUserId();
            AppLogger.DebugState("ChatHub", $"Sending message to chat room. User: {userName} (ID: {userId}), ChatRoom: {testMessage.ChatRoomName}, Content: {testMessage.Message.Content}");
            await Clients.Group(testMessage.ChatRoomName).SendAsync("ReceiveMessage", testMessage.Message);
        }

        private async Task SendMessageToAllAsync(Message message)
        {
            var userName = GetUsername();
            var userId = GetUserId();
            AppLogger.DebugState("ChatHub", $"Sending message to all clients. User: {userName} (ID: {userId}), Content: {message.Content}");
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        private async Task SendMessageToCallerAsync(Message message)
        {
            var userName = GetUsername();
            var userId = GetUserId();
            AppLogger.DebugState("ChatHub", $"Sending message to client. User: {userName} (ID: {userId}), Content: {message.Content}");
            await Clients.Caller.SendAsync("ReceiveMessage", message);
        }

        private async Task SendErrorToClientAsync(string errorMessage)
        {
            var userName = GetUsername();
            var userId = GetUserId();
            AppLogger.DebugState("ChatHub", $"Sending error to client. User: {userName} (ID: {userId}), Error: {errorMessage}");
            await Clients.Caller.SendAsync("ReceiveError", errorMessage);
        }

        private async Task SendEventAsync(ChatEvent chatEvent)
        {
            var userName = GetUsername();
            var userId = GetUserId();
            AppLogger.DebugState("ChatHub", $"Sending event to client. User: {userName} (ID: {userId}), EventType: {chatEvent.EventType}, Details: {chatEvent.Details}");
            await Clients.Caller.SendAsync("ReceiveEvent", chatEvent);
        }

        private async Task SendEventToGroupAsync(ChatEvent chatEvent)
        {
            var userName = GetUsername();
            var userId = GetUserId();
            AppLogger.DebugState("ChatHub", $"Sending event to chat room. User: {userName} (ID: {userId}), ChatRoom: {chatEvent.ChatRoomId}, EventType: {chatEvent.EventType}, Details: {chatEvent.Details}");
            await Clients.Group(chatEvent.ChatRoomId.ToString()).SendAsync("ReceiveEvent", chatEvent);
        }

        public override async Task OnConnectedAsync()
        {
            AppLogger.ConnectionEvent(Context.ConnectionId, "Connected", GetUserId());
            try
            { 
                var chatEvent = new ChatEvent
                {
                    EventType = "NewConnection"
                };

                await _chatEventRepository.AddAsync(chatEvent);

                await SendEventAsync(chatEvent);

                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                await SendErrorToClientAsync($"Internal Error: {ex.Message}");
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            AppLogger.ConnectionEvent(Context.ConnectionId, "Disconnected", GetUserId());
            try 
            {
                var chatEvent = new ChatEvent
                {
                    EventType = "RemovedConnection",
                };

                await _chatEventRepository.AddAsync(chatEvent);

                await SendEventAsync(chatEvent);

                await base.OnDisconnectedAsync(exception);
            }
            catch (Exception ex)
            {
                await SendErrorToClientAsync($"Internal Error: {ex.Message}");
            }
        }

        public async Task JoinChatRoom(TestChatRoom testChatRoom)
        {
            AppLogger.DebugState("ChatHub", $"User joining chat room. User: {GetUsername()} (ID: {GetUserId()}), ChatRoom: {testChatRoom.ChatRoomId}");
            try
            { 
                var chatEvent = new ChatEvent
                {
                    EventType = "UserJoinedChatRoom",
                    ChatRoomId = testChatRoom.ChatRoomId,
                    Details = $"ChatRoom: {testChatRoom.ChatRoomId}"
                };

                await Groups.AddToGroupAsync(Context.ConnectionId, testChatRoom.ChatRoomName);

                await _chatEventRepository.AddAsync(chatEvent);

                await SendEventToGroupAsync(chatEvent);
            }
            catch (Exception ex)
            {
                await SendErrorToClientAsync($"Internal Error: {ex.Message}");
            }
        }

        public async Task LeaveChatRoom(TestChatRoom testChatRoom)
        {
            AppLogger.DebugState("ChatHub", $"User leaving chat room. User: {GetUsername()} (ID: {GetUserId()}), ChatRoom: {testChatRoom.ChatRoomId}");
            try
            { 
                var chatEvent = new ChatEvent
                {
                    EventType = "UserLeftChatRoom",
                    ChatRoomId = testChatRoom.ChatRoomId,
                    Details = $"ChatRoom: {testChatRoom.ChatRoomId}"
                };

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, testChatRoom.ChatRoomName);
                
                await _chatEventRepository.AddAsync(chatEvent);

                await SendEventToGroupAsync(chatEvent);
            }
            catch (Exception ex)
            {
                await SendErrorToClientAsync($"Internal Error: {ex.Message}");
            }
        }

        public async Task SendMessageToChatRoom(TestSendMessage testSendMessage)
        {
            try
            {
                var message = new Message
                {
                    ChatRoomId = testSendMessage.SendMessage?.ChatRoomId ?? 0,
                    SenderId = int.TryParse(Context.UserIdentifier, out var id) ? id : 0,
                    Content = testSendMessage.SendMessage?.Content ?? string.Empty,
                };

                await _messageRepository.AddAsync(message);

                // Find ChatRoom by ChatRoomId. If it exists, use ChatRoomId as the group name. If it doesn't exist, we can either create a new group
                // or return an error. For now, we'll just use the ChatRoomId as the group name and assume it exists.

                var testMessage = new TestMessage
                {
                    Message = message,
                    ChatRoomName = testSendMessage.ChatRoomName
                };

                await SendMessageToGroupAsync(testMessage);
            }
            catch (Exception ex)
            {
                await SendErrorToClientAsync($"Internal Error: {ex.Message}");
            }
        }

        public async Task SendMessageToAll(SendMessage sendMessage)
        {
            try
            {
                var message = new Message
                {
                    SenderId = int.TryParse(Context.UserIdentifier, out var id) ? id : 0,
                    Content = sendMessage.Content,
                };

                await _messageRepository.AddAsync(message);

                await SendMessageToAllAsync(message);
            }
            catch (Exception ex)
            {
                await SendErrorToClientAsync($"Internal Error: {ex.Message}");
            }
        }

        // This method sends a message back to the caller only, which can be useful for acknowledgments or private responses.
        public async Task SendMessageToCaller(SendMessage sendMessage)
        {
            try
            {
                var message = new Message
                {
                    SenderId = int.TryParse(Context.UserIdentifier, out var id) ? id : 0,
                    Content = sendMessage.Content
                };

                await _messageRepository.AddAsync(message);

                await SendMessageToCallerAsync(message);
            }
            catch (Exception ex)
            {
                await SendErrorToClientAsync($"Internal Error: {ex.Message}");
            }
        }
    }
}
