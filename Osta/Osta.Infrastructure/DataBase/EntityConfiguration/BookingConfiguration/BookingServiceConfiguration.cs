using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Osta.Data.Entities;

namespace Osta.Infrastructure.DataBase.EntityConfiguration.BookingConfiguration
{
    public class BookingServiceConfiguration : IEntityTypeConfiguration<BookingService>
    {
        public void Configure(EntityTypeBuilder<BookingService> builder)
        {
            builder.ToTable("BookingServices", "Booking");


            builder.HasKey(bs => new
            {
                bs.BookingId,
                bs.ServiceId
            });


            builder.HasOne(bs => bs.Booking)
                .WithMany()
                .HasForeignKey(bs => bs.BookingId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(bs => bs.Service)
                .WithMany()
                .HasForeignKey(bs => bs.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Property(bs => bs.PriceAtBooking)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
        }
    }
}
