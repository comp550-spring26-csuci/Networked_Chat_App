// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: April 5 2026
//  Description: Defines the ChatEvent entity.
//               This will match the "Events" collection
// --------------------------------------------

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Backend.API.src.Core.Entities
{
    public class ChatEvent
    {
        private string? _id;
        private Guid _chatRoomId;
        private string _eventType = default!;
        private string _details = default!;
        private readonly DateTime _timestamp = DateTime.UtcNow;


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

        public required string EventType
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
        }
    }
}
