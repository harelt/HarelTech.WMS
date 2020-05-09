using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HarelTech.WMS.Common.Entities
{
    [Table("HWMS_USERS")]
    public class WmsUser
    {
        [Key]
        public long HWMS_USER { get; set; }
        [MaxLength(1)]
        public string HWMS_ACTIVE { get; set; }
        [MaxLength(48)]
        public string HWMS_EMAIL { get; set; }
        public long HWMS_WARHS { get; set; }
    }

}
