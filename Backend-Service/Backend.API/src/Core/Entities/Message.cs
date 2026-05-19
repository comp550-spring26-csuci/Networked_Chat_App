// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: April 5 2026
//  Description: Defines the Message entity.
//               This will match the "Messages" collection
// --------------------------------------------

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Backend.API.src.Core.Entities
{
    public class Message
    {
        private string? _id;
        private Guid _senderId;
        private string? _senderUsername;
        private Guid _chatRoomId;
        private string _content = default!;
        private DateTime _timestamp = DateTime.UtcNow;

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get => _id; set => _id = value; }
        [BsonRepresentation(BsonType.String)]
        public Guid SenderId { get => _senderId; set => _senderId = value; }
        public string? SenderUsername { get => _senderUsername; set => _senderUsername = value; }
        [BsonRepresentation(BsonType.String)]
        public Guid ChatRoomId { get => _chatRoomId; set => _chatRoomId = value; }
        public required string Content { get => _content; set => _content = value; }
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime Timestamp { get => _timestamp; set => _timestamp = value; }
    }
}
