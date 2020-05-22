using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
namespace HarelTech.WMS.App.Models
{
    public static class Utilities
    {
        public static long UserId(IEnumerable<Claim> claims)
        {
            var id = claims.FirstOrDefault(w => w.Type == ClaimTypes.UserData).Value;
            return long.Parse(id);
        }

        public static string UserName(IEnumerable<Claim> claims)
        {
            return claims.FirstOrDefault(w => w.Type == ClaimTypes.Name).Value;
        }
    }
}
