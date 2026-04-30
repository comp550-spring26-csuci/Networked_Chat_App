// -------------------------------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: April 26 2026
//  Description: Service for generating JWT tokens for user authentication
// -------------------------------------------------------------------

using Backend.API.src.Core.Entities;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Backend.API.src.Application.Services
{
    public class JwtTokenService
    {
        private readonly SigningCredentials _signingCredentials;
        private readonly string _issuer;
        private readonly string _audience;

        public JwtTokenService(SigningCredentials signingCredentials, string issuer, string audience)
        {
            _signingCredentials = signingCredentials;
            _issuer = issuer;
            _audience = audience;
        }

        public string GenerateToken(User user, TimeSpan expiration)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(expiration),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = _signingCredentials
            };
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
