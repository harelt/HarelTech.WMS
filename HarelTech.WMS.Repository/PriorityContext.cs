using HarelTech.WMS.Common.Entities;
using Microsoft.EntityFrameworkCore;

namespace HarelTech.WMS.Repository
{
    public class PriorityContext : DbContext
    {
        
        public PriorityContext(DbContextOptions<PriorityContext> options) : base(options) { }
        public virtual DbSet<Warhouse> Warhouses { get; set; }
        public virtual DbSet<UserTask> Tasks { get; set; }
        public virtual DbSet<TaskLot> TaskLots { get; set; }
        public virtual DbSet<UserWarhouse> UsersWarhouses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserWarhouse>()
                .HasKey(c => new { c.HWMS_USER, c.HWMS_WARHS });

            //modelBuilder.Entity<Banner>().Property(banner => banner.EHAR_SHOWBANNER).HasColumnType("nchar(1)");

            //modelBuilder.Entity<BTOB_LOADORDERSA>().Property(la => la.VATFLAG).HasColumnType("nchar(1)");
            //modelBuilder.Entity<BTOB_LOADORDERSA>().Property(la => la.LOADED).HasColumnType("nchar(1)");
            //modelBuilder.Entity<BTOB_LOADORDERSA>().Property(la => la.PREPAYED).HasColumnType("nchar(1)");
            //modelBuilder.Entity<BTOB_LOADORDERSA>().Property(la => la.PRSOURCE).HasColumnType("nchar(1)");
            //modelBuilder.Entity<BTOB_LOADORDERSA>().Property(la => la.PURSOURCE).HasColumnType("nchar(1)");
            //modelBuilder.Entity<BTOB_LOADORDERSA>().Property(la => la.ACTION).HasColumnType("nchar(1)");
            //modelBuilder.Entity<BTOB_LOADORDERSA>().Property(la => la.VATFLAG).HasColumnType("nchar(1)");

        }
    }
}
