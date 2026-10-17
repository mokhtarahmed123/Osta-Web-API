using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Osta.Domain.Entities.Customer;

namespace Osta.Infrastructure.DataBase.EntityConfiguration.Payment_ReviewsConfiguration
{
    public class CouponsConfiguration : IEntityTypeConfiguration<Coupons>
    {
        public void Configure(EntityTypeBuilder<Coupons> builder)
        {

            builder.ToTable("Coupons", "Payment");


            builder.HasKey(x => x.Id);


            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();


            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(20);


            builder.HasIndex(x => x.Code)
                .IsUnique();


            builder.Property(x => x.DiscountType)
                .IsRequired();


            builder.Property(x => x.DiscountValue)
                .IsRequired()
                .HasPrecision(18, 2);


            builder.Property(x => x.StartDate)
                .IsRequired();


            builder.Property(x => x.EndDate)
                .IsRequired();

            builder.ToTable(t => t.HasCheckConstraint(
             "CK_YourTable_EndDate_After_StartDate",
             "[EndDate] > [StartDate]"));


            builder.Property(x => x.UsageLimit)
                .IsRequired();


            builder.Property(x => x.UsedCount)
                .IsRequired()
                .HasDefaultValue(0);


            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);


            builder.Property(x => x.CreatedAt)
                .IsRequired();


            builder.HasMany(x => x.Usages)
                .WithOne(x => x.Coupon)
                .HasForeignKey(x => x.CouponId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}