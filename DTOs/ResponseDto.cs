namespace Pet_s_Land.DTOs
{
    public class ResponseDto<T>
    {
        public T Data { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public string Error { get; set; } // Error message (if any)

        public ResponseDto(T data, string message, int statusCode, string error = null)
        {
            Data = data;
            Message = message;
            StatusCode = statusCode;
            Error = error;
        }

    }

}