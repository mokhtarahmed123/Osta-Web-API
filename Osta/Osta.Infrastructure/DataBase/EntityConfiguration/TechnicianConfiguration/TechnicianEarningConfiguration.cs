using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Osta.Domain.Entities.Technician;

namespace Osta.Infrastructure.DataBase.EntityConfiguration.TechnicianConfiguration
{
    public class TechnicianEarningConfiguration : IEntityTypeConfiguration<TechnicianEarning>
    {
        public void Configure(EntityTypeBuilder<TechnicianEarning> builder)
        {
            builder.ToTable("TechnicianEarning", "Technician");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.GrossAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(e => e.PlatformFee)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(e => e.NetAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(e => e.EarnedAt)
                .IsRequired();


            builder.HasOne(e => e.Booking)
                .WithMany(b => b.TechnicianEarnings)
                .HasForeignKey(e => e.BookingId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(e => e.Technician)
                .WithMany(t => t.TechnicianEarning)
                .HasForeignKey(e => e.TechnicianId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
