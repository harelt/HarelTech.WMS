using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HarelTech.WMS.Common.Entities
{
    [Table("HWMS_ITASKSER")]
    public class Tasker
    {
        [Key]
        public long HWMS_ITASKSER { get; set; }
        /// <summary>
        /// Serial number(ID)
        /// </summary>
        public long HWMS_SERN { get; set; }
        /// <summary>
        /// Serial number
        /// </summary>
        public string HWMS_SERNUMBER { get; set; }
        /// <summary>
        /// inv. Task(ID)
        /// </summary>
        public long HWMS_ITASK { get; set; }
        public string HWMS_FROMBIN { get; set; }
        public string HWMS_TOBIN { get; set; }
        
    }
}
