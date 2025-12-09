namespace TikTok.Shared.Common.Exceptions
{
    public class ValidationException : BaseException
    {
        public IDictionary<string, string[]> Errors { get; }

        public ValidationException(string message, string errorCode = "VALIDATION_ERROR")
            : base(message, errorCode)
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(IDictionary<string, string[]> errors)
            : base("One or more validation errors occurred.", "VALIDATION_ERROR")
        {
            Errors = errors;
        }
    }
}
