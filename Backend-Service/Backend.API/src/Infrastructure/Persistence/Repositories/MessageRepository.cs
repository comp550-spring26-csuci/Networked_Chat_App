// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: April 6 2026
//  Description: Defines the class for the
//      essential data operation for messages.
// --------------------------------------------

using Backend.API.src.Core.Entities;
using Backend.API.src.Core.Logging;

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
        }
    }
}
