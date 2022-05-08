namespace ImageBed.Data.Entity
{
    public class LoginEntity
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public LoginEntity(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }
    }
}
