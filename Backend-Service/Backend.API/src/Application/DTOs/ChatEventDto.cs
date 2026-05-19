// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: May 16, 2026
//  Description: This file defines the ChatEventDto class, which is a Data Transfer Object (DTO) used to represent chat events
//  in the application. The class contains properties for the event's ID, associated chat room ID, event type, details, timestamp,
//  and various related entities such as friendships and chat groups. The ChatEventDto class also includes a static method to create
//  an instance of ChatEventDto from a ChatEvent entity, allowing for easy conversion between the domain model and the DTO used for
//  communication with clients. This DTO is used to send event information to clients in real-time updates or API responses.
// --------------------------------------------

using Backend.API.src.Core.Entities;
using Backend.API.src.Core.Enums;
using System.Text.Json.Serialization;

namespace Backend.API.src.Application.DTOs
{
    public class ChatEventDto
    {
        private string? _id;
        private Guid _chatRoomId;
        private ChatEventType _eventType;
        private string _details = default!;
        private DateTime _timestamp = DateTime.UtcNow;
        //private EventFriendRequest? _friendRequest;
        private EventFriendship? _friendship;
        private EventFriendshipDeleted? _friendshipRemoved;
        private ChatGroupDto? _chatGroup;
        private EventChatGroupDeleted? _chatGroupDeleted;
        private EventChatGroupMembershipDeleted? _chatGroupMembershipDeleted;
        private UserStatus? _userStatus;

        public string? Id { get => _id; set => _id = value; }
        public Guid ChatRoomId { get => _chatRoomId; set => _chatRoomId = value; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required ChatEventType EventType { get => _eventType; set => _eventType = value; }
        public string Details { get => _details; set => _details = value; }
        public DateTime Timestamp { get => _timestamp; set => _timestamp = value; }
        //public EventFriendRequest? FriendRequest { get => _friendRequest; set => _friendRequest = value; }
        public EventFriendship? Friendship { get => _friendship; set => _friendship = value; }
        public EventFriendshipDeleted? FriendshipRemoved { get => _friendshipRemoved; set => _friendshipRemoved = value; }
        public ChatGroupDto? Room { get => _chatGroup; set => _chatGroup = value; }
        public EventChatGroupDeleted? ChatGroupDeleted { get => _chatGroupDeleted; set => _chatGroupDeleted = value; }
        public EventChatGroupMembershipDeleted? ChatGroupMembershipDeleted { get => _chatGroupMembershipDeleted; set => _chatGroupMembershipDeleted = value; }
        public UserStatus? UserStatus { get => _userStatus; set => _userStatus = value; }

        public static ChatEventDto FromEntity(ChatEvent chatEvent) => new()
        {
            Id = chatEvent.Id,
            ChatRoomId = chatEvent.ChatRoomId,
            EventType = chatEvent.EventType,
            Details = chatEvent.Details,
            Timestamp = chatEvent.Timestamp,
            //FriendRequest = chatEvent.FriendRequest,
            Friendship = chatEvent.Friendship,
            FriendshipRemoved = chatEvent.FriendshipRemoved,
            Room = chatEvent.ChatGroup,
            ChatGroupDeleted = chatEvent.ChatGroupDeleted,
            ChatGroupMembershipDeleted = chatEvent.ChatGroupMembershipDeleted,
            UserStatus = chatEvent.UserStatus
        };
    }
}
