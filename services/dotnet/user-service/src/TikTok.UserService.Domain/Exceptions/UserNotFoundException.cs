using TikTok.Shared.Common.Exceptions;

namespace TikTok.UserService.Domain.Exceptions
{
    public sealed class UserNotFoundException : NotFoundException
    {
        public UserNotFoundException(Guid userId)
            : base("User", userId)
        {
        }

        public UserNotFoundException(string identifier)
            : base($"User with identifier '{identifier}' was not found", "USER_NOT_FOUND")
        {
        }
    }
}
