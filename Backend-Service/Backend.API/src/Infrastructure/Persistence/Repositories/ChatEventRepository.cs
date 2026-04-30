// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: April 6 2026
//  Description: Defines the class for the
//      essential data operation for Chathub events.
// --------------------------------------------

using Backend.API.src.Core.Entities;
using Backend.API.src.Core.Logging;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Backend.API.src.Infrastructure.Persistence.Repositories
{
    public class ChatEventRepository
    {
        private readonly MongoDbContext _context;

        public ChatEventRepository(MongoDbContext context)
        {
            _context = context;
        }

        // CRUD operations (create, read, update, delete)

        // CREATE
        public async Task AddAsync(ChatEvent chatEvent)
        {
            AppLogger.DebugState("ChatEventRepository", "Creating Event", chatEvent);

            await _context.ChatEvents.InsertOneAsync(chatEvent);
            AppLogger.DataStore("Insert", "ChatEvents", true);
        }

        public async Task<List<ChatEvent>> GetAllChatEventsAsync()
        {
            AppLogger.DebugState("ChatEventRepository", "Retrieving all chat events");
            return await _context.ChatEvents.Find(_ => true).ToListAsync();
        }

        public async Task<List<ChatEvent>> GetEventsByRoomIdAsync(Guid roomId)
        {
            AppLogger.DebugState("ChatEventRepository", $"Retrieving chat events for Room ID {roomId}");
            return await _context.ChatEvents.Find(e => e.ChatRoomId == roomId).ToListAsync();
        }
    }
}
