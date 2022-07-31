using System.Security.Claims;

namespace North.Models.Auth
{
    public class UnitLoginIdentify
    {
        public string Id { get; set; }
        public ClaimsIdentity ClaimsIdentity { get; set; }

        public UnitLoginIdentify(string id, ClaimsIdentity claimsIdentity)
        {
            Id = id;
            ClaimsIdentity = claimsIdentity;
        }
    }
}
