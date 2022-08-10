using System.Text.RegularExpressions;

namespace North.Models.Auth
{
    public class LoginModel
    {
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        public LoginModel()
        {
            Email = string.Empty;
            Password = string.Empty;
        }

        public LoginModel(string email, string password)
        {
            Email = email;
            Password = password;
        }


        /// <summary>
        /// 校验登录模型
        /// </summary>
        /// <returns></returns>
        public string ValidCheck()
        {
            if (string.IsNullOrEmpty(Email)) 
            { 
                return "邮箱不能为空"; 
            }
            else if(string.IsNullOrEmpty(Password))
            {
                return "密码不能为空";
            }
            else if(!new Regex("^\\s*([A-Za-z0-9_-]+(\\.\\w+)*@(\\w+\\.)+\\w{2,5})\\s*$").IsMatch(Email))
            {
                return "邮箱格式不正确";
            }
            return string.Empty;
        }
    }
}
