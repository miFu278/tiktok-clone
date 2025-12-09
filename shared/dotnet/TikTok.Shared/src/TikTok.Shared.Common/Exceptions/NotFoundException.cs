namespace TikTok.Shared.Common.Exceptions
{
    public class NotFoundException : BaseException
    {
        public NotFoundException(string message, string errorCode = "NOT_FOUND")
            : base(message, errorCode)
        {
        }

        public NotFoundException(string entityName, object key)
            : base($"{entityName} with key '{key}' was not found.", "NOT_FOUND")
        {
        }
    }
}
