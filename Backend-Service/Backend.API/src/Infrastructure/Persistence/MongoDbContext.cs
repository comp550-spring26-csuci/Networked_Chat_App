// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: April 6 2026
//  Description: ChatHubContext class for MongoDB connection and operations.
//               Translates C# Entities into
//               MongoDB collections.
// --------------------------------------------

using Backend.API.src.Core.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;


namespace Backend.API.src.Infrastructure.Persistence
{
    public class MongoDbContext
    {
        public readonly IMongoCollection<Message> Messages;
        public readonly IMongoCollection<ChatEvent> ChatEvents;

        public Dictionary<Guid, string> ChatRooms = new Dictionary<Guid, string>();

        public MongoDbContext(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
            Messages = mongoDatabase.GetCollection<Message>(mongoDbSettings.Value.MessagesCollectionName);
            ChatEvents = mongoDatabase.GetCollection<ChatEvent>(mongoDbSettings.Value.EventsCollectionName);
        }

        public Guid NextChatRoomId()
        {
            return Guid.NewGuid();
        }
    }
}
