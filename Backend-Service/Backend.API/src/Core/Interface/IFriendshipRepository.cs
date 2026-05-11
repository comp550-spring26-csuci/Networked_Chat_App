// ----------------------------------------------------------------------------
//  Project: Network Chat App
//  Engineer: Ivana Bavin-Gomez-San Basilio
//  Date: March 7th 2026
//  Description: Defines the interface for the Friendship repository.
//		         It acts as the contract for DB operations.
// ----------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.API.src.Core.Entities;
using Backend.API.src.Application.DTOs;

namespace Backend.API.src.Core.Interface
{
    /// <summary>
    /// Interface for Friendship data operations
    /// </summary>
    public interface IFriendshipRepository
    {
        // Adds a new friendship record to the context
        Task AddAsync(Friendship friendship);

        // Checks if a friendship exists between two users
        Task<bool> ExistsAsync(Guid userId, Guid friendId);

        // Retrieves all friendships for a specific user
        Task<IEnumerable<Friendship>> GetUserFriendshipsAsync(Guid userId);

        // Retrieves the lists of friends mapped to DTOPs (Names + Ids)
        Task<IEnumerable<FriendUserDto>> GetFriendsByUserIdAsync(Guid userId);

        // Removes a friendship (Unfriend logic)
        Task DeleteAsync(Guid userId, Guid friendId);

        // Persists all changes to teh PostgreSQL database
        Task<bool> SaveChangesAsync();



    }
}
