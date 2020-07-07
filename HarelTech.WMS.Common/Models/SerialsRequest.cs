using System.ComponentModel.DataAnnotations;

namespace HarelTech.WMS.Common.Models
{
    public class SerialsRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string Company { get; set; }
        public long PartId { get; set; }
        public long SerialId { get; set; }
        public string LocName { get; set; }

    }
}
