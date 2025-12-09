namespace TikTok.Shared.Common.Models
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public string[]? Errors { get; set; }
        public DateTime Timestamp { get; set; }

        public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
        {
            return new ApiResponse<T>
            {
                IsSuccess = true,
                Message = message,
                Data = data,
                Timestamp = DateTime.UtcNow
            };
        }
        public static ApiResponse<T> ErrorResponse(string message, string[]? errors = null)
        {
            return new ApiResponse<T>
            {
                IsSuccess = false,
                Message = message,
                Errors = errors,
                Timestamp = DateTime.UtcNow
            };
        }

    }
}
