using Microsoft.EntityFrameworkCore;
using Stations.Models;

namespace Stations.Data
{
	public class StationsDbContext : DbContext
	{
		public StationsDbContext()
		{
		}

		public StationsDbContext(DbContextOptions options)
			: base(options)
		{
		}

        public DbSet<Station> Stations { get; set; }

        public DbSet<Train> Trains { get; set; }

        public DbSet<SeatingClass> SeatingClasses { get; set; }

        public DbSet<TrainSeat> TrainSeats { get; set; }

        public DbSet<Trip> Trips { get; set; }

        public DbSet<Ticket> Tickets { get; set; }

        public DbSet<CustomerCard> Cards { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				optionsBuilder.UseSqlServer(Configuration.ConnectionString);
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
            modelBuilder.Entity<Station>()
              .HasAlternateKey(s => s.Name);


            modelBuilder.Entity<Train>()
              .HasAlternateKey(t => t.TrainNumber);


            modelBuilder.Entity<SeatingClass>()
              .HasAlternateKey(sc => new { sc.Name, sc.Abbreviation});

            modelBuilder.Entity<Trip>()
                .HasOne(t => t.OriginStation)
                .WithMany(s => s.TripsFrom)
                .HasForeignKey(t => t.OriginStationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Trip>()
                .HasOne(t => t.DestinationStation)
                .WithMany(s => s.TripsTo)
                .HasForeignKey(t => t.DestinationStationId);

            modelBuilder.Entity<Trip>()
                .HasOne(t => t.Train)
                .WithMany(tr => tr.Trips)
                .HasForeignKey(t => t.TrainId);

            modelBuilder.Entity<TrainSeat>()
               .HasOne(ts => ts.Train)
               .WithMany(tr => tr.TrainSeats)
               .HasForeignKey(t => t.TrainId);

            modelBuilder.Entity<Ticket>()
               .HasOne(t => t.CustomerCard)
               .WithMany(cc => cc.BoughtTickets)
               .HasForeignKey(t => t.CustomerCardId);
        }
	}
}