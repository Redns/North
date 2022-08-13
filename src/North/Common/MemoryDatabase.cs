using North.Core.Data.Entities;

namespace North.Common
{
    public class MemoryDatabase
    {
        public List<UserEntity> Users { get; set; }
        public List<VerifyEmailEntity> VerifyEmails { get; set; }

        public MemoryDatabase(List<UserEntity> users, List<VerifyEmailEntity> verifyEmails)
        {
            Users = users;
            VerifyEmails = verifyEmails;
        }
    }
}
