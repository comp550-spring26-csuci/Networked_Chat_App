// ----------------------------------------------------------------------
//  Project: Network Chat App
//  Engineer: Ivana Bavin-Gomez-San Basilio
//  Date: May 1 2026
//  Description: Implements the hasher for the user password.
// ----------------------------------------------------------------------

namespace Backend.API.src.Core.Interface
{
    public interface IPasswordHasher
    {

        // This will take a plain password and return a hashed string
        string HashPassword(string password);

        // This will compare a plain password against a hashed password
        bool VerifyPassword(string password, string hashedPassword);
    }
}
