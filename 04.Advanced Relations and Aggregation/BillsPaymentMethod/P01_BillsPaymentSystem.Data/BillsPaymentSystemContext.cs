namespace P01_BillsPaymentSystem.Data
{
    using Microsoft.EntityFrameworkCore;
    using P01_BillsPaymentSystem.Data.Models;
    using P01_BillsPaymentSystem.Data.EntityConfig;

    public class BillsPaymentSystemContext : DbContext
    {
        public BillsPaymentSystemContext()
        {

        }

        public BillsPaymentSystemContext(DbContextOptions options)
            :base(options)
        {

        }

        public DbSet<User> Users { get; set; }

        public DbSet<PaymentMethod> PaymentMethods { get; set; }

        public DbSet<CreditCard> CreditCards { get; set; }

        public DbSet<BankAccount> BankAccounts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfig());

            modelBuilder.ApplyConfiguration(new PaymentMethodConfig());

            modelBuilder.ApplyConfiguration(new BankAccountConfig());

            modelBuilder.ApplyConfiguration(new CreditCardConfig());

        }

    }
}
