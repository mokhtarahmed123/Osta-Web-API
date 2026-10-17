using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Osta.Domain.Entities.Appointment;

namespace Osta.Infrastructure.DataBase.EntityConfiguration.AppointmentConfiguration
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.ToTable("Appointments", "Appointment");


            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(a => a.ScheduledStart)
               .IsRequired();


            builder.Property(a => a.ScheduledEnd)
                .IsRequired();

<<<<<<< HEAD
=======
            builder.ToTable(t => t.HasCheckConstraint(
                "CK_Booking_ScheduledEnd_After_ScheduledStart",
                "[ScheduledEnd] > [ScheduledStart]"));

>>>>>>> c73a0b6 (Initial commit)

            builder.Property(a => a.ReminderSent)
                .IsRequired()
                .HasDefaultValue(false);


<<<<<<< HEAD
=======

>>>>>>> c73a0b6 (Initial commit)
            builder.Property(a => a.IsApproved)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(a => a.Notes)
                .HasMaxLength(500)
                .IsRequired(false);


            builder.Property(a => a.BookingId)
                .IsRequired();


            builder.HasOne(a => a.Booking)
                .WithOne(b => b.Appointment)
                .HasForeignKey<Appointment>(a => a.BookingId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasIndex(a => a.BookingId)
          .IsUnique();

        }
    }
}
