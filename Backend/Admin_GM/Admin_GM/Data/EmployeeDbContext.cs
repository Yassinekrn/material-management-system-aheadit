using Admin_GM.Models;
using Microsoft.EntityFrameworkCore;

namespace Admin_GM.Data
{
    public class EmployeeDbContext : DbContext
    {
        public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employee { get; set; }
        public DbSet<Assign> Assign { get; set; }
        public DbSet<Login> Login { get; set; }
        public DbSet<Material> Material { get; set; }
        public DbSet<HRRecord> HRRecord { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Assign>()
                .HasOne(a => a.Employee)
                .WithMany()
                .HasForeignKey(a => a.ID_Employee)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Assign>()
                .HasOne(a => a.Material)
                .WithMany()
                .HasForeignKey(a => a.ID_Material)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HRRecord>()
                .HasOne(h => h.Employee)
                .WithMany()
                .HasForeignKey(h => h.ID_Employee)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
