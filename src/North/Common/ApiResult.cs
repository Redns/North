namespace North.Common
{
    public class ApiResult<T>
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public T? Result { get; set; }

        public ApiResult(int code, string message, T? result)
        {
            Code = code;
            Message = message;
            Result = result;
        }
    }
}
