namespace FastFood.Data
{
    using FastFood.Models;
    using Microsoft.EntityFrameworkCore;

    public class FastFoodDbContext : DbContext
	{
		public FastFoodDbContext()
		{
		}

		public FastFoodDbContext(DbContextOptions options)
			: base(options)
		{
		}

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
		{
			if (!builder.IsConfigured)
			{
				builder.UseSqlServer(Configuration.ConnectionString);
			}
		}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Order>().Ignore(e => e.TotalPrice);

            builder.Entity<Position>()
              .HasAlternateKey(p => p.Name);

            builder.Entity<Item>()
              .HasAlternateKey(i => i.Name);

            builder.Entity<OrderItem>().HasKey(e => new { e.OrderId, e.ItemId });

            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);

            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Item)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.ItemId);

            builder.Entity<Item>()
               .HasOne(i => i.Category)
               .WithMany(c => c.Items)
               .HasForeignKey(i => i.CategoryId);

            builder.Entity<Employee>()
               .HasOne(e => e.Position)
               .WithMany(p => p.Employees)
               .HasForeignKey(e => e.PositionId);

            builder.Entity<Order>()
               .HasOne(o => o.Employee)
               .WithMany(e => e.Orders)
               .HasForeignKey(e => e.EmployeeId);
        }
	}
}