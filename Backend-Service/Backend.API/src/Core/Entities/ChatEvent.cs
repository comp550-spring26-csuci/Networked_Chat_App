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

namespace Backend.API.src.Core.Entities
{
    public enum ChatEventType
    {
        UserJoined,
        UserLeft,
        MembershipAdded,
        MembershipRemoved,
        RoomDeleted,
        FriendRequestReceived,
        FriendRequestAccepted
    }

    public class ChatEvent
    {
        private string? _id;
        private Guid _chatRoomId;
        private ChatEventType _eventType;
        private string _details = default!;
        private DateTime _timestamp = DateTime.UtcNow;
        private EventChatRoom? _room;
        private EventFriendRequest? _request;

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id
        {
            get => _id;
            set => _id = value;
        }

        public Guid ChatRoomId
        {
            get => _chatRoomId;
            set => _chatRoomId = value;
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public required ChatEventType EventType
        {
            get => _eventType;
            set => _eventType = value;
        }

        public string Details
        {
            get => _details;
            set => _details = value;
        }

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime Timestamp
        {
            get => _timestamp;
            set => _timestamp = value;
        }

        public EventChatRoom? Room
        {
            get => _room;
            set => _room = value;
        }

        public EventFriendRequest? Request
        {
            get => _request;
            set => _request = value;
        }
    }
}
