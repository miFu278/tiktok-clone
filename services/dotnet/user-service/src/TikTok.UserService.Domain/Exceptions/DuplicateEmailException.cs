using TikTok.Shared.Common.Exceptions;

namespace TikTok.UserService.Domain.Exceptions
{
    public sealed class DuplicateEmailException : ConflictException
    {
        public DuplicateEmailException(string email)
            : base($"Email '{email}' is already registered", "DUPLICATE_EMAIL")
        {
        }
    }
}
