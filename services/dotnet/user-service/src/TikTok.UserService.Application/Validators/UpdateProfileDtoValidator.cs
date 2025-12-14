using FluentValidation;
using TikTok.UserService.Application.DTOs;

namespace TikTok.UserService.Application.Validators
{
    public class UpdateProfileDtoValidator : AbstractValidator<UpdateProfileDto>
    {
        public UpdateProfileDtoValidator()
        {
            RuleFor(x => x.UserName)
                .MinimumLength(3).WithMessage("Username must be at least 3 characters")
                .MaximumLength(30).WithMessage("Username cannot exceed 30 characters")
                .Matches(@"^[a-zA-Z0-9._]+$").WithMessage("Username can only contain letters, numbers, dots and underscores")
                .When(x => !string.IsNullOrEmpty(x.UserName));

            RuleFor(x => x.FullName)
                .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.FullName));

            RuleFor(x => x.Bio)
                .MaximumLength(500).WithMessage("Bio cannot exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Bio));

            RuleFor(x => x.AvatarUrl)
                .MaximumLength(500).WithMessage("Avatar URL cannot exceed 500 characters")
                .Must(BeAValidUrl).WithMessage("Invalid avatar URL")
                .When(x => !string.IsNullOrEmpty(x.AvatarUrl));

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateTime.UtcNow.AddYears(-13)).WithMessage("You must be at least 13 years old")
                .GreaterThan(DateTime.UtcNow.AddYears(-120)).WithMessage("Invalid date of birth")
                .When(x => x.DateOfBirth.HasValue);

            RuleFor(x => x.Gender)
                .Must(g => g.HasValue && Enum.IsDefined(typeof(Domain.Enums.Gender), g.Value))
                .WithMessage("Invalid gender value")
                .When(x => x.Gender.HasValue);
        }

        private bool BeAValidUrl(string? url)
        {
            if (string.IsNullOrEmpty(url))
                return true;

            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
