namespace TikTok.Shared.Common.Exceptions
{
    public class UnauthorizedException : BaseException
    {
        public UnauthorizedException(string message = "Unauthorized access.", string errorCode = "UNAUTHORIZED")
            : base(message, errorCode)
        {
        }
    }
}
