using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace HarelTech.WMS.Common.Models
{
    public class CompleteTasksByGroupRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string Company { get; set; }
        public long UserId { get; set; }
        public long WarhouseId { get; set; }
        [EnumDataType(typeof(EnumTaskType))]
        public EnumTaskType TaskType { get; set; }
        [EnumDataType(typeof(EnumTaskGroup))]
        public EnumTaskGroup TaskGroup { get; set; }

    }
}
