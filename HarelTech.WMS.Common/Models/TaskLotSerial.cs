using HarelTech.WMS.Common.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HarelTech.WMS.Common.Models
{
    public class TaskLotSerial
    {
        public long HWMS_ITASKLOT { get; set; }
        public string HWMS_ELOTNUMBER { get; set; }
        public string FROMBIN { get; set; }
        public string TOBIN { get; set; }
        public string STATUS { get; set; }
        public long EXPIRYDATE { get; set; }
        [NotMapped]
        public string ExpDate { get; set; }
        public long AVAILABLE { get; set; }
        public string UNITNAME { get; set; }
        public long HWMS_ITASK { get; set; }
        public long HWMS_LOT { get; set; }
        public long Quantity { get; set; }
        public string HWMS_FCUSTNAME { get; set; }
        public string HWMS_TCUSTNAME { get; set; }
        [NotMapped]
        public List<Serial> Serials { get; set; }
        [NotMapped]
        public long ITaskLot { get; set; }
    }

    public class Serial
    {
        public long SerialId { get; set; }
        public string SerialNumber { get; set; }
    }
}
