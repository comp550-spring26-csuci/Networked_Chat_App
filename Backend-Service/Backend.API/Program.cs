using Backend.API.src.Application.Validators;
using Backend.API.src.API.Hubs;
using Backend.API.src.Application.Services;
using Backend.API.src.Core.Interface;
using Backend.API.src.Infrastructure.Persistence;
using Backend.API.src.Infrastructure.Persistence.Repositories;
using Backend.API.src.Infrastructure.Persistence.Repositories.TestRepository;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Serilog;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;

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

                // --- 1. DEVELOPMENT TUNNEL CONFIGURATION ---
                var tunnelUrl = Environment.GetEnvironmentVariable("VS_TUNNEL_URL");
                if (!string.IsNullOrEmpty(tunnelUrl))
                {
                    Log.Information("Dev Tunnel Detected: {TunnelUrl}", tunnelUrl);
                    builder.WebHost.ConfigureKestrel(options =>
                    {
                        options.ConfigureEndpointDefaults(listenOptions =>
                        {
                            listenOptions.Protocols = HttpProtocols.Http1;
                        });
                    });
                }

                // --- 2. DATABASE CONFIGURATION ---
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

                builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDB"));
                builder.Services.AddSingleton<MongoDbContext>();
                BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));


                // --- 3. DEPENDENCY INJECTION ---

                // --- User Services
                builder.Services.AddScoped<IUserRepository, UserRepository>();
                builder.Services.AddScoped<IAuthService, AuthService>();
                builder.Services.AddValidatorsFromAssemblyContaining<CreateAccountRequestValidator>();

                // Messaging Services 
                builder.Services.AddScoped<MessageRepository>();
                builder.Services.AddScoped<ChatEventRepository>();
                builder.Services.AddScoped<TestChatRoomRepository>();
                builder.Services.AddSingleton<ClientPresenceService>();
                builder.Services.AddScoped<SignalRGroupService>();

                // --- COMPATIBILITY FIX ---
                var jwtSettings = builder.Configuration.GetSection(key: "JwtSettings").Get<JwtSettings>() ?? new JwtSettings();
                builder.Services.AddSingleton(Options.Create(jwtSettings));
                
                builder.Services.AddTransient<JwtTokenService>();

                // --- ---

                builder.Services.AddControllers();
                builder.Services.AddSignalR(options => { options.EnableDetailedErrors = true; })
                    .AddJsonProtocol(options => 
                    { 
                        options.PayloadSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase; 
                    });
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
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSettings.Key))
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
                builder.Services.AddAuthorization();

                var app = builder.Build();

                if (app.Environment.IsDevelopment()) { app.MapOpenApi(); }

                app.UseHttpsRedirection();
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