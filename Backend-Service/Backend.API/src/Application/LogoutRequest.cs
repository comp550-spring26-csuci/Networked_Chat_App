// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ivana Bavin-Gomez-San Basilio
//  Date: May 10th 2026
//  Description: DTO envelope for loging out
// --------------------------------------------


namespace Backend.API.src.Application
{
    public class LogoutRequest
    {
        public Guid UserId { get; set; }
    }
}
