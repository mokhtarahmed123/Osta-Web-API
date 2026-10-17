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
<<<<<<< HEAD


=======
>>>>>>> c73a0b6 (Initial commit)
            builder.HasKey(bs => new
            {
                bs.BookingId,
                bs.ServiceId
            });

<<<<<<< HEAD

=======
>>>>>>> c73a0b6 (Initial commit)
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
