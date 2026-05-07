// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: April 6 2026
//  Description: Defines the class for the
//      essential data operation for messages.
// --------------------------------------------

using Backend.API.src.Core.Entities;
using Backend.API.src.Core.Logging;
using MongoDB.Driver;

namespace Backend.API.src.Infrastructure.Persistence.Repositories
{
    public class MessageRepository
    {
        private readonly MongoDbContext _context;

        public MessageRepository(MongoDbContext context)
        {
            _context = context;
        }

        // CRUD operations (create, read, update, delete)

        // CREATE
        public async Task AddAsync(Message message)
        {
            AppLogger.DebugState("MessageRepository", "Creating Message");
            AppLogger.DebugState("MessageRepository", $"Adding new message with Sender ID {message.SenderId} to Room ID {message.ChatRoomId}");

            await _context.Messages.InsertOneAsync(message);
            AppLogger.DataStore("Insert", "Messages", true);
        }

        public async Task DeleteAsync(Message message)
        {
            AppLogger.DebugState("MessageRepository", "Removing Message");
            AppLogger.DebugState("MessageRepository", $"Removing message with Sender ID {message.SenderId} from Room ID {message.ChatRoomId}");

            await _context.Messages.DeleteOneAsync(m => m.Id == message.Id);
            AppLogger.DataStore("Delete", "Messages", true);
        }

        public async Task DeleteMessagesByRoomIdAsync(Guid roomId)
        {
            AppLogger.DebugState("MessageRepository", $"Removing all messages for Room ID {roomId}");
            await _context.Messages.DeleteManyAsync(m => m.ChatRoomId == roomId);
            AppLogger.DataStore("Delete", "Messages", true);
        }

        public async Task<List<Message>> GetAllMessagesAsync()
        {
            AppLogger.DebugState("MessageRepository", "Retrieving all messages");
            return await _context.Messages.Find(_ => true).ToListAsync();
        }

        public async Task<List<Message>> GetMessagesByRoomIdAsync(Guid roomId)
        {
            AppLogger.DebugState("MessageRepository", $"Retrieving messages for Room ID {roomId}");
            return await _context.Messages.Find(m => m.ChatRoomId == roomId).ToListAsync();
        }

        public async Task<Message?> GetMessageByIdAsync(string id)
        {
            AppLogger.DebugState("MessageRepository", $"Retrieving message with ID {id}");
            return await _context.Messages.Find(m => m.Id == id).FirstOrDefaultAsync();
        }

    }
}
