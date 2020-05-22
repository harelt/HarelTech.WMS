namespace HarelTech.WMS.Common.Models
{
    public class CompleteTaskItem
    {
        public long HWMS_ITASKTYPE { get; set; }
        public string HWMS_ITASKTYPEDES { get; set; }
        public string HWMS_REFORDER { get; set; }
        public long HWMS_ITASK { get; set; }
        public long PART { get; set; }
        public string PARTNAME { get; set; }
        public string PARTDES { get; set; }
        public string HWMS_ITASKFROMBIN { get; set; }
        public string HWMS_ITASKTOBIN { get; set; }
        public string HWMS_ITASKTZONE { get; set; }
        public string HWMS_ITASKFZONE { get; set; }
        public long CompletedTasks { get; set; }
        public long TotalTasks { get; set; }
        public string SERNFLAG { get; set; }
        public string HWMS_ITASKNUM { get; set; }
    }
}
