// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: May 16, 2026
//  Description: This file defines the UserEventPublisher class, which is responsible for publishing events related to user
//  status changes and friendship updates. It interacts with the EventService to trigger events when a user's status changes,
//  when a friendship is added, and when a friendship is deleted. The class also uses the IUserRepository to retrieve user
//  information needed for event publishing.
// --------------------------------------------

using Backend.API.src.Core.Interface;

namespace Backend.API.src.Application.Services
{
    public class UserEventPublisher
    {
        private readonly EventService _eventService;
        private readonly IUserRepository _userRepository;
        private readonly IFriendshipRepository _friendshipRepository;

        public UserEventPublisher(EventService eventService, IUserRepository userRepository, IFriendshipRepository friendshipRepository)
        {
            _eventService = eventService;
            _userRepository = userRepository;
            _friendshipRepository = friendshipRepository;
        }

        public async Task PublishUserStatusChangeAsync(Guid userId)
        {
            // Get the user's friends
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return;

            var friends = await _friendshipRepository.GetFriendsByUserIdAsync(userId);
            List<Guid> friendIds = [.. friends.Select(f => f.Id)];

            await _eventService.UserStatusChangeEventAsync(user, friendIds);
        }

        public async Task PublishFriendshipAddAsync(Guid initiatingUser, Guid affectedUser)
        {
            var initiatingUserEntity = await _userRepository.GetByIdAsync(initiatingUser);
            var affectedUserEntity = await _userRepository.GetByIdAsync(affectedUser);

            if (initiatingUserEntity != null && affectedUserEntity != null)
            {
                await _eventService.FriendshipAddEventAsync(initiatingUserEntity, affectedUserEntity);
            }
        }
            

        public async Task PublishFriendshipDeleteAsync(Guid initiatingUser, Guid affectedUser)
        {
            var initiatingUserEntity = await _userRepository.GetByIdAsync(initiatingUser);
            var affectedUserEntity = await _userRepository.GetByIdAsync(affectedUser);

            if (initiatingUserEntity != null && affectedUserEntity != null)
            {
                await _eventService.FriendshipDeleteEventAsync(initiatingUserEntity, affectedUserEntity);
            }
        }
    }
}
