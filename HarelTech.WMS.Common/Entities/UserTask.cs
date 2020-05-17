using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HarelTech.WMS.Common.Entities
{
    /// <summary>
    /// Priority form HWMS_ITASKS
    /// </summary>
    [Table("HWMS_ITASKS")]
    public class UserTask
    {
        [Key]
        public long HWMS_ITASK { get; set; }
        /// <summary>
        /// Inventory Task Num.
        /// </summary>
        public string HWMS_ITASKNUM { get; set; }
        /// <summary>
        /// Reference Order
        /// </summary>
        public string HWMS_REFORDER { get; set; }
        /// <summary>
        /// Reference Order line
        /// </summary>
        public long HWMS_REFORDERLINE { get; set; }
        /// <summary>
        /// Task Status
        /// </summary>
        public string HWMS_ITASKSTATUS { get; set; }
        public long HWMS_ITASKPART { get; set; }
        public long HWMS_TOTALQTY { get; set; }
        public long HWMS_COMPLETEDQTY { get; set; }
        public long HWMS_ITASKTYPE { get; set; }
        public long HWMS_ITASKWARHS { get; set; }
        /// <summary>
        /// Default To BIN
        /// </summary>
        public string HWMS_ITASKTOBIN { get; set; }
        /// <summary>
        /// Default From BIN
        /// </summary>
        public string HWMS_ITASKFROMBIN { get; set; }
        /// <summary>
        /// Def. To Zone(ID)
        /// </summary>
        public long HWMS_ITASKTZONE { get; set; }
        /// <summary>
        /// Def. From Zone(ID)
        /// </summary>
        public long HWMS_ITASKFZONE { get; set; }
        public long HWMS_USEROPEN { get; set; }
        /// <summary>
        /// Time opened
        /// </summary>
        public long HWMS_TIMEOPENED { get; set; }
        [NotMapped]
        public DateTime TimeOpened { get; set; }
        public long HWMS_USERUPDATED { get; set; }
        public long HWMS_TIMEUPDATED { get; set; }
        [NotMapped]
        public DateTime TimeUpdated { get; set; }
        /// <summary>
        /// Task Due Date
        /// </summary>
        public long HWMS_DUEDATE { get; set; }
        [NotMapped]
        public DateTime TaskDueDate { get; set; }
        /// <summary>
        /// Reference Name
        /// </summary>
        public string HWMS_REFNAME { get; set; }
        /// <summary>
        /// PO or SO details(ID)
        /// </summary>
        public long HWMS_ORDI { get; set; }
        public long HWMS_ASSIGNUSER { get; set; }
        public long HWMS_ITASKFDATE { get; set; }

    }
}
