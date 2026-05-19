// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: May 16, 2026
//  Description: This file defines the ChatGroupDto class, which is a Data Transfer Object (DTO) used to represent chat groups in the
//  application. The class contains properties for the group's ID, name, creation timestamp, creator's user ID, and the count of
//  unread messages. The ChatGroupDto class also includes a static method to create an instance of ChatGroupDto from a ChatGroup entity,
//  allowing for easy conversion between the domain model and the DTO used for communication with clients. This DTO is used to
//  send chat group information to clients in real-time updates or API responses.
// --------------------------------------------

using Backend.API.src.Core.Entities;

namespace Backend.API.src.Application.DTOs
{
    public class ChatGroupDto
    {
        private Guid _id;
        private string? _name;
        private DateTime _createdAt;
        private Guid _createdByUserId;
        private int _unreadCount; 

        public Guid Id { get => _id; set => _id = value; }
        public string? Name { get => _name; set => _name = value; }
        public DateTime CreatedAt { get => _createdAt; set => _createdAt = value; }
        public Guid CreatedByUserId { get => _createdByUserId; set => _createdByUserId = value; }
        public int UnreadCount { get => _unreadCount; set => _unreadCount = value; }

        public static ChatGroupDto FromEntity(ChatGroup chatGroup, int unreadCount = 0) => new()
        {
            Id = chatGroup.Id,
            Name = chatGroup.Name,
            CreatedAt = chatGroup.CreatedAt,
            CreatedByUserId = chatGroup.CreatedByUserId,
            UnreadCount = unreadCount
        };
    }
}
