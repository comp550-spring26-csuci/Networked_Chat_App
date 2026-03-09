// -------------------------------------------------------------------
//  Project: Network Chat App
//  Engineer: Ivana Bavin-Gomez-San Basilio
//  Date: March 7th 2026
//  Description: Defines the interface for the
//		essential data operation for users.
// -------------------------------------------------------------------


using System;
using Backend.API.src.Core.Entities;
using System.Threading.Tasks;

namespace Backend.API.src.Core.Interface
{

    /// <summary>
    /// Defines how the applicaiton interacts with User data
    /// </summary>
    public interface IUserRepository
    {
        // CRUD operations (create, read, update, delete)

        // CREATE
        Task AddAsync(User user);

        // READ
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllAsync();

        // UPDATE
        void Update(User user);

        // DELETE
        void Delete(User user);

        // COMMIT (saving it to the physical DB)
        Task<bool> SaveChangesAsync();


    }

}
