using Newtonsoft.Json;

namespace ImageBed.Data.Entity
{
    public class ApiResult<T>
    {
        // 状态码
        private int _StatusCode;
        public int StatusCode
        {
            get { return _StatusCode; }
            set
            {
                if (value > 0)
                {
                    _StatusCode = value;
                }
            }
        }

        // 反馈信息
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


        public ApiResult() { }
        public ApiResult(int statusCode, string message, T? res)
        {
            StatusCode = statusCode;
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


        /// <summary>
        /// 反序列化字符串生成ApiResult
        /// </summary>
        /// <param name="res">待反序列化的字符串</param>
        /// <returns>反序列化成功返回实体类，否则返回null</returns>
        public ApiResult<T>? Load(string res)
        {
            if (!string.IsNullOrEmpty(res))
            {
                try
                {
                    return JsonConvert.DeserializeObject<ApiResult<T>>(res);
                }
                catch (Exception)
                {

                }
            }
            return null;
        }
    }
}
