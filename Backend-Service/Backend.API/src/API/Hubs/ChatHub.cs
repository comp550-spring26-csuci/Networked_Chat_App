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
using Backend.API.src.Core.Enums;
using Backend.API.src.Core.Interface;
using Backend.API.src.Core.Logging;
using Backend.API.src.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
namespace Backend.API.src.API.Hubs
{
    [Authorize]
    public class ChatHub : Hub<IChatClient>
    {
        private readonly MessageRepository _messageRepository;
        private readonly ChatEventRepository _chatEventRepository;
        private readonly IChatGroupRepository _chatGroupRepository;
        private readonly SignalRGroupService _signalRGroupService;
        private readonly ClientPresenceService _clientPresenceService;
        private readonly IUserRepository _userRepository;
        private readonly UserEventPublisher _userEventPublisher;

        public ChatHub(
            MessageRepository messageRepositoy,
            ChatEventRepository chatEventRepository,
            IChatGroupRepository chatGroupRepository,
            SignalRGroupService signalRGroupService,
            ClientPresenceService clientPresenceService,
            IUserRepository userRepository,
            UserEventPublisher userEventPublisher)
        {
            _messageRepository = messageRepositoy;
            _chatEventRepository = chatEventRepository;
            _chatGroupRepository = chatGroupRepository;
            _signalRGroupService = signalRGroupService;
            _clientPresenceService = clientPresenceService;
            _userRepository = userRepository;
            _userEventPublisher = userEventPublisher;
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

        private Message ConstructMessage(SendMessageToChatRoom sendMessageToChatRoom)
        {
            var message = new Message
            {
                ChatRoomId = sendMessageToChatRoom.ChatRoomId,
                Content = sendMessageToChatRoom.Content,
                SenderId = GetUserId(),
                SenderUsername = GetUsername()
            };

            return message;
        }

        private static TestMessagePreview ConstructPreviewDto(MessageDto message, string? chatRoomName)
        {
            string preview = message.Content.Length > 50 ? string.Concat(message.Content.AsSpan(0, 50), "...") : message.Content;

            var messagePreview = new MessagePreview
            {
                SenderId = message.SenderId,
                MessageId = message.Id,
                ChatRoomId = message.ChatRoomId,
                Content = preview,
                SenderUsername = message.SenderUsername,
                Timestamp = message.Timestamp
            };

            var testMessagePreview = new TestMessagePreview
            {
                MessagePreview = messagePreview,
                ChatRoomName = chatRoomName
            };

            return testMessagePreview;
        }

        // -------------------------------------
        // ****ERROR SENDING HELPER METHODS*****
        // -------------------------------------

        private async Task SendErrorToClientAsync(string errorMessage)
        {
            await Clients.Caller.ReceiveError(errorMessage);
        }

        // -------------------------------------
        // **CHAT EVENT SENDING HELPER METHODS**
        // -------------------------------------

        private async Task SendEventToAllAsync(TestChatEvent testChatEvent)
        {
            await Clients.All.ReceiveEvent(testChatEvent);
        }

        private async Task SendEventToGroupAsync(TestChatEvent testChatEvent)
        {
            string activeGroup = SignalRGroupService.GetActiveGroupId(testChatEvent.ChatEvent.ChatRoomId);
            await Clients.Group(activeGroup).ReceiveEvent(testChatEvent);
        }

        private async Task SendEventToCallerUserAsync(TestChatEvent testChatEvent)
        {
            await Clients.User(GetUserId().ToString()).ReceiveEvent(testChatEvent);
        }

        private ChatEvent ConstructChatEvent(ChatEventType eventType, Guid chatRoomId = default)
        {
            var chatEvent = new ChatEvent
            {
                EventType = eventType,
                ChatRoomId = chatRoomId,
                Details = $"Username: {GetUsername()}"
            };

            return chatEvent;
        }

        // -------------------------------------
        // ***STATUS REPORTING HELPER METHODS***
        // -------------------------------------

        public async Task UpdateStatusOnline()
        {
            Guid userId = GetUserId();

            var user = await _userRepository.GetByIdAsync(userId);

            if (user != null && user.PresenceStatus == UserStateType.Inactive)
            {
                user.PresenceStatus = UserStateType.Active;
                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();

                await _userEventPublisher.PublishUserStatusChangeAsync(user.Id);
            }
        }

        // -------------------------------------
        // *************HUB METHODS*************
        // -------------------------------------

        public override async Task OnConnectedAsync()
        {
            AppLogger.ConnectionEvent(Context.ConnectionId, "Connected", GetUserId().ToString());

            Guid userId = GetUserId();

            await _clientPresenceService.UserSessionStarted(userId, Context.ConnectionId);

            List<Guid> myChatRooms = [.. _chatGroupRepository.GetGroupsForUserAsync(GetUserId()).Result.Select(g => g.Id)];

            await _signalRGroupService.SyncConnectionGroupsAsync(Context.ConnectionId, myChatRooms);

            ChatEvent chatEvent = ConstructChatEvent(ChatEventType.UserJoined);

            var testChatEvent = new TestChatEvent
            {
                ChatEvent = ChatEventDto.FromEntity(chatEvent)
            };

            await _chatEventRepository.AddAsync(chatEvent);

            await SendEventToAllAsync(testChatEvent);

            await UpdateStatusOnline();

            await base.OnConnectedAsync();

            AppLogger.DebugState("ChatHub", $"User connection initialization completed for Connection ID: {Context.ConnectionId}, User ID: {GetUserId()}, Username: {GetUsername()}");
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            AppLogger.ConnectionEvent(Context.ConnectionId, "Disconnected", GetUserId().ToString());

            Guid userId = GetUserId();

            await _clientPresenceService.UserSessionEnded(userId, Context.ConnectionId);

            User? user = await _userRepository.GetByIdAsync(userId);

            if (user != null && user.PresenceStatus == UserStateType.Active)
            {
                // Logout/Disconnect from ANY chat groups the user is part of

                user.PresenceStatus = UserStateType.Inactive;

                _userRepository.Update(user);

                await _userRepository.SaveChangesAsync();

                await _userEventPublisher.PublishUserStatusChangeAsync(user.Id);
            }

            ChatEvent chatEvent = ConstructChatEvent(ChatEventType.UserLeft);

            var testChatEvent = new TestChatEvent
            {
                ChatEvent = ChatEventDto.FromEntity(chatEvent)
            };

            await _chatEventRepository.AddAsync(chatEvent);

            await SendEventToAllAsync(testChatEvent);

            await base.OnDisconnectedAsync(exception);

            AppLogger.DebugState("ChatHub", $"User disconnection handling completed for Connection ID: {Context.ConnectionId}, User ID: {GetUserId()}, Username: {GetUsername()}");
        }

        public async Task JoinChatRoom(TestPerformChatRoomAction testPerformChatRoomAction)
        {
            AppLogger.DebugState("ChatHub.JoinChatRoom", $"User with Connection ID: {Context.ConnectionId}, User ID: {GetUserId()}, Username: {GetUsername()} is attempting to join Chat Room ID: {testPerformChatRoomAction.PerformChatRoomAction.ChatRoomId}");
            await UpdateStatusOnline();

            Guid roomId = testPerformChatRoomAction.PerformChatRoomAction.ChatRoomId;

            if (roomId == default || !await _chatGroupRepository.IsUserInGroupAsync(roomId, GetUserId()))
            {
                await SendErrorToClientAsync($"Chat room id \"{roomId}\" does not exist. Please provide a valid ChatRoomId.");
                return;
            }

            ChatEvent chatEvent = ConstructChatEvent(ChatEventType.UserJoined, roomId);

            var testChatEvent = new TestChatEvent
            {
                ChatEvent = ChatEventDto.FromEntity(chatEvent),
                ChatRoomName = testPerformChatRoomAction.ChatRoomName ?? ""
            };

            await _chatEventRepository.AddAsync(chatEvent);

            await _signalRGroupService.JoinChatRoomGroupsAsync(Context.ConnectionId, roomId);
            await SendEventToGroupAsync(testChatEvent);

            AppLogger.DebugState("ChatHub.JoinChatRoom", $"User successfully joined Chat Room ID: {roomId}. Event broadcasted to group.");
        }

        public async Task LeaveChatRoom(TestPerformChatRoomAction testPerformChatRoomAction)
        {
            AppLogger.DebugState("ChatHub.LeaveChatRoom", $"User with Connection ID: {Context.ConnectionId}, User ID: {GetUserId()}, Username: {GetUsername()} is attempting to leave Chat Room ID: {testPerformChatRoomAction.PerformChatRoomAction.ChatRoomId}");
            Guid roomId = testPerformChatRoomAction.PerformChatRoomAction.ChatRoomId;

            ChatEvent chatEvent = ConstructChatEvent(ChatEventType.UserLeft, roomId);

            var testChatEvent = new TestChatEvent
            {
                ChatEvent = ChatEventDto.FromEntity(chatEvent),
                ChatRoomName = testPerformChatRoomAction.ChatRoomName ?? ""
            };

            await _chatEventRepository.AddAsync(chatEvent);

            await _signalRGroupService.LeaveChatRoomGroupAsync(Context.ConnectionId, roomId);
            await SendEventToCallerUserAsync(testChatEvent);
            await SendEventToGroupAsync(testChatEvent);

            AppLogger.DebugState("ChatHub.LeaveChatRoom", $"User successfully left Chat Room ID: {roomId}. Event broadcasted to group and caller.");
        }

        public async Task SendMessageToChatRoom(TestSendMessageToChatRoom testSendMessageToChatRoom)
        {
            AppLogger.DebugState("ChatHub.SendMessageToChatRoom", $"User with Connection ID: {Context.ConnectionId}, User ID: {GetUserId()}, Username: {GetUsername()} is attempting to send a message to Chat Room ID: {testSendMessageToChatRoom.SendMessageToChatRoom.ChatRoomId}");

            await UpdateStatusOnline();

            if (testSendMessageToChatRoom.SendMessageToChatRoom.ChatRoomId == default
            || !await _chatGroupRepository.IsUserInGroupAsync(testSendMessageToChatRoom.SendMessageToChatRoom.ChatRoomId, GetUserId()))
            {
                await SendErrorToClientAsync($"Chat room id \"{testSendMessageToChatRoom.SendMessageToChatRoom.ChatRoomId}\" does not exist. Please provide a valid ChatRoomId.");
                return;
            }

            Message message = ConstructMessage(testSendMessageToChatRoom.SendMessageToChatRoom);

            await _messageRepository.AddAsync(message);

            var testMessage = new TestMessage
            {
                Message = MessageDto.FromEntity(message),
                ChatRoomName = testSendMessageToChatRoom.ChatRoomName
            };

            await _chatGroupRepository.IncrementUnreadCountAsync(message.ChatRoomId, message.SenderId);

            await _chatGroupRepository.SaveChangesAsync();

            TestMessagePreview testMessagePreview = ConstructPreviewDto(testMessage.Message, testSendMessageToChatRoom.ChatRoomName);

            Guid roomId = testMessage.Message.ChatRoomId;
            await Clients.Group(SignalRGroupService.GetActiveGroupId(roomId)).ReceiveMessage(testMessage);
            await Clients.Group(SignalRGroupService.GetGlobalGroupId(roomId)).ReceiveMessagePreview(testMessagePreview);

            AppLogger.DebugState("ChatHub.SendMessageToChatRoom", $"Message successfully sent to Chat Room ID: {roomId} and stored in database.");
        }

        public async Task MarkRoomAsRead(TestPerformChatRoomAction testPerformChatRoomAction)
        {
            AppLogger.DebugState("ChatHub.ReceiveMarkedAsRead", $"User with Connection ID: {Context.ConnectionId}, User ID: {GetUserId()}, Username: {GetUsername()} is attempting to mark Chat Room ID: {testPerformChatRoomAction.PerformChatRoomAction.ChatRoomId} as read");
            await UpdateStatusOnline();

            Guid roomId = testPerformChatRoomAction.PerformChatRoomAction.ChatRoomId;

            if (roomId == default || !await _chatGroupRepository.IsUserInGroupAsync(roomId, GetUserId()))
            {
                await SendErrorToClientAsync($"Chat room id \"{roomId}\" does not exist. Please provide a valid ChatRoomId.");
                return;
            }

            await _chatGroupRepository.ResetUnreadCountAsync(roomId, GetUserId());

            await _chatGroupRepository.SaveChangesAsync();

            // Notify the user's client instances that the room has been marked as read (e.g., to update the UI to show that there are no unread messages in that room)
            var testChatRoomRead = new TestChatRoomRead
            {
                ChatRoomRead = new ChatRoomRead { Id = roomId },
                ChatRoomName = testPerformChatRoomAction.ChatRoomName,
                UserId = testPerformChatRoomAction.UserId,
                Username = testPerformChatRoomAction.Username
            };
            await Clients.User(GetUserId().ToString()).ReceiveMarkedAsRead(testChatRoomRead);

            // Mark the room as read for the user in the database (e.g., update the last read timestamp for that user and chat room)
            AppLogger.DebugState("ChatHub.ReceiveMarkedAsRead", $"Chat Room ID: {testPerformChatRoomAction.PerformChatRoomAction.ChatRoomId} successfully marked as read for User ID: {GetUserId()}");
        }
    }
}
