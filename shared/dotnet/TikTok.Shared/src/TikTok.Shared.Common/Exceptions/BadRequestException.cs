namespace TikTok.Shared.Common.Exceptions
{
    public class BadRequestException : BaseException
    {
        public BadRequestException(string message, string errorCode = "BAD_REQUEST")
            : base(message, errorCode)
        {
        }
    }
}
