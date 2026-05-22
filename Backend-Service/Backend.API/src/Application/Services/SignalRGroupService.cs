// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: May 7 2026
//  Description: Handle all signalR group connection adjustments.
// --------------------------------------------

using Backend.API.src.API.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Backend.API.src.Application.Services
{
    public class SignalRGroupService
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ClientPresenceService _presenceService;

        public SignalRGroupService(IHubContext<ChatHub> hubContext, ClientPresenceService presenceService)
        {
            _hubContext = hubContext;
            _presenceService = presenceService;
        }

        // Meant to be consistent accross all client instances under the same user,
        // so that all clients will receive the same stream updates. This is important
        // for ensuring that a user doesn't miss any messages or updates when they have multiple clients connected.
        public static string GetGlobalGroupId(Guid roomId) => $"room:global:{roomId}";
        
        // Meant to determine when a client should receive complete messages and updates for a room.
        // This is important for ensuring that clients only receive updates for rooms they are actively
        // participating in, and not for rooms they have left or are not currently active in.
        public static string GetActiveGroupId(Guid roomId) => $"room:active:{roomId}";

        public async Task JoinChatRoomGroupsAsync(string connectionId, Guid roomId)
        {
            await _hubContext.Groups.AddToGroupAsync(connectionId, GetGlobalGroupId(roomId));
            await _hubContext.Groups.AddToGroupAsync(connectionId, GetActiveGroupId(roomId));
        }

        public async Task LeaveChatRoomGroupAsync(string connectionId, Guid roomId)
        {
            await _hubContext.Groups.RemoveFromGroupAsync(connectionId, GetActiveGroupId(roomId));
        }

        public async Task RemoveConnectionsFromChatRoomAsync(Guid roomId, IEnumerable<Guid> userIds)
        {
            string globalGroupId = GetGlobalGroupId(roomId);
            string activeGroupId = GetActiveGroupId(roomId);

            foreach (Guid userId in userIds)
            {
                IEnumerable<string> connections = await _presenceService.GetUserConnections(userId);
                foreach (string connectionId in connections)
                {
                    await _hubContext.Groups.RemoveFromGroupAsync(connectionId, globalGroupId);
                    await _hubContext.Groups.RemoveFromGroupAsync(connectionId, activeGroupId);
                }
            }
        }

        public async Task AddGlobalConnectionFromChatRoomAsync(Guid roomId, Guid userId)
        {
            string globalGroupId = GetGlobalGroupId(roomId);

            IEnumerable<string> connections = await _presenceService.GetUserConnections(userId);
            foreach (string connectionId in connections)
            {
                await _hubContext.Groups.AddToGroupAsync(connectionId, globalGroupId);
            }
        }

        public async Task SyncConnectionGroupsAsync(string connectionId, IEnumerable<Guid> UserRoomIds)
        {
            foreach (Guid roomId in UserRoomIds)
            {
                await _hubContext.Groups.AddToGroupAsync(connectionId, GetGlobalGroupId(roomId));
            }
        }
    }
}
