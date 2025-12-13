using TikTok.Shared.Common.Exceptions;

namespace TikTok.UserService.Domain.Exceptions
{
    public sealed class DuplicateUsernameException : ConflictException
    {
        public DuplicateUsernameException(string username)
            : base($"Username '{username}' is already taken", "DUPLICATE_USERNAME")
        {
        }
    }
}
