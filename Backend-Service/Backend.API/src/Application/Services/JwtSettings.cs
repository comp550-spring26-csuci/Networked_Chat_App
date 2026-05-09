namespace Backend.API.src.Application.Services
{
    public class JwtSettings
    {
        public string Issuer { get; set; } = "ChatApp";
        public string Audience { get; set; } = "ChatAppUsers";
        public string Key { get; set; } = "SecretDevelopmentKey1234567890";
    }
}
