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
using Backend.API.src.Core.Interface;
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
        private readonly IUserRepository _userRepository;

        public ChatHub(MessageRepository messageRepositoy, ChatEventRepository chatEventRepository, TestChatRoomRepository testChatRoomRepository, SignalRGroupService signalRGroupService, ClientPresenceService clientPresenceService, IUserRepository userRepository)
        {
            _messageRepository = messageRepositoy;
            _chatEventRepository = chatEventRepository;
            _testChatRoomRepository = testChatRoomRepository;
            _signalRGroupService = signalRGroupService;
            _clientPresenceService = clientPresenceService;
            _userRepository = userRepository;
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

        private TestMessage ConstructMessageDto(SendMessageToChatRoom sendMessageToChatRoom, string? chatRoomName)
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
                ChatRoomName = chatRoomName
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
        // ***STATUS REPORTING HELPER METHODS***
        // -------------------------------------

        private async Task UpdateStatusOnline()
        {
            Guid userId = GetUserId();

            var user = await _userRepository.GetByIdAsync(userId);

            if (user != null /*&& user.PresenceStatus == UserStateType.Inactive*/)
            {
                //user.PresenceStatus = UserStateType.Active;
                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();
                await SendStatusToFriends(user);
            }
        }

        private async Task SendStatusToFriends(User user)
        {
            var testUserStatus = ConstructTestUserStatusDto(user);

            // Notify all of the user's own client instances of their updated status (e.g., to update the UI to show them as online)
            await Clients.User(user.Id.ToString()).SendAsync("ReceiveStatus", testUserStatus);

            // Get friendships from friendship repository

            // for loop to send the status update to each friend
        }

        private static TestUserStatus ConstructTestUserStatusDto(User user) 
        {
            return new TestUserStatus
            {
                // UserStatus = new UserStatus { UserId = user.Id, State = UserStateType.Active },
                UserName = user.Username
            };
        }

        // -------------------------------------
        // *************HUB METHODS*************
        // -------------------------------------

        public override async Task OnConnectedAsync()
        {
            AppLogger.ConnectionEvent(Context.ConnectionId, "Connected", GetUserId().ToString());
            try
            {
                Guid userId = GetUserId();

                await _clientPresenceService.UserSessionStarted(userId, Context.ConnectionId);

                //////////////////// PENDING REWRITE /////////////////////////////////////////////
                List<Guid> myChatRooms = _testChatRoomRepository.GetMyChatRoomIds(GetUsername());
                //////////////////// PENDING REWRITE /////////////////////////////////////////////

                await _signalRGroupService.SyncConnectionGroupsAsync(Context.ConnectionId, myChatRooms);

                TestChatEvent testChatEvent = ConstructChatEventDto(ChatEventType.UserJoined, null);

                await _chatEventRepository.AddAsync(testChatEvent.ChatEvent);

                await SendEventToAllAsync(testChatEvent);

                await UpdateStatusOnline();
               
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
                Guid userId = GetUserId();

                await _clientPresenceService.UserSessionEnded(userId, Context.ConnectionId);

                User? user = await _userRepository.GetByIdAsync(userId);

                if (user != null /* && user.PresenceStatus == UserStateType.Active */)
                {
                    // Logout/Disconnect from ANY 

                    //user.PresenceStatus = UserStateType.Inactive;

                    _userRepository.Update(user);

                    await _userRepository.SaveChangesAsync();

                    await SendStatusToFriends(user);
                }

                TestChatEvent testChatEvent = ConstructChatEventDto(ChatEventType.UserLeft, null);

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
                await UpdateStatusOnline();

                Guid roomId = ChatRoom.ChatRoomId;

                if (roomId == default || !_testChatRoomRepository.ChatRoomExists(roomId))
                {
                    await SendErrorToClientAsync($"Chat room id \"{roomId}\" does not exist. Please provide a valid ChatRoomId.");
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
                await UpdateStatusOnline();

                TestMessage testMessage = ConstructMessageDto(testSendMessageToChatRoom.SendMessageToChatRoom, testSendMessageToChatRoom.ChatRoomName);

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

        public async Task MarkRoomAsRead(Guid chatRoomId)
        {
            AppLogger.DebugState("ChatHub.MarkRoomAsRead", $"User with Connection ID: {Context.ConnectionId}, User ID: {GetUserId()}, Username: {GetUsername()} is attempting to mark Chat Room ID: {chatRoomId} as read");
            try
            {
                await UpdateStatusOnline();
                if (chatRoomId == default || !_testChatRoomRepository.ChatRoomExists(chatRoomId))
                {
                    await SendErrorToClientAsync($"Chat room id \"{chatRoomId}\" does not exist. Please provide a valid ChatRoomId.");
                    return;
                }

                // Notify the user's client instances that the room has been marked as read (e.g., to update the UI to show that there are no unread messages in that room)
                await Clients.User(GetUserId().ToString()).SendAsync("RoomMarkedAsRead", chatRoomId);

                // Mark the room as read for the user in the database (e.g., update the last read timestamp for that user and chat room)
                AppLogger.DebugState("ChatHub.MarkRoomAsRead", $"Chat Room ID: {chatRoomId} successfully marked as read for User ID: {GetUserId()}");
            }
            catch (Exception ex)
            {
                AppLogger.ShieldFailure("ChatHub.MarkRoomAsRead", ex);
                await SendErrorToClientAsync($"Internal Error: {ex.Message}");
            }
        }
    }
}
