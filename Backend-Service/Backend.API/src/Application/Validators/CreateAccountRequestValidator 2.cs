// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ivana Bavin-Gomez-San Basilio
//  Date: April 5 2026
//  Description: Validates the incoming CreateAccountRequest data.
// --------------------------------------------

using System;
using FluentValidation;
using Backend.API.src.Application.DTOs;

namespace Backend.API.src.Application.Validators
{
    /// <summary>
    ///  Enforces the cosmic boundaries for creating a new user account
    /// </summary>
    public class CreateAccountRequestValidator : AbstractValidator<CreateAccountRequest>
    { 
        public CreateAccountRequestValidator() 
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.")
                .MinimumLength(3).WithMessage("Username must be at least 3 characters long.")
                .MaximumLength(50).WithMessage("Username cannot exceed 50 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email address is required.")
                .EmailAddress().WithMessage("A valid email address is required.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(3).WithMessage("Password must be at least 8 characters long.");

            RuleFor(x => x.Age)
                .GreaterThanOrEqualTo(18).WithMessage("User must be at least 18 years old to join this realm.");


        }
    
    }

}


