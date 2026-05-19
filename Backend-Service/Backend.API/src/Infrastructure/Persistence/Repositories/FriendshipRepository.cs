// ----------------------------------------------------------------------------
//  Project: Network Chat App
//  Engineer: Ivana Bavin-Gomez-San Basilio
//  Date: March 7th 2026
//  Description: Defines the class for the
//		essential data operation for friendship relationships between users.
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.API.src.Core.Entities;
using Backend.API.src.Core.Interface;
using Backend.API.src.Infrastructure.Persistence;
using Backend.API.src.Application.DTOs;
using Microsoft.EntityFrameworkCore;




namespace Backend.API.src.Infrastructure.Persistence.Repositories
{
    public class FriendshipRepository : IFriendshipRepository
    {


        //----------------------------------
        //-------  Private Fields ----------
        //----------------------------------
        private readonly AppDbContext _context;




        //----------------------------------
        //--------  Constructors -----------
        //----------------------------------
        public FriendshipRepository(AppDbContext context) 
        {
            _context = context;
        }



        //----------------------------------
        //-----------  Methods -------------
        //----------------------------------

        /// <summary>
        /// Adds a new friendship record to the context
        /// </summary>
        /// <param name="friendship"></param>
        /// <returns></returns>
        public async Task AddAsync(Friendship friendship)
        {

            await _context.Friendships.AddAsync(friendship);
        
        }

        /// <summary>
        /// Checks if a friendship exists between two users
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="friendId"></param>
        /// <returns></returns>
        public async Task<bool> ExistsAsync(Guid userId, Guid friendId)
        {

            // Checks if User A added B Or User B added A 
            return await _context.Friendships
                .AnyAsync(f => (f.UserId == userId && f.FriendId == friendId) ||
                                (f.UserId == friendId && f.FriendId == userId));

        }

        /// <summary>
        /// Retrieves all friendships for a specific user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Friendship>> GetUserFriendshipsAsync(Guid userId)
        {

            return await _context.Friendships
                .Where(f => f.UserId == userId || f.FriendId == userId)
                .ToListAsync();

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<FriendUserDto>> GetFriendsByUserIdAsync(Guid userId)
        {
            // 1. We search for the friendships including the data of the user to have the Usernames
            var friendships = await _context.Friendships
                .Include(f => f.User)       // User that started the relationship
                .Include(f => f.Friend) // The user that received it
                .Where(f => f.UserId == userId || f.FriendId == userId)
                .ToListAsync();

            // 2. We map the FriendUserDtp deciding who is the friend according to the Id
            return friendships.Select(f => new FriendUserDto
            {
                // If the ID inquired is the UserId, the friend is the FriendId
                Id = f.UserId == userId ? f.FriendId : f.UserId,

                // We do teh same with teh Username
                Username = f.UserId == userId ? f.Friend.Username : f.User.Username


            });

        }


        /// <summary>
        /// Removes a friendship (Unfriend logic)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="friendId"></param>
        /// <returns></returns>
        public async Task DeleteAsync(Guid userId, Guid friendId)
        {
            var friendship = await _context.Friendships
                    .FirstOrDefaultAsync(f => (f.UserId == userId && f.FriendId == friendId) ||
                                              (f.UserId == friendId && f.FriendId == userId));

            if (friendship != null)
            {
            
                    _context.Friendships.Remove(friendship);
            
            }


        }

        /// <summary>
        /// Persists all changes to the PostgreSQL database
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SaveChangesAsync()
        {

            // This calls the custom SaveChangesAsync in AppDbContext with the logger
            return await _context.SaveChangesAsync() > 0;

        }



    }
}
