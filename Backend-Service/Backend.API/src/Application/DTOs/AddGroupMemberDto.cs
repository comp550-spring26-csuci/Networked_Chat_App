// -----------------------------------------------------
//  Project: Network Chat App
//  Engineer: Ivana Bavin-Gomez-San Basilio
//  Date: May 11th 2026
//  Description: DTO for adding a user to a Chat Group
// -----------------------------------------------------


using System.ComponentModel.DataAnnotations;

namespace Backend.API.src.Application.DTOs
{
    public class AddGroupMemberDto
    {

        [Required]
        public Guid ChatGroupId { get; set; }

        [Required]
        public Guid RequesterId { get; set; } // Person doing the adding

        [Required]
        public Guid TargetUserId { get; set; } // The friend being added

    }
}
