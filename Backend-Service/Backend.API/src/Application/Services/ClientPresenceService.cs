// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: May 7 2026
//  Description: Detect when a user is active by tracking all of their client connections. Access these connections is helpful when
//  updating a user's clients' streams after a room change.
//  -------------------------------------------

using System.Collections.Concurrent;

namespace Backend.API.src.Application.Services
{
    public class ClientPresenceService
    {
        private readonly ConcurrentDictionary<Guid, List<string>> _userSessions = new();

        // Called when a user connects to the server. 
        public Task UserSessionStarted(Guid userId, string connectionId)
        {
            // Add or update the user's connections. If the user is new, create a new list with the connection ID.
            _userSessions.AddOrUpdate(userId,
                key => [connectionId],
                (key, connections) =>
                {
                    lock (connections)
                    {
                        connections.Add(connectionId);
                    }
                    return connections;
                }); 

            return Task.CompletedTask;
        }

        // Called when a user disconnects from the server. 
        public Task UserSessionEnded(Guid userId, string connectionId)
        {
            // Try to get the user's connections. If found, remove the connection ID and check
            // if the list is empty to determine if the session has ended.
            if (_userSessions.TryGetValue(userId, out List<string>? connections))
            {
                lock (connections)
                {
                    connections.Remove(connectionId);
                    if (connections.Count == 0)
                    {
                        _userSessions.TryRemove(userId, out _);
                    }
                }
            }

            return Task.CompletedTask;
        }

        // Retrieves the list of active connection IDs for a given user. If the user has no active connections, returns an empty list.
        // This method is useful for determining which client connections to
        // update when a user's session state changes (e.g., joining/leaving a room).
        public Task<IEnumerable<string>> GetUserConnections(Guid userId)
        {
            if (_userSessions.TryGetValue(userId, out List<string>? connections))
            {
                lock (connections)
                {
                    return Task.FromResult<IEnumerable<string>>(connections.ToList());
                }
            }
            return Task.FromResult<IEnumerable<string>>([]);
        }
    }
}
