using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HarelTech.WMS.Common.Models
{
    public class ActivateTaskRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string Company { get; set; }
        public long  UserId { get; set; }
        public long TaskId { get; set; }
    }
}
