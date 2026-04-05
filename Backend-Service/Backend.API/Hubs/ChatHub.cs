// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: March 11 2026
//  Description: Defines the ChatHub class for real-time communication using SignalR.
//  This hub allows clients to send messages to all connected clients or to specific groups,
//  and manage group memberships.
// --------------------------------------------

using Microsoft.AspNetCore.SignalR;
using Backend.API.src.Core.Logging;
using Backend.API.src.Core.Entities;

namespace Backend.API.Hubs
{
    public class ChatHub : Hub
    {


        //----------------------------------
        //----------- Methods --------------
        //----------------------------------


        public override async Task OnConnectedAsync()
        {
            var userName = Context.User?.Identity?.Name ?? "UnknownUser";

            var chatEvent = new ChatEvent
            {
                EventType = "UserConnected",
                Username = userName,
                Details = $"ConnectionId: {Context.ConnectionId}",
                Timestamp = DateTime.UtcNow
            };

            // Implement authentication middleware to replace this with actual user information (eg. Context.User?.Identity?.Name)
            await Clients.All.SendAsync("UserConnected", Context.ConnectionId, userName);

            await base.OnConnectedAsync();

            AppLogger.ConnectionEvent(Context.ConnectionId, "UserConnected", Context.UserIdentifier);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userName = Context.User?.Identity?.Name ?? "UnknownUser";

            var chatEvent = new ChatEvent
            {
                EventType = "UserDisconnected",
                Username = userName,
                Details = $"ConnectionId: {Context.ConnectionId}",
                Timestamp = DateTime.UtcNow
            };

            await Clients.All.SendAsync("UserDisconnected", Context.ConnectionId, userName);

            await base.OnDisconnectedAsync(exception);

            AppLogger.ConnectionEvent(Context.ConnectionId, "UserDisconnected", Context.UserIdentifier);
        }

        /*
         * At this time, every method in this class uses the following parameter the same way, so we will just explain it once here:
         * param name="user": The name of the user sending the message. This is used to identify the sender to the recipients.
         * 
         * To target a user across all their sessions, we could use a ClaimsPrincipal-based approach with Context.UserIdentifier and 
         * user-specific groups, but that would require additional setup in the authentication middleware to populate the 
         * UserIdentifier based on the authenticated user's ID.
         */

        public async Task SendMessageToGroup(string groupName, string message, string user = "UnknownUser")
        {
            var userName = Context.User?.Identity?.Name ?? user;

            var chatMessage = new Message
            {
                Username = user,
                Room = groupName,
                Content = message,
                Timestamp = DateTime.UtcNow
            };

            await Clients.Group(groupName).SendAsync("ReceiveMessage", user, message);
        }

        // Need to study functionality of groups more to implement this properly, but here are the basic methods to add/remove from groups
        public async Task JoinGroup(string groupName)
        {
            var userName = Context.User?.Identity?.Name ?? "UnknownUser";

            var chatEvent = new ChatEvent
            {
                EventType = "UserJoinedGroup",
                Username = userName,
                Details = $"Group: {groupName}",
                Timestamp = DateTime.UtcNow
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
                Username = userName,
                Details = $"Group: {groupName}",
                Timestamp = DateTime.UtcNow
            };

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("UserLeftGroup", Context.ConnectionId, groupName);
        }


        // ---------------------------------------------------------------------------------------------------------------------
        // The Message class is less suitable for the following hub methods since it has a "Room" property that doesn't apply to
        // messages sent to all clients or direct messages. We should probably prioritize traditional chat room/group-based messaging
        // for the MVP and then refactor the Message class and hub methods to be more flexible if we want to support more complex
        // messaging patterns later on. For now, we'll just instantiate Message objects in these methods without setting the Room
        // property, but we should consider how to evolve our data models and hub design as we add features.
        // ---------------------------------------------------------------------------------------------------------------------

        public async Task SendMessageToAll(string message, string user = "UnknownUser")
        {
            var userName = Context.User?.Identity?.Name ?? user;

            var chatMessage = new Message
            {
                Username = user,
                Content = message,
                Timestamp = DateTime.UtcNow
            };

            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

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
    }
}
