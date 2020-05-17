using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HarelTech.WMS.Common.Entities
{
    [Table("HWMS_ITASKTYPES")]
    public class TaskType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long HWMS_ITASKTYPE { get; set; }
        [MaxLength(16)]
        public string HWMS_ITASKTYPENAME { get; set; }
        [MaxLength(48)]
        public string HWMS_ITASKTYPEDES { get; set; }
        [MaxLength(1)]
        public string HWMS_ITASKDOCTYPE { get; set; }
    }

}
