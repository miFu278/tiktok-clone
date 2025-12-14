using FluentValidation;
using TikTok.UserService.Application.DTOs;

namespace TikTok.UserService.Application.Validators
{
    public class FollowUserDtoValidator : AbstractValidator<FollowUserDto>
    {
        public FollowUserDtoValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");
        }
    }
}
