using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Osta.Data.Entities.Technician;

namespace Osta.Infrastructure.DataBase.EntityConfiguration.TechnicianConfiguration
{
    public class TechnicianAvailabilityConfiguration
        : IEntityTypeConfiguration<TechnicianAvailability>
    {
        public void Configure(EntityTypeBuilder<TechnicianAvailability> builder)
        {

            builder.ToTable("TechnicianAvailabilities", "Technician");


            builder.HasKey(x => x.Id);


            builder.Property(x => x.TechnicianId)
                .IsRequired();


            builder.HasOne(x => x.Technician)
                .WithMany(x => x.Availabilities)
                .HasForeignKey(x => x.TechnicianId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.Property(x => x.DayOfWeek)
                .IsRequired();


            builder.Property(x => x.StartTime)
                .IsRequired()
                .HasColumnType("time");

            builder.Property(x => x.EndTime)
                .IsRequired()
                .HasColumnType("time");


            builder.Property(x => x.IsAvailable)
                .IsRequired()
                .HasDefaultValue(true);


            builder.HasIndex(x => new
            {
                x.TechnicianId,
                x.DayOfWeek
            })
            .IsUnique();

        }
    }
}