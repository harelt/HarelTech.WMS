using System;
using System.Collections.Generic;
using System.Text;

namespace HarelTech.WMS.Common.Models
{
    /// <summary>
    /// used for retrieve task lots that already been reported but not completed
    /// </summary>
    public class ITaskLotModel
    {
        public long HWMS_ITASKLOT { get; set; }
        public long HWMS_LOT { get; set; }
        public long HWMS_ITASK { get; set; }
        public long Quantity { get; set; }
    }
}
