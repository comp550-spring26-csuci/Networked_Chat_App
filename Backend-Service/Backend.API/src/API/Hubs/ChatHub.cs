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
using Backend.API.src.Application.Services;
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
        private readonly SignalRGroupService _signalRGroupService;
        private readonly ClientPresenceService _clientPresenceService;

        public ChatHub(MessageRepository messageRepositoy, ChatEventRepository chatEventRepository, TestChatRoomRepository testChatRoomRepository, SignalRGroupService signalRGroupService, ClientPresenceService clientPresenceService)
        {
            _messageRepository = messageRepositoy;
            _chatEventRepository = chatEventRepository;
            _testChatRoomRepository = testChatRoomRepository;
            _signalRGroupService = signalRGroupService;
            _clientPresenceService = clientPresenceService;
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

        private TestMessage ConstructMessageDto(SendMessageToChatRoom sendMessageToChatRoom, string? chatRoomName, string? senderUsername)
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
                ChatRoomName = chatRoomName,
                SenderUsername = senderUsername
            };

            return testMessage;
        }

        private static TestMessagePreview ConstructPreviewDto(Message message, string? chatRoomName, Guid? senderId)
        {
            string preview = message.Content.Length > 50 ? string.Concat(message.Content.AsSpan(0, 50), "...") : message.Content;

            var messagePreview = new MessagePreview
            {
                MessageId = message.Id,
                ChatRoomId = message.ChatRoomId,
                Content = preview,
                SenderUsername = message.SenderUsername,
                Timestamp = message.Timestamp
            };

            var testMessagePreview = new TestMessagePreview
            {
                MessagePreview = messagePreview,
                ChatRoomName = chatRoomName,
                SenderId = senderId // Null if the sender chose not to include their user ID with their message
            };

            return testMessagePreview;
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

        private async Task SendEventToAllAsync(TestChatEvent testChatEvent)
        {
            await Clients.All.SendAsync("ReceiveEvent", testChatEvent);
        }

        private async Task SendEventToGroupAsync(TestChatEvent testChatEvent)
        {
            string activeGroup = SignalRGroupService.GetActiveGroupId(testChatEvent.ChatEvent.ChatRoomId);
            await Clients.Group(activeGroup).SendAsync("ReceiveEvent", testChatEvent);
        }

        private async Task SendEventToCallerUserAsync(TestChatEvent testChatEvent)
        {
            await Clients.User(GetUserId().ToString()).SendAsync("ReceiveEvent", testChatEvent);
        }

        private TestChatEvent ConstructChatEventDto(ChatEventType eventType, string? chatRoomName, Guid chatRoomId = default)
        {
            var chatEvent = new ChatEvent
            {
                EventType = eventType,
                ChatRoomId = chatRoomId,
                Details = $"Username: {GetUsername()}"
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
                TestChatEvent testChatEvent = ConstructChatEventDto(ChatEventType.UserJoined, null);

                await _chatEventRepository.AddAsync(testChatEvent.ChatEvent);

                await SendEventToAllAsync(testChatEvent);

                bool firstConnection = await _clientPresenceService.UserSessionStarted(GetUserId(), Context.ConnectionId);

                // Chat rooms are supposed to be accessible by user ID, but since my implementation is based on username,
                // I have to get the chat rooms by username instead. This is a temporary workaround until we implement proper
                // user-based chat room access.
                List<Guid> myChatRooms = _testChatRoomRepository.GetMyChatRoomIds(GetUsername()); 

                await _signalRGroupService.SyncConnectionGroupsAsync(Context.ConnectionId, myChatRooms);

                if (firstConnection)
                {
                    AppLogger.UserAction(GetUserId().ToString(), "User session started");
                    // Update user's online status in the database to true
                    // Let others (Friends/Everyone) know that the user is now
                    // online (could be a UserStatusChanged event with a status of "Online")
                }

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
                TestChatEvent testChatEvent = ConstructChatEventDto(ChatEventType.UserLeft, null);

                await _chatEventRepository.AddAsync(testChatEvent.ChatEvent);

                await SendEventToAllAsync(testChatEvent);

                bool isLastConnection = await _clientPresenceService.UserSessionEnded(GetUserId(), Context.ConnectionId);

                if (isLastConnection) 
                {
                    AppLogger.UserAction(GetUserId().ToString(), "User session ended");
                    // Update user's online status in the database to false
                    // Let others (Friends/Everyone) know that the user is now offline (could be a UserStatusChanged event with a status of "Offline")
                }

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
                Guid roomId = ChatRoom.ChatRoomId;

                if (roomId == default || !_testChatRoomRepository.ChatRoomExists(roomId))
                {
                    await SendErrorToClientAsync("Chat room does not exist. Please provide a valid ChatRoomId.");
                    return;
                }

                TestChatEvent testChatEvent = ConstructChatEventDto(ChatEventType.UserJoined, ChatRoom.ChatRoomName, roomId);

                await _chatEventRepository.AddAsync(testChatEvent.ChatEvent);

                await _signalRGroupService.JoinChatRoomGroupsAsync(Context.ConnectionId, roomId);
                await SendEventToGroupAsync(testChatEvent);

                AppLogger.DebugState("ChatHub.JoinChatRoom", $"User successfully joined Chat Room ID: {roomId}. Event broadcasted to group.");
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
                Guid roomId = ChatRoom.ChatRoomId;

                TestChatEvent testChatEvent = ConstructChatEventDto(ChatEventType.UserLeft, ChatRoom.ChatRoomName, roomId);

                await _chatEventRepository.AddAsync(testChatEvent.ChatEvent);

                await _signalRGroupService.LeaveChatRoomGroupAsync(Context.ConnectionId, roomId);
                await SendEventToCallerUserAsync(testChatEvent);
                await SendEventToGroupAsync(testChatEvent);

                AppLogger.DebugState("ChatHub.LeaveChatRoom", $"User successfully left Chat Room ID: {roomId}. Event broadcasted to group and caller.");
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
                TestMessage testMessage = ConstructMessageDto(testSendMessageToChatRoom.SendMessageToChatRoom, testSendMessageToChatRoom.ChatRoomName, testSendMessageToChatRoom.SenderUsername);

                await _messageRepository.AddAsync(testMessage.Message);

                TestMessagePreview testMessagePreview = ConstructPreviewDto(testMessage.Message, testSendMessageToChatRoom.ChatRoomName, testSendMessageToChatRoom.SenderUserId);

                Guid roomId = testMessage.Message.ChatRoomId;
                await Clients.Group(SignalRGroupService.GetActiveGroupId(roomId)).SendAsync("ReceiveMessage", testMessage);
                await Clients.Group(SignalRGroupService.GetGlobalGroupId(roomId)).SendAsync("ReceiveMessagePreview", testMessagePreview);

                AppLogger.DebugState("ChatHub.SendMessageToChatRoom", $"Message successfully sent to Chat Room ID: {roomId} and stored in database.");
            }
            catch (Exception ex)
            {
                AppLogger.ShieldFailure("ChatHub.SendMessageToChatRoom", ex);
                await SendErrorToClientAsync($"Internal Error: {ex.Message}");
            }
        }
    }
}
