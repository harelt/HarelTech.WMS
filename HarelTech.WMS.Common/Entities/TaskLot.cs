using HarelTech.WMS.Common.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HarelTech.WMS.Common.Entities
{
    [Table("HWMS_ITASKLOTS")]
    public class TaskLot
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long HWMS_ITASKLOT { get; set; }
        [MaxLength(22)]
        public string HWMS_LOTNUMBER { get; set; }
        public long HWMS_LOT { get; set; }
        public long HWMS_LOTQUANTITY { get; set; }
        [MaxLength(14)]
        public string HWMS_FROMBIN { get; set; }
        [MaxLength(14)]
        public string HWMS_TOBIN { get; set; }
        public long HWMS_EXPDATE { get; set; }
        public long HWMS_ITASK { get; set; }
        public string HWMS_FCUSTNAME { get; set; }
        public string HWMS_TCUSTNAME { get; set; }
        [NotMapped]
        public List<Serial> Serials { get; set; }

    }
}
