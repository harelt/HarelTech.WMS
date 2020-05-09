
namespace HarelTech.WMS.Common.Models
{
    public class AuthUser
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }

        public AuthUser WithoutPassword()
        {
            var u = this;
            u.Password = "";
            return u;
        }
    }
}
