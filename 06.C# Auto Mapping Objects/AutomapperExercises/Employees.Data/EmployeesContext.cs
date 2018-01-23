namespace Employees.Data
{

    using Microsoft.EntityFrameworkCore;
    using Employees.Models;

    public class EmployeesContext : DbContext
    {
        public EmployeesContext() { }

        public EmployeesContext(DbContextOptions options)
            : base(options) { }

        public DbSet<Employee> Employees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(60);

                entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(60);

                entity.Property(e => e.Address)
                .HasMaxLength(250);
            });
        }


    }
}
