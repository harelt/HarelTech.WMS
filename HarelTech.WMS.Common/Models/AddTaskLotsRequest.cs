using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HarelTech.WMS.Common.Models
{
    public class AddTaskLotsRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string Company { get; set; }
        [Required]
        public List<TaskLotSerial> Lots { get; set; }
    }
}
