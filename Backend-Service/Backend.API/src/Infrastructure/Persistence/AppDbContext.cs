// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ivana Bavin-Gomez-San Basilio
//  Date: March 6 2026
//  Description: Database context.
//               Translates C# Entities into 
//               PostgreSQL tables
// --------------------------------------------

using Backend.API.src.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Backend.API.src.Core.Logging;

namespace Backend.API.src.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Constructor that passes conneciton settings to base factory
        /// </summary>
        /// <param name="options"></param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            // Tracking when a new context is crated
            // Logging the state change
            AppLogger.DebugState("Database", "AppDbContext initialized");

        }

        //---------Tables -----------

        /// <summary>
        /// Making a table from the user class
        /// </summary>
        public DbSet<User> Users { get; set; }


        /// <summary>
        /// table for managing user relationship
        /// </summary>
        public DbSet<Friendship> Friendships { get; set; }


        /// <summary>
        /// It saves the changes
        /// </summary>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await base.SaveChangesAsync(cancellationToken);
                AppLogger.DataStore("Commit", "PostgreSQL", true);
                return result;

            }
            catch (Exception ex)
            {
                AppLogger.ShieldFailure("DatabaseSave", ex);
                throw; // rethrowing so that the repo knows it failed

            }
        }

        /// <summary>
        /// Configuring the relationships and constrains between tables
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Logging the state change
            AppLogger.DebugState("Database", "Configuring Model Constraints (Indexes)");

            // --- User Table Constraints ---
            // We enforce that the Usernames and emails must be  in the database
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();


            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();


            // --- Friendship Table Configuration ---
            // We enforce tha the Usernames and emails must be  in the database
            modelBuilder.Entity<Friendship>(entity =>
            {
                // Defining the Primary Key
                entity.HasKey(f => f.Id);

                // Relationship: Configuring the 'UserId' (The Sender)
                entity.HasOne(f => f.User)
                .WithMany() // A user can have many friendship records
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Delete friendship if user is deleted

                // Relationship: Configuring the 'FriendId' (The Receiver)
                entity.HasOne(f => f.Friend)
                .WithMany() 
                .HasForeignKey(f => f.FriendId)
                .OnDelete(DeleteBehavior.Cascade); 

                // Business Rule: Ensure User A cannot add User B more than once
                entity.HasIndex(f => new {  f.UserId, f.FriendId})
                .IsUnique();



            });

        }

    }

}
