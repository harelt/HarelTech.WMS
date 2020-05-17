namespace HarelTech.WMS.Common.Models
{
    public class CompleteTasksByGroup
    {
        public long HWMS_ITASKTYPE { get; set; }
        public string HWMS_ITASKTYPENAME { get; set; }
        public string HWMS_REFTYPENAME { get; set; }
        public string HWMS_REFORDER { get; set; }
        public string HWMS_REFNAME { get; set; }
        public long CompleteTasks { get; set; }
        public long TotalTasks { get; set; }
        public string HWMS_WZONENAM { get; set; }
        public  string HWMS_WZONECODE { get; set; }
    }
}
