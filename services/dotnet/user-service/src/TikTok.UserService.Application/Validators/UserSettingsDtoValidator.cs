using FluentValidation;
using TikTok.UserService.Application.DTOs;

namespace TikTok.UserService.Application.Validators
{
    public class UserSettingsDtoValidator : AbstractValidator<UserSettingsDto>
    {
        public UserSettingsDtoValidator()
        {
            RuleFor(x => x.PreferredLanguage)
                .NotEmpty().WithMessage("Preferred language is required")
                .MaximumLength(10).WithMessage("Language code cannot exceed 10 characters")
                .Matches(@"^[a-z]{2}(-[A-Z]{2})?$").WithMessage("Invalid language code format (e.g., 'en' or 'en-US')");

            RuleFor(x => x.PreferredContentRegion)
                .NotEmpty().WithMessage("Preferred content region is required")
                .MaximumLength(10).WithMessage("Region code cannot exceed 10 characters")
                .Matches(@"^[A-Z]{2}$").WithMessage("Invalid region code format (e.g., 'US', 'VN')");
        }
    }
}
