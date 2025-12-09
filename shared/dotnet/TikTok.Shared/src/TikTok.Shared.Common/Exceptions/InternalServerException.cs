namespace TikTok.Shared.Common.Exceptions
{
    public class InternalServerException : BaseException
    {
        public InternalServerException(string message = "An internal server error occurred.", string errorCode = "INTERNAL_SERVER_ERROR")
            : base(message, errorCode)
        {
        }

        public InternalServerException(string message, Exception innerException, string errorCode = "INTERNAL_SERVER_ERROR")
            : base(message, errorCode, innerException)
        {
        }
    }
}
