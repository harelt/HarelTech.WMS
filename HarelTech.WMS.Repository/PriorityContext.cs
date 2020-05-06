using HarelTech.WMS.Common.Entities;
using Microsoft.EntityFrameworkCore;

namespace HarelTech.WMS.Repository
{
    public class PriorityContext : DbContext
    {
        
        public PriorityContext(DbContextOptions<PriorityContext> options) : base(options) { }
        public virtual DbSet<Warhouse> Warhouses { get; set; }
        public virtual DbSet<Task> Tasks { get; set; }
        public virtual DbSet<TaskLot> TaskLots { get; set; }


    }
}
