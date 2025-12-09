namespace TikTok.Shared.Common.Exceptions
{
    public class ConflictException : BaseException
    {
        public ConflictException(string message, string errorCode = "CONFLICT")
            : base(message, errorCode)
        {
        }

        public ConflictException(string entityName, object key)
            : base($"{entityName} with key '{key}' already exists.", "CONFLICT")
        {
        }
    }
}
