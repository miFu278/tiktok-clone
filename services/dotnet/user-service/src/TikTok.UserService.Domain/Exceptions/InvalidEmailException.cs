using TikTok.Shared.Common.Exceptions;

namespace TikTok.UserService.Domain.Exceptions
{
    public sealed class InvalidEmailException : BadRequestException
    {
        public InvalidEmailException(string email)
            : base($"Email '{email}' is invalid", "INVALID_EMAIL")
        {
        }
    }
}
