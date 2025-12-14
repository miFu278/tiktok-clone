using FluentValidation;
using TikTok.UserService.Application.DTOs;

namespace TikTok.UserService.Application.Validators
{
    public class VerifyEmailDtoValidator : AbstractValidator<VerifyEmailDto>
    {
        public VerifyEmailDtoValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Verification token is required");
        }
    }
}
