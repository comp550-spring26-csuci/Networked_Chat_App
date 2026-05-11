// ----------------------------------------------------------------------
//  Project: Network Chat App
//  Engineer: Ivana Bavin-Gomez-San Basilio
//  Date: May 1 2026
//  Description: Implements the hasher for the user password.
// ----------------------------------------------------------------------

using BCrypt.Net;
using Backend.API.src.Core.Interface;


namespace Backend.API.src.Infrastructure.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        // Creating the one-way fingerprint of the password
        public string HashPassword(string password)
        {
            // The default work factor is 11, which works for msot apps
            return BCrypt.Net.BCrypt.HashPassword(password);

        }

        // Checking if the login attempt matches the stored fingerprint
        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

    }
}
