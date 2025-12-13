using FluentValidation;
using TikTok.UserService.Application.DTOs;

namespace TikTok.UserService.Application.Validators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .Must(email => email != null && !string.IsNullOrWhiteSpace(email.Value))
                    .WithMessage("Email is required")
                .Must(email => email != null && email.Value.Length <= 255)
                    .WithMessage("Email cannot exceed 255 characters")
                .Must(email => email != null && System.Text.RegularExpressions.Regex.IsMatch(email.Value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    .WithMessage("Invalid email format");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters")
                .MaximumLength(100).WithMessage("Password cannot exceed 100 characters")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one number")
                .Matches(@"[\W_]").WithMessage("Password must contain at least one special character");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm password is required")
                .Equal(x => x.Password).WithMessage("Passwords do not match");

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required")
                .Must(username => username != null && !string.IsNullOrWhiteSpace(username.Value))
                    .WithMessage("Username is required")
                .Must(username => username != null && username.Value.Length <= 50)
                    .WithMessage("Username cannot exceed 50 characters")
                .Must(username => username != null && System.Text.RegularExpressions.Regex.IsMatch(username.Value, @"^[a-zA-Z0-9_]+$"))
                    .WithMessage("Username can only contain letters, numbers, and underscores");

            RuleFor(x => x.FullName)
                .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.FullName));

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required")
                .LessThan(DateTime.UtcNow.AddYears(-13)).WithMessage("You must be at least 13 years old")
                .GreaterThan(DateTime.UtcNow.AddYears(-120)).WithMessage("Invalid date of birth");

            RuleFor(x => x.Gender)
                .Must(g => Enum.IsDefined(typeof(Domain.Enums.Gender), g))
                .WithMessage("Invalid gender value");
        }
    }
}
