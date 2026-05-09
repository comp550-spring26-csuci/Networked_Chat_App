// -------------------------------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: April 26 2026
//  Description: Service for generating JWT tokens for user authentication
// -------------------------------------------------------------------

using Backend.API.src.Core.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Backend.API.src.Application.Services
{
    public class JwtTokenService
    {
        private readonly JwtSettings _settings;
        private readonly SigningCredentials _signingCredentials;

        public JwtTokenService(IOptions<JwtSettings> options)
        {
            _settings = options.Value;
            _signingCredentials = new(
                new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_settings.Key)),
                SecurityAlgorithms.HmacSha256);
        }

        public string GenerateToken(User user, TimeSpan expiration)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(expiration),
                signingCredentials: _signingCredentials
            );

            return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
