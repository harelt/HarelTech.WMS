using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HarelTech.WMS.Common.Models
{
    public class CompleteTaskItemsRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string Company { get; set; }
        public long UserId { get; set; }
        public long WarhouseId { get; set; }
        [EnumDataType(typeof(EnumTaskType))]
        public EnumTaskType TaskType { get; set; }
        [EnumDataType(typeof(EnumTaskGroup))]
        public EnumTaskGroup TaskGroup { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string RefOrderOrZone { get; set; }
    }
}
