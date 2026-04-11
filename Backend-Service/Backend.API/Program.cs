using Microsoft.EntityFrameworkCore;
using Backend.API.src.Infrastructure.Persistence;
using Backend.API.src.Core.Interface;
using Backend.API.src.Infrastructure.Persistence.Repositories;
using Serilog;
using FluentValidation;
using Backend.API.src.Application.Validators;
using Backend.API.src.Application.Services;


namespace Backend.API
{
    public class Program
    {
        public static void Main(string[] args)
        {

            // Initiating the Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug() // allowing debug level
                .WriteTo.Console() //logging to terminal
                .WriteTo.File("logs/network-chat-log.txt", rollingInterval: RollingInterval.Day) // log to file
                .CreateLogger();

            try
            {

                var builder = WebApplication.CreateBuilder(args);

                builder.Host.UseSerilog();


                // Fetching the map from appsettings.json
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                Console.WriteLine($"---> DATABASE CONNECTION STRING: '{connectionString}'");

                // Directions to use PostgreSQL and your AppDbContext
                builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

                // Adding the IUserRepository and UserRepository
                builder.Services.AddScoped<IUserRepository, UserRepository>();

                // Adding the AuthService
                builder.Services.AddScoped<IAuthService, AuthService>();

                // Adding the Validator to the Application Container
                // It scans the folder and registers the CreateAccountValidator
                builder.Services.AddValidatorsFromAssemblyContaining<CreateAccountRequestValidator>();

                // Add services to the container.
                builder.Services.AddControllers();

                // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
                // Prepares the map
                builder.Services.AddOpenApi();

                // Connecting with front end
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowFrontend", Policy =>
                    {
                        Policy.AllowAnyOrigin()
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });

                });

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    // Creates the JSON file
                    app.MapOpenApi();

                }

                app.UseHttpsRedirection();


                app.UseCors("AllowFrontend");


                app.UseAuthorization();


                app.MapControllers();


                app.Run();

            }

            catch (Exception ex)
            {

                Log.Fatal(ex, "There was an issue starting the application");

            }

            finally
            {
                // all logs will be written before the app closes
                Log.CloseAndFlush();

            }

        }
    }
}
