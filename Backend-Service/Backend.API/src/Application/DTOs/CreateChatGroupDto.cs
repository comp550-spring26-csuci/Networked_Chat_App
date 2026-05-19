// -----------------------------------------------------
//  Project: Network Chat App
//  Engineer: Ivana Bavin-Gomez-San Basilio
//  Date: May 11th 2026
//  Description: DTO for creating a new Chat Group
// -----------------------------------------------------


using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.API.src.Application.DTOs
{
    public class CreateChatGroupDto
    {

        [Required]
        [MaxLength(100, ErrorMessage = "group name cannot exceed 100 characters.")]
        public string GroupName { get; set; } = string.Empty;

        [Required]
        public Guid CreatorUserId { get; set; }
    }
}
