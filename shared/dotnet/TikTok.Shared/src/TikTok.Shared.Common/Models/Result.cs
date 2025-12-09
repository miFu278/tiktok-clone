namespace TikTok.Shared.Common.Models
{
    public class Result
    {
        public bool IsSuccess { get; protected set; }
        public string Message { get; protected set; } = string.Empty;
        public string[]? Errors { get; protected set; }

        protected Result(bool isSuccess, string message, string[]? errors = null)
        {
            IsSuccess = isSuccess;
            Message = message;
            Errors = errors;
        }

        public static Result Success(string message = "Success")
            => new(true, message);

        public static Result Failure(string message, string[]? errors = null)
            => new(false, message, errors);
    }

    public class Result<T> : Result
    {
        public T? Data { get; private set; }
        private Result(bool isSuccess, string message, T? data = default, string[]? errors = null)
            : base(isSuccess, message, errors)
        {
            Data = data;
        }
        public static Result<T> Success(T data, string message = "Success")
            => new(true, message, data);
        public static new Result<T> Failure(string message, string[]? errors = null)
            => new(false, message, default, errors);
    }
}
