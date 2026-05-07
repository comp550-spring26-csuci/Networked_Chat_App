// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ian Milin
//  Date: May 5 2026
//  Description: This DTO is used to represent a user's status in the context of testing. It contains a UserStatus object,
//  which provides information about the user's current status (e.g., online or offline), and an optional username. This
//  allows us to return user status information in a consistent way during testing, without having to include the entire
//  User entity.
// --------------------------------------------

namespace Backend.API.src.Application.DTOs.TestDTOs
{
    public class TestUserStatus
    {
        private UserStatus _userStatus = null!; 
        private string? _username;

        public UserStatus UserStatus
        {
            get => _userStatus;
            set => _userStatus = value;
        }

        public string? UserName
        {
            get => _username;
            set => _username = value;
        }
    }
}
