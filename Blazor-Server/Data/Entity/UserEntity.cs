using ImageBed.Common;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ImageBed.Data.Entity
{
    public class UserEntity
    {
        public const int USERNAME_MIN_LENGTH = 8;       // 最短用户名
        public const int USERNAME_MAX_LENGTH = 20;      // 最长用户名
        public const int PASSWORD_MIN_LENGTH = 8;       // 最短密码
        public const int PASSWORD_MAX_LENGTH = 20;      // 最长密码
        public const int TOKEN_LENGTH = 32;

        [Key]
        public string UserName { get; set; }            // 用户名（账号）
        public string Password { get; set; }            // 密码（16位MD5加密）
        public string Token { get; set; }               // 令牌
        public long ExpireTime { get; set; }            // 到期时间
        public UserType UserType { get; set; }          // 类型
        public string Cover { get; set; }               // 封面
        public int UploadMaxNum { get; set; }           // 最大上传数量（单位：张）
        public double UploadMaxSize { get; set; }       // 最大上传容量（单位：MB）

        public UserEntity(string userName, string password, string token, long expireTime, UserType userType, string cover, int uploadMaxNum, double uploadMaxSize)
        {
            UserName = userName;
            Password = password;
            Token = token;
            ExpireTime = expireTime;
            UserType = userType;
            Cover = cover;
            UploadMaxNum = uploadMaxNum;
            UploadMaxSize = uploadMaxSize;
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


        /// <summary>
        /// 序列化用户信息
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }


        /// <summary>
        /// 校验用户信息的合法性
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool ValidCheck(UserEntity user)
        {
            Regex regex = new(@"^\w+$");

            if((string.IsNullOrEmpty(user.UserName)) || 
               (user.UserName.Length < USERNAME_MIN_LENGTH) || 
               (user.UserName.Length > USERNAME_MAX_LENGTH) ||
               (!regex.IsMatch(user.UserName)) ||
               (string.IsNullOrEmpty(user.Password)) ||
               (user.Password.Length < PASSWORD_MIN_LENGTH) ||
               (user.Password.Length > PASSWORD_MAX_LENGTH) ||
               (!regex.IsMatch(user.Password)) ||
               (user.UploadMaxNum <= 0) ||
               (user.UploadMaxSize <= 0))
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// 生成令牌
        /// </summary>
        /// <param name="validPeriod">有效时间（单位：秒，默认24小时）</param>
        public void GenerateToken(long validPeriod = 24 * 60 * 60)
        {
            string newToken;

            while ((newToken = UnitNameGenerator.GererateRandomString(TOKEN_LENGTH)) == Token){ }

            Token = newToken;
            ExpireTime = UnitNameGenerator.GetTimeStamp() + validPeriod * 1000;
        }


        /// <summary>
        /// 检验Token是否有效
        /// </summary>
        /// <returns>有效返回True，否则返回False</returns>
        public bool IsTokenValid()
        {
            if(!string.IsNullOrEmpty(Token) && (UnitNameGenerator.GetTimeStamp() < ExpireTime))
            {
                return true;
            }
            return false;
        }
    }


    /// <summary>
    /// 用户类型枚举
    /// </summary>
    public enum UserType
    {
        Visitor = 0,        // 游客
        User,               // 普通用户
        Admin,              // 管理员
        SuperAdmin          // 超级管理员
    }
}
