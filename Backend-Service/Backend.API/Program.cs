using Microsoft.EntityFrameworkCore;
using Backend.API.src.Infrastructure.Persistence;


namespace Backend.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // Fetching the map from appsettings.json
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // Directions to use PostgreSQL and your AppDbContext
            builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

            // Add services to the container.
            builder.Services.AddControllers();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();


            app.UseAuthorization();


            app.MapControllers();


            app.Run();
        }
    }
}
