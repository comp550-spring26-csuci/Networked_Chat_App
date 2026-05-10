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
using System.Collections.ObjectModel;


namespace Backend.API.src.Infrastructure.Persistence
{
    public enum ChatRoomEnum
    {
        none,
        ian_kenneth,
        ian_brielle,
        ian_ivana,
        kenneth_brielle,
        kenneth_ivana,
        brielle_ivana
    }

    public class MongoDbContext
    {
        public readonly IMongoCollection<Message> Messages;
        public readonly IMongoCollection<ChatEvent> ChatEvents;

        // public Dictionary<Guid, string> ChatRooms = new Dictionary<Guid, string>();

        public static Guid ToGuid(ChatRoomEnum chatRoomEnum)
        {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes((int) chatRoomEnum).CopyTo(bytes, 0);
            return new Guid(bytes);
        }

        private readonly Dictionary<Guid, string> _chatRooms = new()
        {
            { ToGuid(ChatRoomEnum.ian_kenneth),     "ian_kenneth" },
            { ToGuid(ChatRoomEnum.ian_brielle),     "ian_brielle" },
            { ToGuid(ChatRoomEnum.ian_ivana),       "ian_ivana" },
            { ToGuid(ChatRoomEnum.kenneth_brielle), "kenneth_brielle" },
            { ToGuid(ChatRoomEnum.kenneth_ivana),   "kenneth_ivana" },
            { ToGuid(ChatRoomEnum.brielle_ivana),   "brielle_ivana" }
        };

        public readonly ReadOnlyDictionary<Guid, string> ChatRooms;

        public MongoDbContext(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
            Messages = mongoDatabase.GetCollection<Message>(mongoDbSettings.Value.MessagesCollectionName);
            ChatEvents = mongoDatabase.GetCollection<ChatEvent>(mongoDbSettings.Value.EventsCollectionName);

            ChatRooms = new ReadOnlyDictionary<Guid, string>(_chatRooms);
        }

        public Guid NextChatRoomId()
        {
            return Guid.NewGuid();
        }
    }
}
