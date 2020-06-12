using System;
using System.Collections.Generic;
using System.Text;

namespace HarelTech.WMS.Common.Models
{
    public class AddNewLotRequest
    {
        public int Id { get; set; }
        public long TaskId { get; set; }
        public string LotNumber { get; set; }
        public long  PartId { get; set; }
        public string SernFlag { get; set; }
        public int TaskType { get; set; }
        public string FromBin { get; set; }
        public string ToBin { get; set; }
        public int Qty { get; set; }
    }
}
