namespace ImageBed.Common
{
    public class ApiException : ApplicationException
    {
        public ApiResultCode ApiResultCode { get; set; }
        public object? Param { get; set; }

        public ApiException(ApiResultCode apiResultCode, object? param = null)
        {
            ApiResultCode = apiResultCode;
            Param = param;
        }
    }
}
