namespace ApiUsers.Models.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public T Data { get; set; }

        public ApiResponse() { }

        public ApiResponse(bool succes, T data = default, string[] errors = null)
        {
            Success = succes;
            Data = data;
            Errors = errors;
        }

        public static ApiResponse<T> SuccessResponse(T data)
            => new ApiResponse<T>()
            {
                Success = true,
                Errors = [],
                Data = data
            };

        public static ApiResponse<string> FailResponse(string message, params string[] errors)
            => new ApiResponse<string>()
            {
                Success = false,
                Errors = errors,
                Data = message
            };
    }
}
