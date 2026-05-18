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
using Backend.API.src.Core.Enums;
using Backend.API.src.Core.Interface;
using Backend.API.src.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace Backend.API.src.Application.Services
{
    public class EventService
    {
        private readonly IHubContext<ChatHub, IChatClient> _hubContext;
        private readonly ChatEventRepository _eventRepository;

        public EventService(IHubContext<ChatHub, IChatClient> hubContext, ChatEventRepository eventRepository)
        {
            _hubContext = hubContext;
            _eventRepository = eventRepository;
        }

        public async Task MembershipAddEventAsync(Guid userId/*, ChatGroup chatGroup*/)
        {
            ChatEvent chatEvent = new()
            {
                EventType = ChatEventType.ChatGroupMembershipAdded,
                //ChatGroup = ChatGroupDto.FromEntity(chatGroup)
            };
            
            await _eventRepository.AddAsync(chatEvent);
            
            await _hubContext.Clients.User(userId.ToString()).ChatGroupMembershipAdded(ChatEventDto.FromEntity(chatEvent));
        }

        public async Task MembershipDeleteEventAsync(Guid userId, Guid roomId)
        {
            ChatEvent chatEvent = new()
            {
                EventType = ChatEventType.ChatGroupMembershipDeleted,
                ChatRoomId = roomId
            };

            await _eventRepository.AddAsync(chatEvent);
            
            await _hubContext.Clients.User(userId.ToString()).ChatGroupMembershipDeleted(ChatEventDto.FromEntity(chatEvent));
        }

        public async Task RoomDeleteEventAsync(Guid roomId) 
        {
            ChatEvent chatEvent = new()
            {
                EventType = ChatEventType.ChatGroupDeleted,
                ChatRoomId = roomId
            };

            await _eventRepository.AddAsync(chatEvent);

            await _hubContext.Clients.Group(SignalRGroupService.GetGlobalGroupId(roomId)).ChatGroupDeleted(ChatEventDto.FromEntity(chatEvent));
        }

        public async Task FriendshipAddEventAsync(User initiatingUser, User affectedUser)
        {
            ChatEvent chatEvent = new()
            {
                EventType = ChatEventType.FriendshipAdded,
                Friendship = EventFriendship.FromUsers(initiatingUser, affectedUser)
            };

            await _eventRepository.AddAsync(chatEvent);

            var userIds = new[] { initiatingUser.Id, affectedUser.Id }.Select(id => id.ToString()).ToArray();

            await _hubContext.Clients.Users(userIds).FriendshipAdded(ChatEventDto.FromEntity(chatEvent));
        }

        public async Task FriendshipDeleteEventAsync(Guid initiatingUser, Guid affectedUser)
        {
            ChatEvent chatEvent = new()
            {
                EventType = ChatEventType.FriendshipDeleted,
                FriendshipRemoved = EventFriendshipDeleted.FromIds(initiatingUser, affectedUser)
            };

            await _eventRepository.AddAsync(chatEvent);

            var userIds = new[] { initiatingUser, affectedUser }.Select(id => id.ToString()).ToArray(); 
            
            await _hubContext.Clients.Users(userIds).FriendshipDeleted(ChatEventDto.FromEntity(chatEvent));
        }

        public async Task UserStatusChangeEventAsync(User user, IEnumerable<Guid> friendIds)
        {
            ChatEvent chatEvent = new()
            {
                EventType = ChatEventType.UserStatusChanged,
                //UserStatus = newStatus
            };

            await _eventRepository.AddAsync(chatEvent);
            
            await _hubContext.Clients.User(user.Id.ToString()).MyUserStatusChanged(ChatEventDto.FromEntity(chatEvent));

            var idStrings = friendIds.Select(id => id.ToString()).ToArray();
            if (idStrings.Length != 0)
            {
                await _hubContext.Clients.Users(idStrings).FriendUserStatusChanged(ChatEventDto.FromEntity(chatEvent));
            }
        }
    }
}
