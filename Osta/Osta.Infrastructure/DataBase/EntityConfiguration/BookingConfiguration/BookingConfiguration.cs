using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Osta.Data.Entities.Booking;
using Osta.Data.Enum;
using Osta.Domain.Entities.Appointment;

namespace Osta.Infrastructure.DataBase.EntityConfiguration.BookingConfiguration
{
    public class BookingConfiguration : IEntityTypeConfiguration<Bookings>
    {
        public void Configure(EntityTypeBuilder<Bookings> builder)
        {
            builder.ToTable("Bookings", "Booking");


            builder.HasKey(b => b.Id);


            builder.Property(b => b.CustomerId)
                .IsRequired();

            builder.HasOne(b => b.Customer)
                .WithMany()
                .HasForeignKey(b => b.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Property(b => b.TechnicianId)
                .IsRequired();

            builder.HasOne(b => b.Technician)
                .WithMany()
                .HasForeignKey(b => b.TechnicianId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Property(b => b.Street)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(b => b.Area)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(b => b.City)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(b => b.Governorate)
                .HasMaxLength(100)
                .IsRequired();


            builder.Property(b => b.BookingDate)
                .IsRequired();

            builder.Property(b => b.Status)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(BookingStatus.Pending);


            builder.HasMany(b => b.Payments)
                .WithOne()
                .HasForeignKey(p => p.BookingId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasMany(b => b.Reviews)
                .WithOne()
                .HasForeignKey(r => r.BookingId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasMany(b => b.Medias)
                .WithOne()
                .HasForeignKey(m => m.BookingId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasMany(b => b.Complaints)
                .WithOne(c => c.Booking)
                .HasForeignKey(c => c.BookingId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasMany(b => b.TechnicianEarnings)
                .WithOne()
                .HasForeignKey(e => e.BookingId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(b => b.Appointment)
                .WithOne(a => a.Booking)
                .HasForeignKey<Appointment>(a => a.BookingId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}