// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: March 11 2026
//  Description: Defines the ChatHub class for real-time communication using SignalR.
//  This hub allows clients to send messages to all connected clients or to specific groups,
//  and manage group memberships.
// --------------------------------------------

using Backend.API.src.Application.DTOs.TestDTOs;
using Backend.API.src.Core.Entities;
using Backend.API.src.Core.Logging;
using Backend.API.src.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
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

        // -------------------------------------
        // ******USER INFO HELPER METHODS*******
        // -------------------------------------

        //private string? GetUsername(TestSendMessage? testSendMessage = null)
        //{
        //    return Context.User?.Identity?.Name ?? testSendMessage?.Username ?? "UnknownUser";
        //}

        //private string GetUsername()
        //{
        //    return Context.User?.Identity?.Name ?? "UnknownUser";
        //}

        //private string TestGetUsername(TestMessage testMessage) {
        //    var username = GetUsername();
        //    if (username == "UnknownUser")
        //    {
        //        return testMessage.Username ?? "UnknownUser";
        //    }
        //    return Context.User?.Identity?.Name ?? testMessage.Username ?? "UnknownUser";
        //}

        // By default, SignalR automatically maps the ClaimTypes.NameIdentifier claim to Context.UserIdentifier
        private int? GetUserId()
        {
            return int.TryParse(Context.UserIdentifier, out var userId) ? userId : null;
        }

        //private int GetChatRoomId(Message message)
        //{
        //    return message.ChatRoomId;
        //}

        //private string GetChatRoomName(Message message)
        //{
        //    // TODO: In a real application, you would likely want to look up the chat room name
        //    // based on the ChatRoomId in the message, rather than just returning a placeholder value.
        //    return "UnknownChatRoom";
        //}

        //private string TestGetChatRoomName(TestMessage testSendMessage)
        //{

        //    // TODO: In a real application, you would likely want to look up the chat room name
        //    // based on the ChatRoomId in the message, rather than just returning a placeholder value.
        //    return testSendMessage.ChatRoomName ?? "UnknownChatRoom";
        //}

        //private string GetMessageContent(TestSendMessage? testSendMessage = null)
        //{
        //    return testSendMessage?.SendMessage?.Content ?? string.Empty;
        //}

        // -------------------------------------
        // ***MESSAGE SENDING HELPER METHODS****
        // -------------------------------------

        private async Task SendMessageToGroupAsync(TestMessage testMessage)
        {
            string group = testMessage.Message.ChatRoomId.ToString() ?? testMessage.ChatRoomName ?? "UnknownChatRoom";
            await Clients.Group(group).SendAsync("ReceiveMessage", testMessage);
        }

        private async Task SendMessageToAllAsync(TestMessage testMessage)
        {
            await Clients.All.SendAsync("ReceiveMessage", testMessage);
        }

        private async Task SendMessageToCallerAsync(TestMessage testMessage)
        {
            await Clients.Caller.SendAsync("ReceiveMessage", testMessage);
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
            string group = testChatEvent.ChatEvent.ChatRoomId?.ToString() ?? testChatEvent.ChatRoomName;
            await Clients.Group(group).SendAsync("ReceiveEvent", testChatEvent.ChatEvent);
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
                    EventType = "NewConnection"
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
        public async Task JoinChatRoom(TestChatRoom testChatRoom)
        {
            try
            { 
                var chatEvent = new ChatEvent
                {
                    EventType = "UserJoinedChatRoom",
                    ChatRoomId = testChatRoom.ChatRoomId,
                    Details = $"ChatRoom: {testChatRoom.ChatRoomId?.ToString() ?? testChatRoom.ChatRoomName ?? "NullChatRoom"}"
                };


                await Groups.AddToGroupAsync(Context.ConnectionId, testChatRoom.ChatRoomId?.ToString() ?? testChatRoom.ChatRoomName ?? "NullChatRoom");

                await _chatEventRepository.AddAsync(chatEvent);

                var testChatEvent = new TestChatEvent
                {
                    ChatEvent = chatEvent,
                    ChatRoomName = testChatRoom.ChatRoomName
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
        public async Task LeaveChatRoom(TestChatRoom testChatRoom)
        {
            try
            { 
                var chatEvent = new ChatEvent
                {
                    EventType = "UserLeftChatRoom",
                    ChatRoomId = testChatRoom.ChatRoomId ?? 0,
                    Details = $"ChatRoom: {testChatRoom.ChatRoomId?.ToString() ?? testChatRoom.ChatRoomName ?? "NullChatRoom"}"
                };

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, testChatRoom.ChatRoomId?.ToString() ?? testChatRoom.ChatRoomName ?? "NullChatRoom");
                
                await _chatEventRepository.AddAsync(chatEvent);

                var testChatEvent = new TestChatEvent
                {
                    ChatEvent = chatEvent,
                    ChatRoomName = testChatRoom.ChatRoomName
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

                // Find ChatRoom by ChatRoomId. If it exists, use ChatRoomId as the group name. If it doesn't exist, we can either create a new group
                // or return an error. For now, we'll just use the ChatRoomId as the group name and assume it exists.

                var testMessage = new TestMessage
                {
                    Message = message,
                    ChatRoomName = testSendMessageToChatRoom.ChatRoomName,
                    Username = testSendMessageToChatRoom.Username
                };

                await SendMessageToGroupAsync(testMessage);
            }
            catch (Exception ex)
            {
                await SendErrorToClientAsync($"Internal Error: {ex.Message}");
            }
        }

        public async Task SendMessageToAll(TestSendMessage testSendMessage)
        {
            try
            {
                var message = new Message
                {
                    Content = testSendMessage.SendMessage.Content,
                    SenderId = GetUserId()
                };

                await _messageRepository.AddAsync(message);

                var testMessage = new TestMessage
                {
                    Message = message,
                    Username = testSendMessage.Username
                };

                await SendMessageToAllAsync(testMessage);
            }
            catch (Exception ex)
            {
                await SendErrorToClientAsync($"Internal Error: {ex.Message}");
            }
        }

        // This method sends a message back to the caller only, which can be useful for acknowledgments or private responses.
        public async Task SendMessageToCaller(TestSendMessage testSendMessage)
        {
            AppLogger.DebugState("ChatHub", "SendMessageToCaller called", new { testSendMessage });
            try
            {
                var message = new Message
                {
                    SenderId = GetUserId(),
                    Content = testSendMessage.SendMessage.Content
                };
                
                await _messageRepository.AddAsync(message);

                var testMessage = new TestMessage
                {
                    Message = message,
                    Username = testSendMessage.Username
                };

                await SendMessageToCallerAsync(testMessage);
            }
            catch (Exception ex)
            {
                await SendErrorToClientAsync($"Internal Error: {ex.Message}");

                AppLogger.ShieldFailure("ChatHub.SendMessageToCaller", ex);
            }
        }
    }
}
