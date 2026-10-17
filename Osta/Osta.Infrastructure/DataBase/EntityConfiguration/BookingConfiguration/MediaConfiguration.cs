using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Osta.Data.Entities.Booking;

namespace Osta.Infrastructure.DataBase.EntityConfiguration.BookingConfiguration
{
    public class MediaConfiguration : IEntityTypeConfiguration<Media>
    {
        public void Configure(EntityTypeBuilder<Media> builder)
        {
            builder.ToTable("Media", "Booking");


            builder.HasKey(m => m.Id);


            builder.Property(m => m.FileUrl)
                .HasMaxLength(500)
                .IsRequired();


            builder.Property(m => m.FileType)
                .IsRequired()
                    .HasConversion<int>();


            builder.Property(m => m.RepairMediaType)
                .IsRequired()
                .HasConversion<int>();


            builder.Property(m => m.Description)
                .HasMaxLength(500)
                .IsRequired(false);


            builder.Property(m => m.UploadedByUserId)
                .IsRequired();


            builder.Property(m => m.CreatedAt)
                .IsRequired();


            builder.Property(m => m.UpdatedAt)
                .IsRequired(false);


            builder.HasOne(m => m.Booking)
                .WithMany(b => b.Medias)
                .HasForeignKey(m => m.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.UploadedByUser)
                .WithMany()
                .HasForeignKey(m => m.UploadedByUserId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasIndex(m => m.BookingId);

            builder.HasIndex(m => m.UploadedByUserId);
        }
    }
}