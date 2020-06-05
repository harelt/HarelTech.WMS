using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HarelTech.WMS.Common.Entities
{
    [Table("HWMS_ITASKSERIALS")]
    public class TaskSerial
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long HWMS_ITASKSERIAL { get; set; }
        public long HWMS_SERN { get; set; }
        public long HWMS_ITASKLOT { get; set; }
        [MaxLength(20)]
        public string HWMS_SERNUMBER { get; set; }
    }

}
