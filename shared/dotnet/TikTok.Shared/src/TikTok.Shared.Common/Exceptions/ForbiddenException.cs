namespace TikTok.Shared.Common.Exceptions
{
    public class ForbiddenException : BaseException
    {
        public ForbiddenException(string message = "Access forbidden.", string errorCode = "FORBIDDEN")
            : base(message, errorCode)
        {
        }
    }
}
