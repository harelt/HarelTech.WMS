using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HarelTech.WMS.Common.Models
{
    public class UserLoginModel
    {
        [Required(AllowEmptyStrings = false)]
        public string UserName { get; set; }
        [Required(AllowEmptyStrings = true)]
        public string Password { get; set; }
        public string Companies { get; set; }
    }


    public class Company
    {
        public string dname { get; set; }
        public string title { get; set; }
    }
}
