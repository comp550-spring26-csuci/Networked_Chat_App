using Backend.API.src.API.Hubs;
using Backend.API.src.Application.Services;
using Backend.API.src.Core.Interface;
using Backend.API.src.Infrastructure.Persistence;
using Backend.API.src.Infrastructure.Persistence.Repositories;
using Backend.API.src.Infrastructure.Persistence.Repositories.TestRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Serilog;
using System.Text;

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

                var mongoDbSettings = builder.Configuration.GetSection("MongoDB"); 

                Console.WriteLine($"---> MONGODB CONNECTION STRING: '{mongoDbSettings["ConnectionString"]}'");

                builder.Services.Configure<MongoDbSettings>(mongoDbSettings);
                builder.Services.AddSingleton<MongoDbContext>();

                BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

                // Adding the IUserRepository and UserRepository
                builder.Services.AddScoped<IUserRepository, UserRepository>();

                builder.Services.AddScoped<MessageRepository>();

                builder.Services.AddScoped<ChatEventRepository>();

                builder.Services.AddScoped<TestChatRoomRepository>();

                builder.Services.AddTransient<JwtTokenService>();

                // Add services to the container.
                builder.Services.AddControllers();
                builder.Services.AddSignalR(options =>
                {
                    options.EnableDetailedErrors = true;
                });

                // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
                // Prepares the map
                builder.Services.AddOpenApi();

                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowFrontend", builder =>
                    {
                        builder.SetIsOriginAllowed(origin => true) // Effectively allows any origin
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowCredentials();
                    });
                });

                // JWT Auth Setup
                var jwtKey = builder.Configuration.GetSection("JwtSettings:SecretKey").Value ?? "SecretDevelopmentKeyWithPlentyOfBits1234567890";
                var key = Encoding.UTF8.GetBytes(jwtKey!);

                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration.GetSection("JwtSettings:Issuer").Value,
                        ValidAudience = builder.Configuration.GetSection("JwtSettings:Audience").Value,
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };

                    // For SignalR authentication
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            // If the request is for our hub...
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chathub"))
                            {
                                // Read the token out of the query string
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
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

                app.UseCors("AllowFrontend");


                app.UseAuthorization();

                // Added route to connect to a hub
                app.MapHub<ChatHub>("/chathub");

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
