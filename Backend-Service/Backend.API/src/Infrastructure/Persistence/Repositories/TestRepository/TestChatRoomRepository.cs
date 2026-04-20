// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: April 6 2026
//  Description: Defines the TestChatRoomRepository class for managing chat rooms in a test environment using an in-memory dictionary.
// --------------------------------------------


using System.Collections.ObjectModel;

namespace Backend.API.src.Infrastructure.Persistence.Repositories.TestRepository
{
    public class TestChatRoomRepository
    {
        private readonly MongoDbContext _context;

        public TestChatRoomRepository(MongoDbContext context)
        {
            _context = context;
        }

        //public Guid AddChatRoom(string roomName)
        //{
        //    Guid guid = _context.NextChatRoomId();
        //    _context.ChatRooms[guid] = roomName;
        //    return guid;
        //}

        public string? GetChatRoomName(Guid roomId)
        {
            if (_context.ChatRooms.TryGetValue(roomId, out var roomName))
            {
                return roomName;
            }
            return null; // Room not found
        }

        public Guid? GetChatRoomId(string roomName)
        {
            var room = _context.ChatRooms.FirstOrDefault(r => r.Value == roomName);
            if (!room.Equals(default(KeyValuePair<Guid, string>)))
            {
                return room.Key;
            }
            return null; // Room not found
        }

        //public void RemoveChatRoom(Guid roomId)
        //{
        //    _context.ChatRooms.Remove(roomId);
        //}

        public ReadOnlyDictionary<Guid, string> GetAllChatRooms()
        {
            return _context.ChatRooms;
        }

        public bool ChatRoomExists(Guid roomId)
        {
            return _context.ChatRooms.ContainsKey(roomId);
        }

        //public void ClearChatRooms()
        //{
        //    _context.ChatRooms.Clear();
        //}

        public int GetChatRoomCount()
        {
            return _context.ChatRooms.Count;
        }

        public List<string> GetChatRoomNames()
        {
            return _context.ChatRooms.Values.ToList();
        }

        public List<Guid> GetChatRoomIds()
        {
            return _context.ChatRooms.Keys.ToList();
        }

        // This method is a bit more complex and is based on the assumption that the room names contain the usernames of the participants.
        // It is very improper and is only meant for testing purposes with the specific room names defined in the MongoDbContext.
        public Dictionary<Guid, string> GetChatRoomsByUsername(string name)
        {
            return _context.ChatRooms.Where(r => r.Value.Contains(name)).ToDictionary(r => r.Key, r => r.Value);
        }
    }
}
