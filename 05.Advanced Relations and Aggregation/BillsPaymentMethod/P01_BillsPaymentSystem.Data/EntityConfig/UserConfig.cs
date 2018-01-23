namespace P01_BillsPaymentSystem.Data.EntityConfig
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using P01_BillsPaymentSystem.Data.Models;

    class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(e => e.UserId);

            builder.Property(e => e.FirstName)
                .IsUnicode()
                .HasMaxLength(50);

            builder.Property(e => e.LastName)
                .IsUnicode()
                .HasMaxLength(50);

            builder.Property(e => e.Email)
               .IsUnicode(false)
               .HasMaxLength(80);

            builder.Property(e => e.Password)
               .IsUnicode(false)
               .HasMaxLength(25);
        }
    }
}
