namespace North.Models.Auth
{
    public class LoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public LoginModel()
        {
            UserName = string.Empty;
            Password = string.Empty;
        }

        public LoginModel(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }
    }
}
