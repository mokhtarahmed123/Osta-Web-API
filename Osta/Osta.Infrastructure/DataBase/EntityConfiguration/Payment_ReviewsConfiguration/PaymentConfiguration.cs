using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Osta.Data.Entities;

namespace Osta.Infrastructure.DataBase.EntityConfiguration.Payment_ReviewsConfiguration
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments", "Payment");


            builder.HasKey(x => x.Id);


            builder.HasOne(x => x.bookings)
                .WithMany()
                .HasForeignKey(x => x.BookingId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Property(x => x.Amount)
                .IsRequired()
                .HasPrecision(18, 2);


            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.Method)
                .IsRequired();


            builder.Property(x => x.TransactionId)
                .HasMaxLength(200)
                .IsRequired(false);


            builder.Property(x => x.CreatedAt)
                .IsRequired();
        }
    }
}