// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ivana Bavin-Gomez-San Basilio
//  Date: March 6 2026
//  Description: Validator for the LoginRequest DTO.
//               Ensures credentials are not empty before processing.
// --------------------------------------------

using FluentValidation;
using Backend.API.src.Application.DTOs;


namespace Backend.API.src.Application.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {

        public LoginRequestValidator()
        {
            // Rule for the user
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required to log in.");

            // Rule for the password
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required to log in.");


        }
    }
}
