// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: April 11 2026
//  Description: Defines the SendMessage DTO for sending messages in the network chat application.
// --------------------------------------------

namespace Backend.API.src.Application.DTOs
{
    public class SendMessage
    {
        public required string Content {  get; set; }
    }
}
