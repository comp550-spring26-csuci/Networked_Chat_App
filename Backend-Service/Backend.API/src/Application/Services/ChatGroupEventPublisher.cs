// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: May 16, 2026
//  Description: This file defines the ChatGroupEventPublisher class, which is responsible for publishing events related to
//  chat group membership and room management. It interacts with the EventService to trigger events when users are added or
//  removed from chat groups, and when chat rooms are deleted. The class also uses the SignalRGroupService to manage real-time
//  updates for chat group membership changes.
// --------------------------------------------

using Backend.API.src.Core.Entities;
using Backend.API.src.Core.Interface;

namespace Backend.API.src.Application.Services
{
    public class ChatGroupEventPublisher
    {
        private readonly EventService _eventService;
        private readonly IChatGroupRepository _chatGroupRepository;
        private readonly SignalRGroupService _signalRGroupService;

        public ChatGroupEventPublisher(EventService eventService, IChatGroupRepository chatGroupRepository, SignalRGroupService signalRGroupService)
        {
            _eventService = eventService;
            _chatGroupRepository = chatGroupRepository;
            _signalRGroupService = signalRGroupService;
        }

        public async Task PublishMembershipAddAsync(Guid userId, ChatGroup chatGroup)
        {
            await _eventService.MembershipAddEventAsync(userId, chatGroup);
            
            await _signalRGroupService.AddGlobalConnectionFromChatRoomAsync(chatGroup.Id, userId);
        }
    
        public async Task PublishMembershipDeleteAsync(Guid userId, Guid chatGroupId)
        {
            await _signalRGroupService.RemoveConnectionsFromChatRoomAsync(chatGroupId, [userId]);

            await _eventService.MembershipDeleteEventAsync(userId, chatGroupId);
        }
    
        public async Task PublishRoomDeleteAsync(Guid roomId)
        {
            await _eventService.RoomDeleteEventAsync(roomId); 
            
            var members = await _chatGroupRepository.GetGroupMembersAsync(roomId);

            List<Guid> userIds = [.. members.Select(m => m.UserId)];

            await _signalRGroupService.RemoveConnectionsFromChatRoomAsync(roomId, userIds);
        }
    }
}
