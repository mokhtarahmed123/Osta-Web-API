using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Osta.Data.Entities;

namespace Osta.Infrastructure.DataBase.EntityConfiguration.Payment_ReviewsConfiguration
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {

            builder.ToTable("Reviews", "Review");


            builder.HasKey(x => x.Id);


            builder.HasOne(x => x.Booking)
                .WithMany()
                .HasForeignKey(x => x.BookingId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Property(x => x.Rating)
                .IsRequired();


            builder.Property(x => x.Comment)
                .HasMaxLength(1000)
                .IsRequired(false);


            builder.Property(x => x.CreatedAt)
                .IsRequired();


            builder.ToTable(t => t.HasCheckConstraint(
                "CK_Reviews_Rating",
                "[Rating] >= 1 AND [Rating] <= 5"
            ));
        }
    }
}