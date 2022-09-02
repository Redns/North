namespace North.Common
{
    public class ApiResult<T>
    {
        public ApiStatusCode Code { get; set; }
        public string Message { get; set; }
        public T? Result { get; set; }

        public ApiResult(ApiStatusCode code, string message, T? result = default)
        {
            Code = code;
            Message = message;
            Result = result;
        }

        public ApiResult(ApiStatusCode code, T? result = default)
        {
            Code = code;
            Message = code switch
            {
                ApiStatusCode.Success => "Success",
                ApiStatusCode.ParamIncomplete => "Param incomplete",
                ApiStatusCode.AccountNotExist => "Account not exist",
                ApiStatusCode.UnAuthorized => "UnAuthorized",
                ApiStatusCode.PermissionDenied => "Permission denied",
                ApiStatusCode.OperationDenied => "Operation denied",
                ApiStatusCode.AccountStateChanged => "Account state changed",
                ApiStatusCode.ServerInternalError => "Server Internal Error",
                _ => string.Empty
            };
            Result = result;
        }
    }


    /// <summary>
    /// 状态码
    /// </summary>
    public enum ApiStatusCode
    {
        None = 0,               // 未指定
        Success,                // 成功
        ParamIncomplete,        // 参数不完整
        AccountNotExist,        // 账号不存在
        UnAuthorized,           // 未授权
        PermissionDenied,       // 权限不足
        OperationDenied,        // 拒绝操作
        AccountStateChanged,    // 账户状态改变
        ServerInternalError,    // 服务器内部错误
    }
}
