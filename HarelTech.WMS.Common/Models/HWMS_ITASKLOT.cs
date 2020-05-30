using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HarelTech.WMS.Common.Models
{
    [Table("HWMS_ITASKLOT")]
    public class ITASKLOT
    {
        [Key, Column("HWMS_ITASKLOT")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
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

    }
}
