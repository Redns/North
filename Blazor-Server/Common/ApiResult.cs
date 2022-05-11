using Newtonsoft.Json;

namespace ImageBed.Common
{
    public class ApiResult<T>
    {
        // 状态码
        private ApiResultCode _ApiResultCode;
        public ApiResultCode ApiResultCode
        {
            get { return _ApiResultCode; }
            set
            {
                if (value > 0)
                {
                    _ApiResultCode = value;
                }
            }
        }

        // 提示信息
        private string _Message;
        public string Message
        {
            get { return _Message; }
            set
            {
                _Message = value;
                if (string.IsNullOrEmpty(_Message))
                {
                    _Message = "success";
                }
            }
        }

        // 返回值
        private T? _Res;
        public T? Res
        {
            get { return _Res; }
            set
            {
                _Res = value;
            }
        }

        public ApiResult(ApiResultCode apiResultCode, string message, T? res)
        {
            ApiResultCode = apiResultCode;
            Message = message;
            Res = res;
        }


        /// <summary>
        /// 序列化类
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public enum ApiResultCode
    {
        Success = 0,            // 成功
        InternalError,          // 服务器内部错误
        TokenGenerateFailed,    // 令牌生成失败
        TokenDestroyFailed,     // 令牌销毁失败
        TokenInvalid,           // 非法的令牌
        AccessDenied,           // 权限不足
        SpaceNotEnough,         // 存储空间不足
        UserNotFound,           // 用户不存在
    }
}
