using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HarelTech.WMS.Common.Models
{
    public class TaskLotSerial
    {
        public string HWMS_ELOTNUMBER { get; set; }
        public string FROMBIN { get; set; }
        public string STATUS { get; set; }
        public long EXPIRYDATE { get; set; }
        [NotMapped]
        public DateTime ExpDate { get; set; }
        public long AVAILABLE { get; set; }
        public string UNITNAME { get; set; }
    }
}
