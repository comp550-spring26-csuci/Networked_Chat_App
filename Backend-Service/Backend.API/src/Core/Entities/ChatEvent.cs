// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: April 5 2026
//  Description: Defines the ChatEvent entity.
//               This will match the "Events" collection
// --------------------------------------------

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;
using Backend.API.src.Application.DTOs;
using Backend.API.src.Core.Enums;

namespace Backend.API.src.Core.Entities
{
    public class ChatEvent
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

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get => _id; set => _id = value; }
        [BsonRepresentation(BsonType.String)]
        public Guid ChatRoomId { get => _chatRoomId; set => _chatRoomId = value; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public required ChatEventType EventType { get => _eventType; set => _eventType = value; }
        public string Details { get => _details; set => _details = value; }
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime Timestamp { get => _timestamp; set => _timestamp = value; }
        //public EventFriendRequest? FriendRequest { get => _friendRequest; set => _friendRequest = value; }
        public EventFriendship? Friendship { get => _friendship; set => _friendship = value; }
        public EventFriendshipDeleted? FriendshipRemoved { get => _friendshipRemoved; set => _friendshipRemoved = value; }
        public ChatGroupDto? ChatGroup { get => _chatGroup; set => _chatGroup = value; }
        public EventChatGroupDeleted? ChatGroupDeleted { get => _chatGroupDeleted; set => _chatGroupDeleted = value; }
        public EventChatGroupMembershipDeleted? ChatGroupMembershipDeleted { get => _chatGroupMembershipDeleted; set => _chatGroupMembershipDeleted = value; }
        public UserStatus? UserStatus { get => _userStatus; set => _userStatus = value; }
    }
}
