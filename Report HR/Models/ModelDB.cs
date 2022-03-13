using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Report_HR.Models
{
    public partial class ModelDB : DbContext
    {
        public ModelDB()
            : base("name=ModelDB")
        {
        }

        public virtual DbSet<Absence> Absences { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<EmpSetting> EmpSettings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .HasMany(e => e.EmpSettings)
                .WithRequired(e => e.Employee)
                .WillCascadeOnDelete(false);
        }
    }
}
