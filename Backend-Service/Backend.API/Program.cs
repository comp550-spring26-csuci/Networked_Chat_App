using Backend.API.src.Application.Validators;
using Backend.API.src.API.Hubs;
using Backend.API.src.Application.Services;
using Backend.API.src.Core.Interface;
using Backend.API.src.Infrastructure.Persistence;
using Backend.API.src.Infrastructure.Persistence.Repositories;
using Backend.API.src.Infrastructure.Persistence.Repositories.TestRepository;
using Backend.API.src.Infrastructure.Security;

using FluentValidation;
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
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/network-chat-log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                var builder = WebApplication.CreateBuilder(args);
                builder.Host.UseSerilog();

                // --- 2. DATABASE CONFIGURATION ---
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

                var mongoDbSettings = builder.Configuration.GetSection("MongoDB");
                builder.Services.Configure<MongoDbSettings>(mongoDbSettings);
                builder.Services.AddSingleton<MongoDbContext>();
                BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));


                // --- 3. DEPENDENCY INJECTION ---

                // --- User Services
                builder.Services.AddScoped<IUserRepository, UserRepository>();
                builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
                builder.Services.AddScoped<IAuthService, AuthService>();
                builder.Services.AddValidatorsFromAssemblyContaining<CreateAccountRequestValidator>();

                // --- Friendship Services
                builder.Services.AddScoped<IFriendshipRepository, FriendshipRepository>();

                // Messaging Services 
                builder.Services.AddScoped<MessageRepository>();
                builder.Services.AddScoped<ChatEventRepository>();
                builder.Services.AddScoped<TestChatRoomRepository>();

                // --- COMPATIBILITY FIX ---
                var jwtSection = builder.Configuration.GetSection("JwtSettings");
                var issuer = jwtSection["Issuer"] ?? "ChatApp";
                var audience = jwtSection["Audience"] ?? "ChatAppUsers";
                var jwtKeyString = jwtSection["Key"] ?? "SecretDevelopmentKey1234567890";

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKeyString));
                var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

                builder.Services.AddSingleton(signingCredentials);
                builder.Services.AddSingleton(issuer);
                builder.Services.AddSingleton(audience);

                builder.Services.AddTransient<JwtTokenService>(provider => new JwtTokenService(signingCredentials, issuer, audience));
                // --- ---

                builder.Services.AddControllers();
                builder.Services.AddSignalR(options => { options.EnableDetailedErrors = true; });
                builder.Services.AddOpenApi();

                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowEverything", policy =>
                    {
                        policy.SetIsOriginAllowed(_ => true)
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowCredentials();
                    });
                });

                // --- 5. JWT SECURITY DEFINITION ---
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
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = securityKey
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chathub"))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

                var app = builder.Build();

                if (app.Environment.IsDevelopment()) { app.MapOpenApi(); }

                //app.UseHttpsRedirection();
                app.UseCors("AllowEverything");

                app.UseWhen(context => context.Request.Path.StartsWithSegments("/chathub"), appBuilder =>
                {
                    appBuilder.UseAuthentication();
                    appBuilder.UseAuthorization();
                });

                app.MapControllers();
                app.MapHub<ChatHub>("/chathub");

                app.Run();
            }
            catch (Exception ex) { Log.Fatal(ex, "App failed to start"); }
            finally { Log.CloseAndFlush(); }
        }
    }
}