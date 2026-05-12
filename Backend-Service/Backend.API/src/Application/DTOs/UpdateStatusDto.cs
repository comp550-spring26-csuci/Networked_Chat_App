// --------------------------------------------------------------------------
//  Project: Network Chat App
//  Engineer: Ivana Bavin-Gomez-San Basilio
//  Date: May 10th  2026
//  Description: Updates the users status
// ---------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using Backend.API.src.Core.Enums;

namespace Backend.API.src.Application.DTOs
{
    public class UpdateStatusDto
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public UserStateType NewStatus { get; set; }

        // Validation of length for the custom status

        [MaxLength(100, ErrorMessage = "Thr custom status exceeds the allowed length of 100 characters.")]
        [RegularExpression(@"^[^<>]*$", ErrorMessage = "The use of these special characters '<' and '>' is not allowed.")]
        public string? CustomText { get; set; }
    }
}
