// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: April 6 2026
//  Description: Defines the class for configuring
//               MongoDB settings.
// --------------------------------------------

namespace Backend.API.src.Infrastructure.Persistence
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string MessagesCollectionName { get; set; } = null!;
        public string EventsCollectionName { get; set; } = null!;
    }
}
