using HarelTech.WMS.Common.Entities;
using Microsoft.EntityFrameworkCore;

namespace HarelTech.WMS.Repository
{
    public class PrioritySystemContext : DbContext
    {
        
        public PrioritySystemContext(DbContextOptions<PrioritySystemContext> options) : base(options) { }
        
        public virtual DbSet<WmsUser> WmsUsers { get;  }
        public virtual DbSet<SystemUser> SystemUsers { get;  }
    }
}
