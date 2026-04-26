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
        /// IOt saves the changes
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
                throw; // rethrowing so that the repo knowsn it failed

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Logging the state change
            AppLogger.DebugState("Database", "Configuring Model Constraints (Indexes))");


            // We enforce tha the Usernames and emails must be  in the database
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();


            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

        }

    }

}
