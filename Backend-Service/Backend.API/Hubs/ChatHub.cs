// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: March 11 2026
//  Description: Defines the ChatHub class for real-time communication using SignalR.
//  This hub allows clients to send messages to all connected clients or to specific groups,
//  and manage group memberships.
// --------------------------------------------

using Backend.API.src.Core.Entities;
using Backend.API.src.Core.Interface;
using Backend.API.src.Core.Logging;
using Backend.API.src.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver.Core.Servers;

namespace Backend.API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly MessageRepository _messageRepository;

        public ChatHub(MessageRepository messageRepositoy) 
        { 
            _messageRepository = messageRepositoy;

        }


        public override async Task OnConnectedAsync()
        {
            var userName = Context.User?.Identity?.Name ?? "UnknownUser";

            var chatEvent = new ChatEvent
            {
                EventType = "NewConnection",
                Details = $"ConnectionId: {Context.ConnectionId}",
            };

            await Clients.All.SendAsync("UserConnected", Context.ConnectionId, userName);

            await base.OnConnectedAsync();

            // By default, SignalR automatically maps the ClaimTypes.NameIdentifier claim to Context.UserIdentifier
            AppLogger.ConnectionEvent(Context.ConnectionId, "NewConnection", Context.UserIdentifier);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userName = Context.User?.Identity?.Name ?? "UnknownUser";

            var chatEvent = new ChatEvent
            {
                EventType = "RemovedConnection",
                Details = $"ConnectionId: {Context.ConnectionId}"
            };

            await Clients.All.SendAsync("UserDisconnected", Context.ConnectionId, userName);

            await base.OnDisconnectedAsync(exception);

            AppLogger.ConnectionEvent(Context.ConnectionId, "RemovedConnection", Context.UserIdentifier);
        }

        /*
         * At this time, every method in this class uses the following parameter the same way, so we will just explain it once here:
         * param name="user": The name of the user sending the message. This is used to identify the sender to the recipients.
         * 
         * To target a user across all their sessions, we could use a ClaimsPrincipal-based approach with Context.UserIdentifier and 
         * user-specific groups, but that would require additional setup in the authentication middleware to populate the 
         * UserIdentifier based on the authenticated user's ID.
         */

        public async Task SendMessageToGroup(string groupName, string msgString, string userName = "UnknownUser")
        {
            AppLogger.DebugState("ChatHub", "Standard room message");

            try
            {
                var senderId = int.TryParse(Context.UserIdentifier, out var id) ? id : 0;
                
                var chatMessage = new Message
                {
                    ChatRoomId = 0, // We would need to map group names to chat room IDs in a real implementation
                    SenderId = senderId,
                    Content = msgString
                };

                await _messageRepository.AddAsync(chatMessage);

                // If and only if the SenderId is 0, the client should use the userName parameter to display the sender's name, otherwise
                // they should look up the sender's name based on the SenderId in the message. This allows us to support both authenticated
                // users (with a valid SenderId) and unauthenticated users (with a SenderId of 0 and a provided userName).
                await Clients.Group(groupName).SendAsync("ReceiveMessage", chatMessage, userName);
            }
            catch (Exception ex)
            {
                AppLogger.ShieldFailure("ChatHub", ex);
                await Clients.Caller.SendAsync("ReceiveError", $"Internal Error: {ex.Message}");
            }
        }

        /*
        // Need to study functionality of groups more to implement this properly, but here are the basic methods to add/remove from groups
        public async Task JoinGroup(string groupName)
        {
            var userName = Context.User?.Identity?.Name ?? "UnknownUser";

            var chatEvent = new ChatEvent
            {
                EventType = "UserJoinedGroup",
                Details = $"Group: {groupName}"
            };

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("UserJoinedGroup", Context.ConnectionId, groupName);
        }

        public async Task LeaveGroup(string groupName)
        {
            var userName = Context.User?.Identity?.Name ?? "UnknownUser";

            var chatEvent = new ChatEvent
            {
                EventType = "UserLeftGroup",
                Details = $"Group: {groupName}"
            };

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("UserLeftGroup", Context.ConnectionId, groupName);
        }
        */

        // ---------------------------------------------------------------------------------------------------------------------
        // The Message class is less suitable for the following hub methods since it has a "Room" property that doesn't apply to
        // messages sent to all clients or direct messages. We should probably prioritize traditional chat room/group-based messaging
        // for the MVP and then refactor the Message class and hub methods to be more flexible if we want to support more complex
        // messaging patterns later on. For now, we'll just instantiate Message objects in these methods without setting the Room
        // property, but we should consider how to evolve our data models and hub design as we add features.
        // ---------------------------------------------------------------------------------------------------------------------
        
        public async Task SendMessageToAll(string msgString, string userName = "UnknownUser")
        {
            AppLogger.DebugState("ChatHub", "Standard room message");

            try
            {
                var senderId = int.TryParse(Context.UserIdentifier, out var id) ? id : 0;
                var chatMessage = new Message
                {
                    ChatRoomId = 0, // We would need to map group names to chat room IDs in a real implementation
                    SenderId = senderId,
                    Content = msgString
                };

                await _messageRepository.AddAsync(chatMessage);

                await Clients.All.SendAsync("ReceiveMessage", chatMessage);

            }
            catch (Exception ex)
            {
                AppLogger.ShieldFailure("ChatHub", ex);
                await Clients.Caller.SendAsync("ReceiveError", $"Internal Error: {ex.Message}");
            }
        }

        /*
        // Sends a private message to a specific client connection.
        // This method targets a single client based on the specified connection identifier. The
        // param name="connectionId": The unique identifier of the client connection to which the message will be sent. Cannot be null or empty.
        //
        // Note that a user could be connected with multiple devices or browser tabs, each having a different connection ID, so this method is
        // for targeting a specific session rather than a user as a whole. See my comment above SendMessageToAll() in the "user" parameter
        // description for more on how we could target users across sessions with additional setup.
        public async Task SendDirectMessage(string connectionId, string message, string user = "UnknownUser")
        {
            var userName = Context.User?.Identity?.Name ?? user;

            var chatMessage = new Message
            {
                Username = user,
                Content = message,
                Timestamp = DateTime.UtcNow
            };

            await Clients.Client(connectionId).SendAsync("ReceiveMessage", user, message);
        }

        // This method sends a message back to the caller only, which can be useful for acknowledgments or private responses.
        public async Task SendMessageToCaller(string message, string user = "UnknownUser")
        {
            var userName = Context.User?.Identity?.Name ?? user;

            var chatMessage = new Message
            {
                Username = user,
                Content = message,
                Timestamp = DateTime.UtcNow
            };

            await Clients.Caller.SendAsync("ReceiveMessage", user, message);
        }
        */
    }
}
