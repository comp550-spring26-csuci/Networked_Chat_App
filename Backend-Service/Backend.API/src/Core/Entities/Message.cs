using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Backend.API.src.Core.Entities
{
    public class Message
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Username { get; set; } = null!;

        public string Room { get; set; } = null!;

        public string Content { get; set; } = null!;

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime Timestamp { get; set; }
    }
}
