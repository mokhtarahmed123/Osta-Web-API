using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Osta.Data.Entities.Technician;
using Osta.Data.Enum;
using Osta.Domain.Entities.Technician;

namespace Osta.Infrastructure.DataBase.EntityConfiguration.TechnicianConfiguration
{
    public class TechniciansConfiguration : IEntityTypeConfiguration<Technicians>
    {
        public void Configure(EntityTypeBuilder<Technicians> builder)
        {

            builder.ToTable("Technicians", "Technician");


            builder.HasKey(x => x.Id);


            builder.Property(x => x.Id)
                .IsRequired();

            builder.HasOne(x => x.User)
                .WithOne()
                .HasForeignKey<Technicians>(x => x.Id)
                .OnDelete(DeleteBehavior.Cascade);


            builder.Property(x => x.Bio)
                .HasMaxLength(500)
                .IsRequired(false);


            builder.Property(x => x.IsVerified)
                .IsRequired()
                .HasDefaultValue(false);


            builder.Property(x => x.NationalId)
                .IsRequired()
                .HasMaxLength(14);


            builder.Property(x => x.Rating)
                .IsRequired()
                .HasDefaultValue(0);


            builder.Property(x => x.TotalReviews)
                .IsRequired()
                .HasDefaultValue(0);


            builder.Property(x => x.CompletedBookings)
                .IsRequired()
                .HasDefaultValue(0);


            builder.Property(x => x.YearsOfExperience)
                .IsRequired();


            builder.Property(x => x.CreatedAt)
                .IsRequired();


            builder.Property(x => x.ReasonOfReject)
                .IsRequired(false);


            builder.Property(x => x.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(StatusOfTechnicianRequestEnum.None);

            builder.HasIndex(x => new
            {
                x.Status,
                x.IsVerified
            });


            builder.HasMany(x => x.Availabilities)
                .WithOne(x => x.Technician)
                .HasForeignKey(x => x.TechnicianId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasMany(x => x.TechnicianServiceArea)
                .WithOne(x => x.Technician)
                .HasForeignKey(x => x.TechnicianId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasMany(x => x.TechnicianServices)
                .WithOne(x => x.Technician)
                .HasForeignKey(x => x.TechnicianId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasMany(x => x.Bookings)
                .WithOne(x => x.Technician)
                .HasForeignKey(x => x.TechnicianId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasMany(x => x.FavoriteTechnicians)
                .WithOne(x => x.Technician)
                .HasForeignKey(x => x.TechnicianId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasMany(x => x.TechnicianEarning)
                .WithOne(x => x.Technician)
                .HasForeignKey(x => x.TechnicianId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(x => x.TechnicianWallet)
                .WithOne(x => x.Technician)
                .HasForeignKey<TechnicianWallet>(x => x.TechnicianId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasMany(x => x.TechnicianPayouts)
                .WithOne(x => x.Technician)
                .HasForeignKey(x => x.TechnicianId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}