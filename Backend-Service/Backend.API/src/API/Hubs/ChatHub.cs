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
using Backend.API.src.Core.Logging;
using Backend.API.src.Infrastructure.Persistence.Repositories;
using Backend.API.src.Infrastructure.Persistence.Repositories.TestRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
namespace Backend.API.src.API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly MessageRepository _messageRepository;
        private readonly ChatEventRepository _chatEventRepository;
        private readonly TestChatRoomRepository _testChatRoomRepository;

        public ChatHub(MessageRepository messageRepositoy, ChatEventRepository chatEventRepository, TestChatRoomRepository testChatRoomRepository) 
        { 
            _messageRepository = messageRepositoy;
            _chatEventRepository = chatEventRepository;
            _testChatRoomRepository = testChatRoomRepository;
        }

        // -------------------------------------
        // ******USER INFO HELPER METHODS*******
        // -------------------------------------

        // By default, SignalR automatically maps the ClaimTypes.NameIdentifier claim to Context.UserIdentifier
        private Guid GetUserId()
        {
            return Guid.TryParse(Context.UserIdentifier, out var userId) ? userId : Guid.Empty;
        }

        private string GetUsername()
        {
            return Context.User?.Identity?.Name ?? "UnknownUser";
        }

        // -------------------------------------
        // ***MESSAGE SENDING HELPER METHODS****
        // -------------------------------------

        private async Task SendMessageToGroupAsync(TestMessage testMessage)
        {
            string group = testMessage.Message.ChatRoomId.ToString();
            await Clients.Group(group).SendAsync("ReceiveMessage", testMessage);
        }

        // -------------------------------------
        // ****DIRECT MESSAGE HELPER METHODS****
        // -------------------------------------

        //private async Task SendMessageToUserAsync(Guid userId, TestAcknowledgeDirectMessage testAcknowledgeDirectMessage)
        //{
        //    await Clients.User(userId.ToString()).SendAsync("AcknowledgeDirectMessage", testAcknowledgeDirectMessage);
        //}

        // -------------------------------------
        // ****ERROR SENDING HELPER METHODS*****
        // -------------------------------------

        private async Task SendErrorToClientAsync(string errorMessage)
        {
            await Clients.Caller.SendAsync("ReceiveError", errorMessage);
        }

        // -------------------------------------
        // **CHAT EVENT SENDING HELPER METHODS**
        // -------------------------------------

        private async Task SendEventToAllAsync(ChatEvent chatEvent)
        {
            await Clients.All.SendAsync("ReceiveEvent", chatEvent);
        }

        private async Task SendEventToGroupAsync(TestChatEvent testChatEvent)
        {
            string group = testChatEvent.ChatEvent.ChatRoomId.ToString();
            await Clients.Group(group).SendAsync("ReceiveEvent", testChatEvent);
        }

        private async Task SendEventToCallerUserAsync(TestChatEvent testChatEvent)
        {
            await Clients.User(GetUserId().ToString()).SendAsync("ReceiveEvent", testChatEvent);
        }

        // -------------------------------------
        // *************HUB METHODS*************
        // -------------------------------------

        public override async Task OnConnectedAsync()
        {
            try
            { 
                var chatEvent = new ChatEvent
                {
                    EventType = ChatEventType.UserJoined,
                    Details = $"Username: {GetUsername()}"
                };

                await _chatEventRepository.AddAsync(chatEvent);

                await SendEventToAllAsync(chatEvent);

                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                await SendErrorToClientAsync($"Internal Error: {ex.Message}");
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try 
            {
                var chatEvent = new ChatEvent
                {
                    EventType = ChatEventType.UserLeft,
                    Details = $"Username: {GetUsername()}"
                };

                await _chatEventRepository.AddAsync(chatEvent);

                await SendEventToAllAsync(chatEvent);

                await base.OnDisconnectedAsync(exception);
            }
            catch (Exception ex)
            {
                await SendErrorToClientAsync($"Internal Error: {ex.Message}");
            }
        }

        public async Task JoinChatRoom(ChatRoom ChatRoom)
        {
            try
            {
                Guid guid;
                string chatRoomName;

                if (ChatRoom.ChatRoomId != default && _testChatRoomRepository.ChatRoomExists(ChatRoom.ChatRoomId))
                {
                    guid = ChatRoom.ChatRoomId;
                    chatRoomName = _testChatRoomRepository.GetChatRoomName(ChatRoom.ChatRoomId) ?? "UnnamedChatRoom";
                }
                else
                {
                    await SendErrorToClientAsync("Chat room does not exist. Please provide a valid ChatRoomId.");
                    return;
                }
                
                var chatEvent = new ChatEvent
                {
                    EventType = ChatEventType.UserJoined,
                    ChatRoomId = guid,
                    Details = $"ChatRoom: {chatRoomName}, Username: {GetUsername()}"
                };

                var testChatEvent = new TestChatEvent
                {
                    ChatEvent = chatEvent,
                    ChatRoomName = chatRoomName
                };

                await _chatEventRepository.AddAsync(chatEvent);

                await Groups.AddToGroupAsync(Context.ConnectionId, guid.ToString());

                await SendEventToGroupAsync(testChatEvent);
            }
            catch (Exception ex)
            {
                await SendErrorToClientAsync($"Internal Error: {ex.Message}");
            }
        }

        public async Task LeaveChatRoom(ChatRoom ChatRoom)
        {
            try
            { 
                if (ChatRoom.ChatRoomId == default || !_testChatRoomRepository.ChatRoomExists(ChatRoom.ChatRoomId))
                {
                    await SendErrorToClientAsync("Chat room does not exist.");
                    return;
                }

                string chatRoomName = _testChatRoomRepository.GetChatRoomName(ChatRoom.ChatRoomId) ?? "UnnamedChatRoom";

                var chatEvent = new ChatEvent
                {
                    EventType = ChatEventType.UserLeft,
                    ChatRoomId = ChatRoom.ChatRoomId,
                    Details = $"ChatRoom: {chatRoomName}, Username: {GetUsername()}"
                };

                var testChatEvent = new TestChatEvent
                {
                    ChatEvent = chatEvent,
                    ChatRoomName = chatRoomName
                };

                await _chatEventRepository.AddAsync(chatEvent);

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, ChatRoom.ChatRoomId.ToString());

                await SendEventToCallerUserAsync(testChatEvent);

                await SendEventToGroupAsync(testChatEvent);
            }
            catch (Exception ex)
            {
                await SendErrorToClientAsync($"Internal Error: {ex.Message}");
            }
        }

        public async Task SendMessageToChatRoom(TestSendMessageToChatRoom testSendMessageToChatRoom)
        {
            try
            {
                var message = new Message
                {
                    ChatRoomId = testSendMessageToChatRoom.SendMessageToChatRoom.ChatRoomId,
                    Content = testSendMessageToChatRoom.SendMessageToChatRoom.Content, 
                    SenderId = GetUserId()
                };

                await _messageRepository.AddAsync(message);

                // Find ChatRoom by ChatRoomId.

                var testMessage = new TestMessage
                {
                    Message = message,
                    ChatRoomName = _testChatRoomRepository.GetChatRoomName(message.ChatRoomId) ?? "UnknownChatRoom",
                    SenderUsername = GetUsername()
                };

                await SendMessageToGroupAsync(testMessage);
            }
            catch (Exception ex)
            {
                await SendErrorToClientAsync($"Internal Error: {ex.Message}");
            }
        }
    }
}
