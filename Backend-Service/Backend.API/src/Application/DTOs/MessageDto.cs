// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: May 17 2026
//  Description: Defines the Message DTO for data transfer between layers.
// --------------------------------------------

using Backend.API.src.Core.Entities;

namespace Backend.API.src.Application.DTOs
{
    public class MessageDto
    {
        private string? _id;
        private Guid _senderId;
        private string? _senderUsername;
        private Guid _chatRoomId;
        private string _content = default!;
        private DateTime _timestamp = DateTime.UtcNow;

        public string? Id { get => _id; set => _id = value; }
        public Guid SenderId { get => _senderId; set => _senderId = value; }
        public string? SenderUsername { get => _senderUsername; set => _senderUsername = value; }
        public Guid ChatRoomId { get => _chatRoomId; set => _chatRoomId = value; }
        public required string Content { get => _content; set => _content = value; }
        public DateTime Timestamp { get => _timestamp; set => _timestamp = value; }

        public static MessageDto FromEntity(Message message) => new()
        {
            Id = message.Id,
            SenderId = message.SenderId,
            SenderUsername = message.SenderUsername,
            ChatRoomId = message.ChatRoomId,
            Content = message.Content,
            Timestamp = message.Timestamp
        };
    }
}
