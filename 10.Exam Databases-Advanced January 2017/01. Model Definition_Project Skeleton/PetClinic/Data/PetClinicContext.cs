namespace PetClinic.Data
{
    using Microsoft.EntityFrameworkCore;
    using PetClinic.Models;

    public class PetClinicContext : DbContext
    {
        public PetClinicContext() { }

        public PetClinicContext(DbContextOptions options)
            :base(options) { }

        public DbSet<Animal> Animals { get; set; }
        public DbSet<Passport> Passports { get; set; }
        public DbSet<Procedure> Procedures { get; set; }
        public DbSet<Vet> Vets { get; set; }
        public DbSet<ProcedureAnimalAid> ProceduresAnimalAids { get; set; }
        public DbSet<AnimalAid> AnimalAids { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Procedure>().Ignore(p => p.Cost);

            builder.Entity<Vet>()
              .HasAlternateKey(v => v.PhoneNumber);

            builder.Entity<AnimalAid>()
            .HasAlternateKey(a => a.Name);

            builder.Entity<ProcedureAnimalAid>().HasKey(pa => new { pa.ProcedureId, pa.AnimalAidId });

            builder.Entity<ProcedureAnimalAid>()
              .HasOne(pa => pa.Procedure)
              .WithMany(p => p.ProcedureAnimalAids)
              .HasForeignKey(pa => pa.ProcedureId);

            builder.Entity<ProcedureAnimalAid>()
            .HasOne(pa => pa.AnimalAid)
            .WithMany(a => a.AnimalAidProcedures)
            .HasForeignKey(pa => pa.AnimalAidId);

            builder.Entity<Procedure>()
                .HasOne(p => p.Animal)
                .WithMany(a => a.Procedures)
                .HasForeignKey(p => p.AnimalId);

            builder.Entity<Procedure>()
               .HasOne(p => p.Vet)
               .WithMany(a => a.Procedures)
               .HasForeignKey(p => p.VetId);
        }
    }
}
