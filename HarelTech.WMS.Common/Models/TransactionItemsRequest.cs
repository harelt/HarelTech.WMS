using System.ComponentModel.DataAnnotations;

namespace HarelTech.WMS.Common.Models
{
    public class TransactionItemsRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string Company { get; set; }
        public long WarhouseId { get; set; }
        public long ParId { get; set; }
    }
}
