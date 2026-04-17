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

        private async Task SendMessageToUserAsync(Guid userId, TestAcknowledgeDirectMessage testAcknowledgeDirectMessage)
        {
            await Clients.User(userId.ToString()).SendAsync("AcknowledgeDirectMessage", testAcknowledgeDirectMessage);
        }

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

        // -------------------------------------
        // *************HUB METHODS*************
        // -------------------------------------

        public override async Task OnConnectedAsync()
        {
            try
            { 
                var chatEvent = new ChatEvent
                {
                    EventType = "NewConnection",
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
                    EventType = "RemovedConnection",
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

        // Note: In a real application, you would likely want to check if the user is already in the chat room
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
                    guid = _testChatRoomRepository.AddChatRoom(ChatRoom.ChatRoomName ?? "UnnamedChatRoom");
                    chatRoomName = ChatRoom.ChatRoomName ?? "UnnamedChatRoom";
                }
                
                await Groups.AddToGroupAsync(Context.ConnectionId, guid.ToString());

                var chatEvent = new ChatEvent
                {
                    EventType = "UserJoinedChatRoom",
                    ChatRoomId = guid,
                    Details = $"ChatRoom: {chatRoomName}"
                };

                await _chatEventRepository.AddAsync(chatEvent);

                var testChatEvent = new TestChatEvent
                {
                    ChatEvent = chatEvent,
                    ChatRoomName = ChatRoom.ChatRoomName
                };

                await SendEventToGroupAsync(testChatEvent);
            }
            catch (Exception ex)
            {
                await SendErrorToClientAsync($"Internal Error: {ex.Message}");
            }
        }

        // Note: In a real application, you would likely want to check if the user is actually in the chat room
        // before allowing them to leave it, and handle cases where they try to leave a room they're not in. For
        // simplicity, this example just attempts to remove them from the group and logs the event.
        public async Task LeaveChatRoom(ChatRoom ChatRoom)
        {
            try
            { 
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, _testChatRoomRepository.GetChatRoomName(ChatRoom.ChatRoomId) ?? "UnnamedChatRoom");
                var chatEvent = new ChatEvent
                {
                    EventType = "UserLeftChatRoom",
                    ChatRoomId = ChatRoom.ChatRoomId,
                    Details = $"ChatRoom: {_testChatRoomRepository.GetChatRoomName(ChatRoom.ChatRoomId) ?? "UnnamedChatRoom"}"
                };

                _testChatRoomRepository.RemoveChatRoom(ChatRoom.ChatRoomId);

                await _chatEventRepository.AddAsync(chatEvent);

                var testChatEvent = new TestChatEvent
                {
                    ChatEvent = chatEvent,
                    ChatRoomName = _testChatRoomRepository.GetChatRoomName(ChatRoom.ChatRoomId) ?? "UnnamedChatRoom"
                };

                await SendEventToGroupAsync(testChatEvent);
            }
            catch (Exception ex)
            {
                await SendErrorToClientAsync($"Internal Error: {ex.Message}");
            }
        }

        // Parameters are sent from the client as a TestSendMessage DTO, which
        // contains both the message content and the chat room name.
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
                    Username = testSendMessageToChatRoom.Username
                };

                await SendMessageToGroupAsync(testMessage);
            }
            catch (Exception ex)
            {
                await SendErrorToClientAsync($"Internal Error: {ex.Message}");
            }
        }

        public async Task StartDirectMessage(StartDirectMessage startDirectMessage)
        {
            try
            {
                Guid guid = _testChatRoomRepository.AddChatRoom(startDirectMessage.ChatRoomName);

                await Groups.AddToGroupAsync(Context.ConnectionId, guid.ToString());

                var acknowledgeDirectMessage = new AcknowledgeDirectMessage
                {
                    SenderId = GetUserId(),
                    ChatRoomId = guid,
                };

                var testAcknowledgeDirectMessage = new TestAcknowledgeDirectMessage
                {
                    AcknowledgeDirectMessage = acknowledgeDirectMessage,
                    Username = Context.User?.Identity?.Name ?? "UnknownUser",
                    ChatRoomName = startDirectMessage.ChatRoomName
                };

                await SendMessageToUserAsync(startDirectMessage.OtherUserId, testAcknowledgeDirectMessage);
            }
            catch (Exception ex)
            {
                await SendErrorToClientAsync($"Internal Error: {ex.Message}");
            }
        }

    }
}
