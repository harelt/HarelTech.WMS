using System.ComponentModel.DataAnnotations;

namespace HarelTech.WMS.Common.Models
{
    public class AuthenticateModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
