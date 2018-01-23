namespace P01_BillsPaymentSystem.Data.EntityConfig
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using P01_BillsPaymentSystem.Data.Models;

    class BankAccountConfig : IEntityTypeConfiguration<BankAccount>
    {
        public void Configure(EntityTypeBuilder<BankAccount> builder)
        {
            builder.HasKey(e => e.BankAccountId);

            builder.Property(e => e.BankName)
                .IsUnicode()
                .HasMaxLength(50);

            builder.Property(e => e.SwiftCode)
                .IsUnicode(false)
                .HasMaxLength(20);

            builder.Ignore(e => e.PaymentMethodId);
        }
    }
}
