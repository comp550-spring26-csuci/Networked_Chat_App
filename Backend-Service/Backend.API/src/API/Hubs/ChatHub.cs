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

        private async Task SendMessageToGroupAsync(TestMessage testMessage, TestMessagePreview testMessagePreview)
        {
            string group = testMessage.Message.ChatRoomId.ToString();
            await Clients.Group(group).SendAsync("ReceiveMessage", testMessage);
            await Clients.Group("global_" + group).SendAsync("ReceiveMessagePreview", testMessagePreview);
        }

        private TestMessage ConstructMessageDto(SendMessageToChatRoom sendMessageToChatRoom)
        {
            var message = new Message
            {
                ChatRoomId = sendMessageToChatRoom.ChatRoomId,
                Content = sendMessageToChatRoom.Content,
                SenderId = GetUserId(),
                SenderUsername = GetUsername()
            };

            var testMessage = new TestMessage
            {
                Message = message,
                ChatRoomName = _testChatRoomRepository.GetChatRoomName(message.ChatRoomId) ?? "UnknownChatRoom",
                SenderUsername = GetUsername()
            };

            return testMessage;
        }

        private TestMessagePreview ConstructPreviewDto(Message message)
        {
            string preview = message.Content.Length > 50 ? message.Content.Substring(0, 50) + "..." : message.Content;

            var messagePreview = new MessagePreview
            {
                ChatRoomId = message.ChatRoomId,
                Content = preview,
                SenderUsername = message.SenderUsername
            };

            var testMessagePreview = new TestMessagePreview
            {
                MessagePreview = messagePreview,
                ChatRoomName = _testChatRoomRepository.GetChatRoomName(message.ChatRoomId) ?? "UnknownChatRoom",
                SenderId = message.SenderId
            };

            return testMessagePreview;
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

        private async Task SendEventToAllAsync(TestChatEvent testChatEvent)
        {
            await Clients.All.SendAsync("ReceiveEvent", testChatEvent);
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

        private TestChatEvent ConstructChatEventDto(ChatEventType eventType, string chatRoomName, Guid chatRoomId = default)
        {
            var chatEvent = new ChatEvent
            {
                EventType = eventType,
                ChatRoomId = chatRoomId,
                Details = $"ChatRoom: {chatRoomName}, Username: {GetUsername()}"
            };

            var testChatEvent = new TestChatEvent
            {
                ChatEvent = chatEvent,
                ChatRoomName = chatRoomName
            };

            return testChatEvent;
        }

        // -------------------------------------
        // *************HUB METHODS*************
        // -------------------------------------

        public override async Task OnConnectedAsync()
        {
            AppLogger.ConnectionEvent(Context.ConnectionId, "Connected", GetUserId().ToString());
            try
            {
                TestChatEvent testChatEvent = ConstructChatEventDto(ChatEventType.UserJoined, "Global");

                await _chatEventRepository.AddAsync(testChatEvent.ChatEvent);

                await SendEventToAllAsync(testChatEvent);

                await base.OnConnectedAsync();

                AppLogger.DebugState("ChatHub", $"User connection initialization completed for Connection ID: {Context.ConnectionId}, User ID: {GetUserId()}, Username: {GetUsername()}");
            }
            catch (Exception ex)
            {
                AppLogger.ShieldFailure("ChatHub", ex);
                await SendErrorToClientAsync($"Internal Error: {ex.Message}");
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            AppLogger.ConnectionEvent(Context.ConnectionId, "Disconnected", GetUserId().ToString());
            try
            {
                TestChatEvent testChatEvent = ConstructChatEventDto(ChatEventType.UserLeft, "Global");

                await _chatEventRepository.AddAsync(testChatEvent.ChatEvent);

                await SendEventToAllAsync(testChatEvent);

                await base.OnDisconnectedAsync(exception);

                AppLogger.DebugState("ChatHub", $"User disconnection handling completed for Connection ID: {Context.ConnectionId}, User ID: {GetUserId()}, Username: {GetUsername()}");
            }
            catch (Exception ex)
            {
                AppLogger.ShieldFailure("ChatHub", ex);
                await SendErrorToClientAsync($"Internal Error: {ex.Message}");
            }
        }

        public async Task JoinChatRoom(ChatRoom ChatRoom)
        {
            AppLogger.DebugState("ChatHub.JoinChatRoom", $"User with Connection ID: {Context.ConnectionId}, User ID: {GetUserId()}, Username: {GetUsername()} is attempting to join Chat Room ID: {ChatRoom.ChatRoomId}");
            try
            {
                Guid guid = ChatRoom.ChatRoomId;
                string chatRoomName = _testChatRoomRepository.GetChatRoomName(guid) ?? "UnnamedChatRoom";

                if (guid == default || !_testChatRoomRepository.ChatRoomExists(guid))
                {
                    await SendErrorToClientAsync("Chat room does not exist. Please provide a valid ChatRoomId.");
                    return;
                }

                TestChatEvent testChatEvent = ConstructChatEventDto(ChatEventType.UserJoined, chatRoomName, guid);

                await _chatEventRepository.AddAsync(testChatEvent.ChatEvent);

                await Groups.AddToGroupAsync(Context.ConnectionId, guid.ToString());
                await Groups.AddToGroupAsync(Context.ConnectionId, "global_" + guid.ToString());

                await SendEventToGroupAsync(testChatEvent);

                AppLogger.DebugState("ChatHub.JoinChatRoom", $"User successfully joined Chat Room ID: {ChatRoom.ChatRoomId}. Event broadcasted to group.");
            }
            catch (Exception ex)
            {
                AppLogger.ShieldFailure("ChatHub.JoinChatRoom", ex);
                await SendErrorToClientAsync($"Internal Error: {ex.Message}");
            }
        }

        public async Task LeaveChatRoom(ChatRoom ChatRoom)
        {
            AppLogger.DebugState("ChatHub.LeaveChatRoom", $"User with Connection ID: {Context.ConnectionId}, User ID: {GetUserId()}, Username: {GetUsername()} is attempting to leave Chat Room ID: {ChatRoom.ChatRoomId}");
            try
            {
                Guid guid = ChatRoom.ChatRoomId;

                string chatRoomName = _testChatRoomRepository.GetChatRoomName(guid) ?? "UnnamedChatRoom";

                if (guid == default || !_testChatRoomRepository.ChatRoomExists(guid))
                {
                    await SendErrorToClientAsync("Chat room does not exist.");
                    return;
                }

                TestChatEvent testChatEvent = ConstructChatEventDto(ChatEventType.UserLeft, chatRoomName, guid);

                await _chatEventRepository.AddAsync(testChatEvent.ChatEvent);

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, guid.ToString());

                await SendEventToCallerUserAsync(testChatEvent);

                await SendEventToGroupAsync(testChatEvent);

                AppLogger.DebugState("ChatHub.LeaveChatRoom", $"User successfully left Chat Room ID: {ChatRoom.ChatRoomId}. Event broadcasted to group and caller.");
            }
            catch (Exception ex)
            {
                AppLogger.ShieldFailure("ChatHub.LeaveChatRoom", ex);
                await SendErrorToClientAsync($"Internal Error: {ex.Message}");
            }
        }

        public async Task SendMessageToChatRoom(TestSendMessageToChatRoom testSendMessageToChatRoom)
        {
            AppLogger.DebugState("ChatHub.SendMessageToChatRoom", $"User with Connection ID: {Context.ConnectionId}, User ID: {GetUserId()}, Username: {GetUsername()} is attempting to send a message to Chat Room ID: {testSendMessageToChatRoom.SendMessageToChatRoom.ChatRoomId}");
            try
            {
                TestMessage testMessage = ConstructMessageDto(testSendMessageToChatRoom.SendMessageToChatRoom);
                
                TestMessagePreview testMessagePreview = ConstructPreviewDto(testMessage.Message);

                await _messageRepository.AddAsync(testMessage.Message);

                await SendMessageToGroupAsync(testMessage, testMessagePreview);

                AppLogger.DebugState("ChatHub.SendMessageToChatRoom", $"Message successfully sent to Chat Room ID: {testMessage.Message.ChatRoomId} and stored in database.");
            }
            catch (Exception ex)
            {
                AppLogger.ShieldFailure("ChatHub.SendMessageToChatRoom", ex);
                await SendErrorToClientAsync($"Internal Error: {ex.Message}");
            }
        }
    }
}
