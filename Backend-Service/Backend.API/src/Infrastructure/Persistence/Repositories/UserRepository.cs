// -------------------------------------------------------------------
//  Project: Network Chat App
//  Engineer: Ivana Bavin-Gomez-San Basilio
//  Date: March 7th 2026
//  Description: Defines the class for the
//		essential data operation for users.
// -------------------------------------------------------------------

using Backend.API.src.Core.Interface;
using Backend.API.src.Core.Entities;
using Backend.API.src.Core.Logging;
using Microsoft.EntityFrameworkCore;
using Backend.API.src.Infrastructure.Persistence;

namespace Backend.API.src.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        { 
            _context = context;
        }


        // CRUD operations (create, read, update, delete)

        // CREATE
        public async Task AddAsync(User user)
        {
            // Logging the create user
            AppLogger.DebugState("UserRepository", "Creating User");
            AppLogger.DebugState("UserRepository", $"Adding new user: {user.Username}");
            await _context.Users.AddAsync(user);

        }

        // READ: Find by ID
        public async Task<User?> GetByIdAsync(Guid id) 
        {
            AppLogger.DebugState("UserRepository", "Find User by ID");
            return await _context.Users.FindAsync(id);
        }

        // READ: Find by Email (used for login/auth)
        public async Task<User?> GetByEmailAsync(string email)
        {

            AppLogger.DebugState("UserRepository", "Find User by email");
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }


        // READ: Get all users
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            AppLogger.DebugState("UserRepository", "Find all the users");
            return await _context.Users.ToListAsync();
        }


        // UPDATE: Existing user change
        public void Update(User user)
        {
            AppLogger.DebugState("UserRepository", "Updating User");
            _context.Users.Update(user);
            AppLogger.DebugState("UserRepository", $"User marked for update: {user.Username}");

        }

        // DELETE: Removing a user
        public void Delete(User user)
        {
            AppLogger.DebugState("UserRepository", "Removing User");
            _context.Users.Remove(user);
            AppLogger.UserAction(user.Id.ToString(), "Account deleted");
        }


        // COMMIT: Executing the save changes
        public async Task<bool> SaveChangesAsync()
        {
            AppLogger.DebugState("UserRepository", "Saving Changes");
            // This calls the overridden SaveChangesAsync in AppDbContext
            return await _context.SaveChangesAsync() > 0;

        }


    }



}