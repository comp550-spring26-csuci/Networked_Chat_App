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
