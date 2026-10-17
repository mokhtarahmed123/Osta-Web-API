using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Osta.Domain.Entities.Payment___Reviews;

namespace Osta.Infrastructure.DataBase.EntityConfiguration.Payment_ReviewsConfiguration
{
    public class CouponUsageConfiguration : IEntityTypeConfiguration<CouponUsage>
    {
        public void Configure(EntityTypeBuilder<CouponUsage> builder)
        {
            builder.ToTable("CouponUsages", "Payment");

            // Primary Key
            builder.HasKey(x => x.Id);

            // Coupon
            builder.HasOne(x => x.Coupon)
                .WithMany(x => x.Usages)
                .HasForeignKey(x => x.CouponId)
                .OnDelete(DeleteBehavior.Cascade);

            // User
            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Booking - Optional
            builder.HasOne(x => x.Booking)
                .WithMany()
                .HasForeignKey(x => x.BookingId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // UserId
            builder.Property(x => x.UserId)
                .IsRequired();

            // UsedAt
            builder.Property(x => x.UsedAt)
                .IsRequired();

            // A user can use a coupon only once
            builder.HasIndex(x => new
            {
                x.CouponId,
                x.UserId
            })
            .IsUnique();
        }
    }
}
