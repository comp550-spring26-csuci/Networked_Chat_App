// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: May 5 2026
//  Description: This service is responsible for handling events that occur within the chat application, such as users
//  joining rooms or sending friend requests. It interacts with the ChatHub to send real-time notifications to clients
//  and uses the ChatEventRepository to persist event data.
// --------------------------------------------

using Backend.API.src.API.Hubs;
using Backend.API.src.Application.DTOs;
using Backend.API.src.Core.Entities;
using Backend.API.src.Core.Interface;
using Backend.API.src.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace Backend.API.src.Application.Services
{
    public class EventService
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ChatEventRepository _eventRepository;
        private readonly IServiceProvider _serviceProvider;

        public EventService(IHubContext<ChatHub> hubContext, ChatEventRepository eventRepository, IServiceProvider serviceProvider)
        {
            _hubContext = hubContext;
            _eventRepository = eventRepository;
            _serviceProvider = serviceProvider;
        }

        public async Task MembershipAddEventAsync(Guid userId, ChatRoom chatRoom)
        {
            ChatEvent chatEvent = new()
            {
                EventType = ChatEventType.MembershipAdded,
                Room = new EventChatRoom
                {
                    ChatRoomId = chatRoom.ChatRoomId,
                    ChatRoomName = chatRoom.ChatRoomName
                }
            };
            
            await _eventRepository.AddAsync(chatEvent);
            
            await _hubContext.Clients.User(userId.ToString()).SendAsync("RoomJoined", chatEvent);
        }

        public async Task MembershipRemoveEventAsync(Guid userId, Guid roomId)
        {
            ChatEvent chatEvent = new()
            {
                EventType = ChatEventType.MembershipRemoved,
                ChatRoomId = roomId
            };

            await _eventRepository.AddAsync(chatEvent);
            
            await _hubContext.Clients.User(userId.ToString()).SendAsync("RoomLeft", chatEvent);
        }

        public async Task RoomDeleteEventAsync(Guid roomId) 
        {
            ChatEvent chatEvent = new()
            {
                EventType = ChatEventType.RoomDeleted,
                ChatRoomId = roomId
            };

            await _eventRepository.AddAsync(chatEvent);

            await _hubContext.Clients.Group(SignalRGroupService.GetGlobalGroupId(roomId)).SendAsync("RoomDeleted", chatEvent);
        }

        public async Task FriendshipAddEventAsync(Guid addresseeId, Guid requesterId)
        {
            ChatEvent chatEvent = new()
            {
                EventType = ChatEventType.FriendshipAdded,
                Friendship = new EventFriendship
                {
                    UserId1 = addresseeId,
                    UserId2 = requesterId,
                    Username1 = (await _serviceProvider.GetRequiredService<IUserRepository>().GetByIdAsync(addresseeId))?.Username ?? "Unknown",
                    Username2 = (await _serviceProvider.GetRequiredService<IUserRepository>().GetByIdAsync(requesterId))?.Username ?? "Unknown"
                    // Add status information too
                }
            };

            await _eventRepository.AddAsync(chatEvent);

            await _hubContext.Clients.User(addresseeId.ToString()).SendAsync("FriendAdded", chatEvent);
            await _hubContext.Clients.User(requesterId.ToString()).SendAsync("FriendAdded", chatEvent);
        }

        public async Task FriendshipRemoveEventAsync(Guid addresseeId, Guid requesterId)
        {
            ChatEvent chatEvent = new()
            {
                EventType = ChatEventType.FriendshipRemoved,
                Friendship = new EventFriendship
                {
                    UserId1 = addresseeId,
                    UserId2 = requesterId,
                    Username1 = (await _serviceProvider.GetRequiredService<IUserRepository>().GetByIdAsync(addresseeId))?.Username ?? "Unknown",
                    Username2 = (await _serviceProvider.GetRequiredService<IUserRepository>().GetByIdAsync(requesterId))?.Username ?? "Unknown"
                    // Add status information too
                }
            };

            await _eventRepository.AddAsync(chatEvent);

            await _hubContext.Clients.User(addresseeId.ToString()).SendAsync("FriendRemoved", chatEvent);
            await _hubContext.Clients.User(requesterId.ToString()).SendAsync("FriendRemoved", chatEvent);
        }



        //public async Task FriendRequestEventAsync(Guid addresseeId, Guid requesterId, string requesterUsername)
        //{
        //    ChatEvent chatEvent = new()
        //    {
        //        EventType = ChatEventType.FriendRequestReceived,
        //        Request = new EventFriendRequest
        //        {
        //            Id = requesterId,
        //            Username = requesterUsername
        //        }
        //    };

        //    await _eventRepository.AddAsync(chatEvent);

        //    await _hubContext.Clients.User(addresseeId.ToString()).SendAsync("FriendRequestReceived", chatEvent);
        //}

        //public async Task FriendAcceptEventAsync(Guid requesterId, Guid addresseeId, string addresseeUsername)
        //{
        //    ChatEvent chatEvent = new()
        //    {
        //        EventType = ChatEventType.FriendRequestAccepted,
        //        Request = new EventFriendRequest
        //        {
        //            Id = addresseeId,
        //            Username = addresseeUsername
        //        }
        //    };

        //    await _eventRepository.AddAsync(chatEvent);

        //    await _hubContext.Clients.User(requesterId.ToString()).SendAsync("FriendRequestAccepted", chatEvent);
        //}
    }
}
