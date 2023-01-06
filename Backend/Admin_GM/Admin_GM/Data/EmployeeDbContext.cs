
using Admin_GM.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Runtime.Serialization;
using System;
using System.Configuration;
//using TrainingManagement.Entitities.Models; 


namespace Admin_GM.Data
{
    public class EmployeeDbContext : DbContext 
    {
        public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options)
            : base(options) 
        { 
        }

        public DbSet<Employee> Employee { get; set; } //<Employee> is the class | Employee is the table
        public DbSet<Assign> Assign { get; set; }
        public DbSet<Login> Login { get; set; }
        public DbSet<Material> Material { get; set; }


        //----------------------------- Copied Section From a Friend ------------------------------------
        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured) //I don't know how to fix the "ConnectionStrings" Error
            {
                optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Assign>(os =>
            {
                os.HasKey(u => u.ID).HasName("PK_FormationId"); //not sure what the HasName does..
                os.Property(u => u.ID_Employee).IsRequired(true);
                os.Property(u => u.ID_Material).IsRequired(true);
                os.Property(u => u.Start_Date).IsRequired(true);
                os.Property(u => u.End_Date).IsRequired(true);
            });
            modelBuilder.Entity<Material>(os =>
            {
                os.HasKey(u => u.ID).HasName("PK_FormateurId"); //not sure what the HasName does..
                os.Property(u => u.Reference).IsRequired(true).HasMaxLength(100);
                os.Property(u => u.IsTaken).IsRequired(true);
                os.Property(u => u.Name).IsRequired(true).HasMaxLength(100);
                os.Property(u => u.Price).IsRequired(true);
                os.Property(u => u.Purchase_Date).IsRequired(true);
                os.Property(u => u.Brand).IsRequired(true).HasMaxLength(100);
            });
            modelBuilder.Entity<Employee>(os =>
            {
                os.HasKey(u => u.ID).HasName("PK_EmployeId"); //not sure what the HasName does..
                os.Property(u => u.Emp_Function).IsRequired(true).HasMaxLength(200);
                os.Property(u => u.Prename).IsRequired(true).HasMaxLength(100);
                os.Property(u => u.Name).IsRequired(true).HasMaxLength(100);
                os.Property(u => u.Home_Address).IsRequired(true).HasMaxLength(255);
                os.Property(u => u.Phone_Number).IsRequired(true).HasMaxLength(255);
                os.Property(u => u.Email).IsRequired(true).HasMaxLength(150); 
                os.Property(u => u.CIN).HasMaxLength(8);


            });
        } */
        //----------------------------- Copied Section From a Friend ------------------------------------
    }
}
