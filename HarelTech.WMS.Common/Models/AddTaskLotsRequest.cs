﻿using HarelTech.WMS.Common.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
