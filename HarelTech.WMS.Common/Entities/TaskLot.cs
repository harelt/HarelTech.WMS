using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HarelTech.WMS.Common.Entities
{
    [Table("HWMS_ITASKLOTS")]
    public class TaskLot
    {
        [Key]
        public long HWMS_ITASKLOT { get; set; }
        public string HWMS_LOTNUMBER { get; set; }
        public long HWMS_LOT { get; set; }
        public long HWMS_LOTQUANTITY { get; set; }
        public string HWMS_FROMBIN { get; set; }
        public string HWMS_TOBIN { get; set; }
        public System.DateTime HWMS_EXPDATE { get; set; }
        /// <summary>
        /// Inv. Task(ID)
        /// </summary>
        public long HWMS_ITASK { get; set; }
        
    }
}
