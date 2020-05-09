using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HarelTech.WMS.Common.Entities
{
    [Table("HWMS_USERWARHS")]
    public class UserWarhouse
    {
        public long HWMS_USER { get; set; }
        public long HWMS_WARHS { get; set; }
        [MaxLength(1)]
        public string HWMS_WARHSDEF { get; set; }
        [NotMapped]
        public bool IsDefault { get; set; }
    }
}
