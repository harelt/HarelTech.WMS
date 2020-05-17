using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HarelTech.WMS.Common.Entities
{
    [Table("USERS")]
    public class SystemUser
    {
        [Key, Column("T$USER")]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string USERLOGIN { get; set; }
        public string USERNAME { get; set; }
        public long USERID { get; set; }
        public long USERGROUP { get; set; }
    }
}
