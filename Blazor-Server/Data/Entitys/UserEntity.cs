using ImageBed.Common;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ImageBed.Data.Entity
{
    /// <summary>
    /// 用户类型枚举
    /// </summary>
    public enum UserType
    {
        User,               // 普通用户
        Admin,              // 管理员
        SuperAdmin          // 超级管理员
    }


    public class UserEntity
    {
        // TODO 纳入设置文件
        public const int USERNAME_MIN_LENGTH = 8;               // 最短用户名
        public const int USERNAME_MAX_LENGTH = 20;              // 最长用户名
        public const int PASSWORD_MIN_LENGTH = 8;               // 最短密码
        public const int PASSWORD_MAX_LENGTH = 20;              // 最长密码
        public const int TOKEN_LENGTH = 32;                     // 令牌长度
        public const int TOKEN_VALID_PERIOD = 24 * 60 * 60;     // 令牌有效期

        [Key]
        public string UserName { get; set; }                    // 用户名（账号）
        public string Password { get; set; }                    // 密码（16位MD5加密）
        public string Token { get; set; }                       // 令牌
        public long ExpireTime { get; set; }                    // 到期时间
        public UserType UserType { get; set; }                  // 类型
        public string Cover { get; set; }                       // 封面
        public int TotalUploadNum { get; set; }                 // 已上传图片总数（单位：张）
        public double TotalUploadSize { get; set; }             // 已上传图片总容量（单位：MB）
        public int TotalUploadMaxNum { get; set; }              // 最大上传总数（单位：张）
        public double TotalUploadMaxSize { get; set; }          // 最大上传总容量（单位：MB）
        public double SingleUploadMaxSize { get; set; }         // 单次最大上传尺寸(单位：MB)
        public int SingleUploadMaxNum { get; set; }             // 单次最大上传数量(单位：张)

        public UserEntity(string userName, string password, string token, long expireTime, UserType userType, string cover, int totalUploadNum, double totalUploadSize, int totalUploadMaxNum, double totalUploadMaxSize, double singleUploadMaxSize, int singleUploadMaxNum)
        {
            UserName = userName;
            Password = password;
            Token = token;
            ExpireTime = expireTime;
            UserType = userType;
            Cover = cover;
            TotalUploadNum = totalUploadNum;
            TotalUploadSize = totalUploadSize;
            TotalUploadMaxNum = totalUploadMaxNum;
            TotalUploadMaxSize = totalUploadMaxSize;
            SingleUploadMaxSize = singleUploadMaxSize;
            SingleUploadMaxNum = singleUploadMaxNum;
        }


        /// <summary>
        /// 序列化用户信息
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }


        /// <summary>
        /// 生成令牌
        /// </summary>
        /// <param name="validPeriod">有效时间（单位：秒，默认24小时）</param>
        public void GenerateToken(long validPeriod = TOKEN_VALID_PERIOD)
        {
            string newToken;

            while ((newToken = FileHelper.GererateRandomString(TOKEN_LENGTH)) == Token){ }

            Token = newToken;
            ExpireTime = FileHelper.GetTimeStamp() + validPeriod * 1000;
        }


        /// <summary>
        /// 销毁令牌
        /// </summary>
        public void DestroyToken()
        {
            Token = string.Empty;
            ExpireTime = 0;
        }


        /// <summary>
        /// 检验Token是否有效
        /// </summary>
        /// <returns>有效返回True，否则返回False</returns>
        public bool IsTokenValid()
        {
            if(!string.IsNullOrEmpty(Token) && (FileHelper.GetTimeStamp() < ExpireTime))
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// 生成DTO对象
        /// </summary>
        /// <returns></returns>
        public UserDTOEntity DTO()
        {
            return new UserDTOEntity(UserName, ExpireTime, UserType, Cover, TotalUploadNum, TotalUploadSize, TotalUploadMaxNum, TotalUploadMaxSize, SingleUploadMaxSize, SingleUploadMaxNum);
        }


        /// <summary>
        /// 解析用户字符串
        /// </summary>
        /// <param name="userStr"></param>
        /// <returns></returns>
        public static UserEntity? Parse(string userStr)
        {
            return JsonConvert.DeserializeObject<UserEntity>(userStr);
        }
    }


    public class UserDTOEntity
    {
        public string UserName { get; set; }                    // 用户名（账号）
        public long ExpireTime { get; set; }                    // 到期时间
        public UserType UserType { get; set; }                  // 类型
        public string Cover { get; set; }                       // 封面
        public int TotalUploadNum { get; set; }                 // 已上传图片总数（单位：张）
        public double TotalUploadSize { get; set; }             // 已上传图片总容量（单位：MB）
        public int TotalUploadMaxNum { get; set; }              // 最大上传总数（单位：张）
        public double TotalUploadMaxSize { get; set; }          // 最大上传总容量（单位：MB）
        public double SingleUploadMaxSize { get; set; }         // 单次最大上传尺寸(单位：MB)
        public int SingleUploadMaxNum { get; set; }             // 单次最大上传数量(单位：张)

        public UserDTOEntity(string userName, long expireTime, UserType userType, string cover, int totalUploadNum, double totalUploadSize, int totalUploadMaxNum, double totalUploadMaxSize, double singleUploadMaxSize, int singleUploadMaxNum)
        {
            UserName = userName;
            ExpireTime = expireTime;
            UserType = userType;
            Cover = cover;
            TotalUploadNum = totalUploadNum;
            TotalUploadSize = totalUploadSize;
            TotalUploadMaxNum = totalUploadMaxNum;
            TotalUploadMaxSize = totalUploadMaxSize;
            SingleUploadMaxSize = singleUploadMaxSize;
            SingleUploadMaxNum = singleUploadMaxNum;
        }
    }
}
