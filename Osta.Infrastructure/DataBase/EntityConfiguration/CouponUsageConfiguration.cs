using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Osta.Domain.Entities.Payment___Reviews;

namespace Osta.Infrastructure.DataBase.EntityConfiguration
{
    public class CouponUsageConfiguration : IEntityTypeConfiguration<CouponUsage>
    {
        public void Configure(EntityTypeBuilder<CouponUsage> builder)
        {
            builder.ToTable("CouponUsages", schema: "Payment");

            builder.HasKey(cu => cu.Id);

            builder.HasOne(cu => cu.Coupon)
                .WithMany(c => c.Usages)
                .HasForeignKey(cu => cu.CouponId);

            builder.HasOne(cu => cu.User)
                .WithMany()
                .HasForeignKey(cu => cu.UserId);

            builder.HasOne(cu => cu.Booking)
                .WithMany()
                .HasForeignKey(cu => cu.BookingId)
                .IsRequired(false);

            // يمنع نفس اليوزر يستخدم نفس الكوبون مرتين
            builder.HasIndex(cu => new { cu.CouponId, cu.UserId }).IsUnique();
        }
    }
}
