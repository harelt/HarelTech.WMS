using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HarelTech.WMS.Common.Entities
{
    [Table("WAREHOUSES")]
    public class Warhouse
    {
        [Key]
        public long WARHS { get; set; }
        public string WARHSNAME { get; set; }
        public string WARHSDES { get; set; }
        public string LOCNAME { get; set; }
    }
}
