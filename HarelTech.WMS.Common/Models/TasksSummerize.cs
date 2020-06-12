using HarelTech.WMS.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HarelTech.WMS.Common.Models
{
    public class TasksSummerize
    {
        public List<TaskSum> Tasks { get; set; }
        public int Done { get; set; }
        public int Total { get; set; }
    }

    public class TaskSum
    {
        public long Task { get; set; }
        public int Count { get; set; }
        public string show { get; set; } = "";
    }

    
}
